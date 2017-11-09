import processing.core.PApplet;
import processing.core.PShape;
import processing.data.JSONArray;
import processing.data.JSONObject;

/**
 * 
 */

/**
 * @author Zhiling
 *
 */
public class Shape extends PApplet {

	long shapeID;

	Plane3d onPlane;
	PShape shape;

	boolean closed;

	//
	// public static final String CIRCLE = "CIRCLE";
	// public static final String RECTANGLE = "RECT";
	// public static final String SQUARE = "SQUA";

	// ArrayList<Vec3d> points; // points representing the polygon

	public Shape(String cmd, Plane3d plane, Point3d pt, float w, float h) {
		shape = createShape();
		onPlane = plane;
		default2dShape(cmd, (float) pt.getX(), (float) pt.getY(), w, h);
	}

	public void default2dShape(String str, float x, float y, float w, float h) {
		switch (str) {
		case "rectangle":
			shape = createShape(RECT, x, y, w, h);
			break;
		case "circle":
			shape = createShape(ELLIPSE, x, y, w, h);
			break;
		case "triangle":
			shape = createShape(ELLIPSE, x, y, w, h);
			break;
		case "square":
			shape = createShape(RECT, x, y, w, w);
			break;
		default:
			shape = regPolygon(x, y, w, (int) h);
		}
	}

	public PShape regPolygon(float x, float y, float radius, int npoints) {
		PShape poly = createShape();
		float angle = TWO_PI / npoints;
		beginShape();
		for (float a = 0; a < TWO_PI; a += angle) {
			float sx = x + cos(a) * radius;
			float sy = y + sin(a) * radius;
			poly.vertex(sx, sy);
		}
		endShape(CLOSE);
		return poly;
	}

	// public PShape custom2dShape() {
	//
	// }

	public void default3dShape(String str, Plane3d plane, int w, int h, int d) {
		// plane.getPlanePt()
		// translate(, y, z);
		switch (str) {
		case "cube":
			shape = createShape(BOX, w);
			break;
		case "box":
			shape = createShape(BOX, w, h, d);
			break;
		case "sphere":
			// w as radius
			shape = createShape(SPHERE, w);
			break;
		}
	}
	
//	public Shape loadShape() {
//		JSONObject json = loadJSONObject("plane.json");
//		JSONArray normArr = json.getJSONArray("norm");
//		double[] arr = new double[3];
//		for (int i = 0; i < 3; i++)
//			arr[i] = normArr.getDouble(i);
//		return new Shape(new Vec3d(arr), json.getDouble("dist"));
//	}
//
//	public void savePlane(String fileName, Plane3d plane) {
//		JSONObject json = new JSONObject();
//		Vec3d norm = plane.getNorm();
//		json.put("norm", norm.getArr());
//		json.put("dist", plane.getD());
//		saveJSONObject(json, "plane.json");
//	}

}
