import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
import java.util.Random;

import processing.core.PApplet;
import processing.core.PVector;
import processing.data.JSONArray;
import processing.data.JSONObject;

public class ClosedHandDrawTest extends PApplet {

	KinectBodyDataProvider kinectReader;
	KinectBodyData bodyData;
	public static float PROJECTOR_RATIO = 1080f / 1920.0f;

	PersonTracker pTracker = new PersonTracker();
	HashMap<Long, Person> people = new HashMap<Long, Person>();

	// default drawing hand
	Long drawerID = null;
	Boolean handDetected = false;
	String trackingHand = null; // Body.LEFT_HAND_STATE; Body.RIGHT_HAND_STATE;

	Boolean isDrawing = false;
	Boolean isSelecting = false;
	Long drawerId;

	HashSet<Plane3d> planes = new HashSet<Plane3d>();
	HashMap<Long, Shape3d> shapes = new HashMap<Long, Shape3d>();
	Shape3d currShape;
	static Long sid;

	ArrayList<Point3d> traces = new ArrayList<Point3d>();

	/**
	 * Will be called on each frame
	 */
	public void draw() {
		setScale(.5f);

		background(173, 216, 230);

		bodyData = kinectReader.getMostRecentData();
		// bodyData = kinectReader.getData();

		update(bodyData);
		// drawing(isDrawing);
	}

	public void update(KinectBodyData bodyData) {
		pTracker.update(bodyData);

		for (Long id : pTracker.getEnters())
			people.put(id, new Person());

		for (Long id : pTracker.getExits())
			people.remove(id);

		for (Body b : pTracker.getPeople().values()) {
			Person p = people.get(b.getId());
			p.update(b);
			drawing2(p);
			openHand(p.body.getJoint(Body.HAND_RIGHT));
		}
	}

	public Point3d hoverOver() {
		if (traces == null || traces.isEmpty())
			return null;

		double xSum = 0;
		double ySum = 0;
		double zSum = 0;

		for (Point3d pt : traces) {
			xSum += pt.getX();
			ySum += pt.getY();
			zSum += pt.getZ();
		}

		return new Point3d(xSum / traces.size(), ySum / traces.size(), zSum / traces.size());

	}

	// bug: kinect seems to detect both hands open all the time
	public void drawingHand(Body bd) {
		if (bd.rightHandOpen) {
//			if (!bd.leftHandOpen) { // only right hand opens
				if (!handDetected) {// no hand is being tracked rn
					trackingHand = Body.HAND_RIGHT;
					handDetected = true;
				} else {
					if (trackingHand != Body.HAND_RIGHT)
						handDetected = false;
//				}
//
//			} else { // both hands are open
//				handDetected = false;
			}
		} else { // right hand closed
			if (bd.leftHandOpen) { // only left hand opens
				if (!handDetected) {// no hand is being tracked rn
					trackingHand = Body.HAND_LEFT;
					handDetected = true;
				} else
					return; // does nothing
			} else // both hands open
				handDetected = false;
		}
	}

	// before drawing, call select method to check if any shape is in the range
	// if not, drawing
	// add drawing points within seconds??
	// suspend hand??
	public void drawing2(Person p) {
		Body bd = p.body;
		long id = bd.getId(); // body id
		drawingHand(bd);

		// if the person's drawing hand is open
		if (handDetected) {
			PVector rt = bd.getJoint(trackingHand);
			// no one else initiates the drawing mood
			if (drawerID == null) {
				// System.out.println("drawer id null");
				if (!isDrawing) {
					// System.out.println("not drawing");
					if (!isSelecting && traces.size() < 10) { // either idle or no current tracking traces
						traces.add(new Point3d(rt.x, rt.y, rt.z));
						// System.out.println("add traces");
						// isSelecting = true;
					} else {
						if (traces.size() >= 10) {
							// average traces
							isSelecting = select(hoverOver()); // update current shape if is selecting
							if (isSelecting) {
								traces = new ArrayList<Point3d>();
							} else { // create a new shape
								System.out.println("created a new shape");
								drawerID = new Long(id);
								currShape = new Shape3d(); // start a new shape
								traces = new ArrayList<Point3d>();
							}
						}
					}
				} else {
					// traces update might lose the drawer ID
					// detect right hand open, but isDrawing is still on
				}
			} else if (drawerID == id) { // same drawer
				if (isSelecting) {
					if (currShape != null) { // a shape is selected as particular
						if (traces.size() >= 10)
							traces.remove(0);
						traces.add(new Point3d(rt.x, rt.y, rt.z));
						// should be discussed later
						double newExtru = currShape.onPlane.signedPtDist(hoverOver());
						if (!currShape.isClosed) // will be discussed later; on definition of closed shape
							currShape.extrusion = newExtru;
					} else { // should not go into this case
						isSelecting = false;
					}
				} else { // no selection
					if (!isDrawing) {
						isDrawing = true;
						// create a new shape
						drawerID = new Long(id);
						currShape = new Shape3d(); // start a new shape
						traces = new ArrayList<Point3d>();
						if (rt != null) {
							currShape.addVertex(new Point3d(rt.x, rt.y, rt.z));
							openHand(rt); // draw out for checking
						}
					} else { // still drawing
						if (rt != null) {
							if (traces.size() >= 20)
								traces.remove(0);
							traces.add(new Point3d(rt.x, rt.y, rt.z));
							currShape.addVertex(hoverOver());
							openHand(rt); // draw out for checking
						}
					}
				}
			} else if (drawerID != id)
				return;
		} else {
			if (isDrawing && drawerID != null) { // lose track of drawing person
				if (traces.size() > 0) {
					traces.remove(0);
				} else if (traces.size() == 0) {
					isDrawing = false; // resumes drawing state
					drawerID = null;
					// start building the current shape
					try {
						sid = createShapeId(id);
						currShape.buildShape(sid);
						System.out.println(currShape.toString());
						shapes.put(sid, currShape);
						System.out.println("shapes size: " + shapes.size());
					} catch (IllegalArgumentException e) {
						// e.printStackTrace();
						System.out.println("invalid building shape");
						System.out.println("# shapes" + shapes.size());
					}
				}
			}
		}
	}

