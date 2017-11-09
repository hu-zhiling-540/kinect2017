import processing.core.PApplet;
import processing.core.PShape;
import processing.core.PVector;
import processing.data.JSONArray;
import processing.data.JSONObject;

/**
 * @author Zhiling
 *
 */
public class Shape extends PApplet {

	long shapeID;

	Plane3d onPlane;
	PShape curr;

	boolean closed;

	//
	// public static final String CIRCLE = "CIRCLE";
	// public static final String RECTANGLE = "RECT";
	// public static final String SQUARE = "SQUA";

	// ArrayList<Vec3d> points; // points representing the polygon

	public Shape(String cmd, Plane3d plane, Point3d pt, float w, float h) {
		curr = createShape();
		onPlane = plane;
		default2dShape(cmd, (float) pt.getX(), (float) pt.getY(), w, h);
	}

	public void default2dShape(String cmd, float x, float y, float w, float h) {
		switch (cmd) {
		case "rectangle":
			curr = createShape(RECT, x, y, w, h);
			break;
		case "circle":
			curr = createShape(ELLIPSE, x, y, w, h);
			break;
		case "triangle":
			curr = createShape(ELLIPSE, x, y, w, h);
			break;
		case "square":
			curr = createShape(RECT, x, y, w, w);
			break;
		default:
			curr = regPolygon(x, y, w, (int) h);
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
			curr = createShape(BOX, w);
			break;
		case "box":
			curr = createShape(BOX, w, h, d);
			break;
		case "sphere":
			// w as radius
			curr = createShape(SPHERE, w);
			break;
		}
	}

	public PShape loadSavedShape(String fileName) {
		JSONObject json = loadJSONObject(fileName);

		JSONObject plane = json.getJSONObject("plane");

		JSONArray vertices = json.getJSONArray("vertices");
		int size = vertices.size();

		PShape sp = createShape();
		for (int i = 0; i < size; i++) {
			JSONObject obj = vertices.getJSONObject(i);
			sp.setVertex(i, deserializePVec(obj));
		}
		// for drawing
		// shape(sp);
		return sp;
	}

	public void saveShape(String fileName) {
		JSONObject json = new JSONObject();

		JSONObject plane = onPlane.serialize();
		json.put("plane", plane);

		// translate(20, 20);
		int size = curr.getVertexCount();
		JSONArray vertices = new JSONArray();
		for (int i = 0; i < size; i++) {
			PVector v = curr.getVertex(i);
			vertices.setJSONObject(i, serializePVec(v));
		}
		json.put("vertices", vertices);
		saveJSONObject(json, "shape.json");
	}

	public JSONObject serializePVec(PVector pv) {
		JSONObject obj = new JSONObject();
		obj.setFloat("x", pv.x);
		obj.setFloat("y", pv.y);
		obj.setFloat("z", pv.z);
		return obj;
	}

	public PVector deserializePVec(JSONObject obj) {
		float x = obj.getFloat("x");
		float y = obj.getFloat("y");
		float z = obj.getFloat("z");
		return new PVector(x, y, z);
	}
}