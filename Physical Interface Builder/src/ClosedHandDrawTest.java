import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Random;

import com.sun.istack.internal.Nullable;

import processing.core.PApplet;
import processing.core.PShape;
import processing.core.PVector;
import processing.data.JSONArray;
import processing.data.JSONObject;

public class ClosedHandDrawTest extends PApplet {

	KinectBodyDataProvider kinectReader;
	KinectBodyData bodyData;

	Long drawerID = null;
	PersonTracker pTracker = new PersonTracker();
	HashMap<Long, Person> people = new HashMap<Long, Person>();

	HashSet<Plane3d> planes = new HashSet<Plane3d>();
	HashMap<Long, Shape3d> shapes = new HashMap<Long, Shape3d>();
	Shape3d currShape;

	// default drawing hand
	String trakcingHand = Body.RIGHT_HAND_STATE;

	ArrayList<Point3d> planeMarks = new ArrayList<Point3d>();

	Boolean isDrawing = false;
	Long drawerId;

	Plane3d workingPlane;

	public static float PROJECTOR_RATIO = 1080f / 1920.0f;

	public void setup() {
		try {
			kinectReader = new KinectBodyDataProvider("openDrawShape.kinect", 2);
		} catch (IOException e) {
			System.out.println("Unable to creat e kinect producer");
		}
		// kinectReader = new KinectBodyDataProvider(8008);
		kinectReader.start();
	}

	/**
	 * Will be called on each frame
	 */
	public void draw() {
		setScale(.5f);

		// case BLUE:
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
			drawing(p);
			// openHand();
		}
	}

	public void drawing(Person p) {
		Body bd = p.body;
		long id = bd.getId(); // body id
		// if the person's drawing hand is open
		if (bd.rightHandOpen) {
			// no one else initiates the drawing mood
			if (drawerID == null || isDrawing == false) {
				drawerID = new Long(id);
				isDrawing = true;
				currShape = new Shape3d();
			}
			// same drawer
			else if (drawerID == id) {
				// currently working on a shape
				if (isDrawing == true) {
					PVector rt = bd.getJoint(Body.HAND_RIGHT);

					if (rt != null) {
						currShape.addVertex(new Point3d(rt.x, rt.y, rt.z));
						openHand(rt);
					}
				}
			} else if (drawerID != id)
				return;
		} else {
			if (drawerID != null && drawerID == id && !currShape.isClosed) {
				currShape.buildShape();
				shapes.put(createShapeId(id), currShape);
				isDrawing = false; // resumes drawing state
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
		return Long.valueOf(l3).longValue(); // converting String to long
	}

	public void openHand(PVector h) {
		// if (isDrawing)
		// return;
		// else {
		int color = color(0, 255, 255);
		fill(color);
		strokeWeight(.1f);
		// if (drawerID == null)
		// return;
		//// System.out.println(drawerId.toString());
		// Body drawer = people.get(drawerId).body;
		// PVector handRight = drawer.getJoint(Body.HAND_RIGHT);
		drawIfValid(h);
		// if (handRight != null) {
		// planeMarks.add(new Point3d(handRight.x, handRight.y, handRight.z));
		// }
		// }
	}

	public void drawIfValid(PVector vec) {
		if (vec != null) {
			ellipse(vec.x, vec.y, .1f, .1f);
		}
	}

	public void stopDrawing() {
		List<Point3d> copy = new ArrayList<>(planeMarks);
		planeMarks = new ArrayList<Point3d>();
		isDrawing = false;
		System.out.println("Stop Drawing");
		// System.out.println(copy.size());
		// System.out.println(copy.toString());
		OrthogonalRegression3D or = new OrthogonalRegression3D((ArrayList<Point3d>) copy);
		// or.fitPlane();
		try {
			Plane3d plane = or.fitPlane(1.0);
			// Plane3d plane = planeDetection(marks, 0.05, 0.6);
			if (plane != null) {
				System.out.println("Finished" + plane.toString());
				noLoop();
			}
		} catch (

		IllegalArgumentException e) {
			e.printStackTrace();
		}
	}

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

	// use lower numbers to zoom out (show more of the world)
	// zoom of 1 means that the window is 2 meters wide and appox 1 meter tall.
	public void setScale(float zoom) {
		scale(zoom * width / 2.0f, zoom * -width / 2.0f);
		translate(1f / zoom, -PROJECTOR_RATIO / zoom);
	}

	public static void main(String[] args) {
		PApplet.main(ClosedHandDrawTest.class.getName());
	}

}