	/**
	 * Mergs drawerID and a random number to form the shapeID
	 * 
	 * @param drawerID
	 * @return
	 */
	public Long createShapeId(long drawerID) {
		Random ran = new Random();
		int x = ran.nextInt(10); // give a number 1 - 9
		Long lx = new Long(x);
		String l3 = Long.toString(lx) + Long.toString(drawerID);
		Long id = Long.valueOf(l3).longValue(); // converting String to long
		System.out.println(id.toString());
		return id;
	}

	public void openHand(PVector h) {
		int color = color(0, 255, 255);
		fill(color);
		strokeWeight(.1f);
		drawIfValid(h);
	}

	public void drawIfValid(PVector vec) {
		if (vec != null) {
			ellipse(vec.x, vec.y, .1f, .1f);
		}
	}

	/**
	 * NEW METHOD called on if drawing mood on
	 * 
	 * @param pt
	 */
	// loop through existing shape files
	// set current drawing shape if selected
	public boolean select(Point3d pt) {
		// no shapes around
		if (shapes.isEmpty())
			return false;
		for (Shape3d sp : shapes.values()) {
			// shapes.get(sp.getId());
			if (sp.contains(pt)) {
				currShape = sp; // update current shape
				return true;
			}
		}
		return false;
	}

	// public void stopDrawing() {
	// List<Point3d> copy = new ArrayList<>(planeMarks);
	// planeMarks = new ArrayList<Point3d>();
	// isDrawing = false;
	// System.out.println("Stop Drawing");
	// // System.out.println(copy.size());
	// // System.out.println(copy.toString());
	// OrthogonalRegression3D or = new OrthogonalRegression3D((ArrayList<Point3d>)
	// copy);
	// // or.fitPlane();
	// try {
	// Plane3d plane = or.fitPlane(1.0);
	// // Plane3d plane = planeDetection(marks, 0.05, 0.6);
	// if (plane != null) {
	// System.out.println("Finished" + plane.toString());
	// noLoop();
	// }
	// } catch (
	//
	// IllegalArgumentException e) {
	// e.printStackTrace();
	// }
	// }

	public Plane3d loadPlane() {
		JSONObject json = loadJSONObject("plane.json");
		JSONArray normArr = json.getJSONArray("norm");
		double[] arr = new double[3];
		for (int i = 0; i < 3; i++)
			arr[i] = normArr.getDouble(i);
		return new Plane3d(new Vec3d(arr), json.getDouble("dist"));
	}

	public void savePlane(String fileName, Plane3d plane) {
		// JSONObject json = new JSONObject();
		// Vec3d norm = plane.getNorm();
		// json.put("norm", norm.getArr());
		// json.put("dist", plane.getD());
		saveJSONObject(plane.serialize(), "plane.json");
	}

	public Plane3d selectPlane(PVector hand, double tol) {
		Point3d pt = new Point3d(hand.x, hand.y, hand.z);
		for (Plane3d plane : planes) {
			if (plane.hasPoint(pt, tol))
				return plane;
		}
		return null;
	}

	public void settings() {
		createWindow(true, false, .5f);
	}

	public void createWindow(boolean useP2D, boolean isFullscreen, float windowsScale) {
		if (useP2D) {
			if (isFullscreen) {
				fullScreen(P2D);
			} else {
				size((int) (1920 * windowsScale), (int) (1080 * windowsScale), P2D);
			}
		} else {
			if (isFullscreen) {
				fullScreen();
			} else {
				size((int) (1920 * windowsScale), (int) (1080 * windowsScale));
			}
		}
	}

	public static void main(String[] args) {
		PApplet.main(ClosedHandDrawTest.class.getName());
		// try {
		// // load shape for given name passed in the argument: sid = ...
		// Shape3d temp = ShapeData.loadShape(String.valueOf(sid));
		// // System.out.println(temp.toString());
		// } catch (IOException e) {
		// e.printStackTrace();
		// }
	}

	public void setup() {
		try {
			kinectReader = new KinectBodyDataProvider("verticalwClosed.kinect", 1);
		} catch (IOException e) {
			System.out.println("Unable to creat e kinect producer");
		}
		// kinectReader = new KinectBodyDataProvider(8008);
		kinectReader.start();
	}

	// use lower numbers to zoom out (show more of the world)
	// zoom of 1 means that the window is 2 meters wide and appox 1 meter tall.
	public void setScale(float zoom) {
		scale(zoom * width / 2.0f, zoom * -width / 2.0f);
		translate(1f / zoom, -PROJECTOR_RATIO / zoom);
	}

}
