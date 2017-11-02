import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

import processing.core.PApplet;
import processing.core.PVector;

public class ClosedHandDrawTest extends PApplet {

	KinectBodyDataProvider kinectReader;
	KinectBodyData bodyData = new KinectBodyData("");

	Long firstPersonId = null;
	HashMap<Long, Person> tracks = new HashMap<Long, Person>();
	PersonTracker tracker = new PersonTracker();

	// default drawing hand
	Point3d handRight = new Point3d();

	ArrayList<Point3d> marks = new ArrayList<Point3d>();

	Boolean isDrawing = false;
	Long drawerId;
	public static float PROJECTOR_RATIO = 1080f / 1920.0f;

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
	// public ClosedHandDrawTest() {
	// kinectReader = new KinectBodyDataProvider(8008);
	// kinectReader.start();
	// }

	// use lower numbers to zoom out (show more of the world)
	// zoom of 1 means that the window is 2 meters wide and appox 1 meter tall.
	public void setScale(float zoom) {
		scale(zoom * width / 2.0f, zoom * -width / 2.0f);
		translate(1f / zoom, -PROJECTOR_RATIO / zoom);
	}

	public void settings() {
		createWindow(true, true, .5f);
	}

	public void setup() {
		try {
			kinectReader = new KinectBodyDataProvider("openClosedHand.kinect", 2);
		} catch (IOException e) {
			System.out.println("Unable to creat e kinect producer");
		}
		// kinectReader = new KinectBodyDataProvider(8008);
		kinectReader.start();
	}

	// called on each frame
	public void draw() {
		setScale(.5f);

		// case BLUE:
		background(173, 216, 230);

		KinectBodyData bodyData = kinectReader.getMostRecentData();
		// bodyData = kinectReader.getData();
		tracker.update(bodyData);

		for (Long id : tracker.getEnters()) {
			tracks.put(id, new Person());
		}
		for (Long id : tracker.getExits()) {
			tracks.remove(id);
		}

		for (Body b : tracker.getPeople().values()) {
			Person p = tracks.get(b.getId());
			p.update(b);
			drawingCmd(p);
		}
		
		if (isDrawing) {
			int color = color(0, 255, 255);
			fill(color);
			strokeWeight(.1f);
			Body drawer = tracks.get(drawerId).body;
			PVector handRight = drawer.getJoint(Body.HAND_RIGHT);
			drawIfValid(handRight);
			if (handRight != null) {
				marks.add(new Point3d(handRight.x, handRight.y, handRight.z));
			}
		}
	}

	public void drawIfValid(PVector vec) {
		if (vec != null) {
			ellipse(vec.x, vec.y, .1f, .1f);
		}
	}

	public void drawingCmd(Person p) {
		// current body
		Body bd = p.body;
		// current body id
		long id = bd.getId();
		if (bd != null) {
			// not in drawing mood
			if (isDrawing == false) {
				// check if right drawing hand is open
				if (bd.rightHandOpen == true) {
					startDrawing(id);
				}
			}
			// in drawing
			else {
				// keep track of the drawing person
				if (id == drawerId) {
					if (!bd.rightHandOpen)
						stopDrawing();
				}
			}
		}
	}

	public void startDrawing(Long id) {
		isDrawing = true;
		drawerId = id;
		System.out.println("Drawing");
	}

	public void stopDrawing() {
		List<Point3d> copy = new ArrayList<>(marks);
		marks = new ArrayList<Point3d>();
		isDrawing = false;
		System.out.println("Stop Drawing");
//		System.out.println(copy.size());
//		System.out.println(copy.toString());
		OrthogonalRegression3D or = new OrthogonalRegression3D((ArrayList<Point3d>) copy);
		// or.fitPlane();
		try {
			Plane3d plane = or.fitPlane(1.0);
			// Plane3d plane = planeDetection(marks, 0.05, 0.6);
			if (plane != null) {
				System.out.println("Finished" + plane.toString());
			}
		} catch (

		IllegalArgumentException e) {
			
		}

	}

	// public void addPath(Boolean isDrawing) {
	// Point3d handLeft = tracks.get(drawerId).body.getJoint(Body.HAND_LEFT);
	// }

	public void closePath() {

	}

	public static void main(String[] args) {
		PApplet.main(ClosedHandDrawTest.class.getName());
	}

}
