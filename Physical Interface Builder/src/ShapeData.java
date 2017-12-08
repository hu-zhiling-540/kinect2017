import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.util.ArrayList;

import processing.core.PApplet;
import processing.core.PShape;
import processing.core.PVector;
import processing.data.JSONArray;
import processing.data.JSONObject;

public class ShapeData extends PApplet {

	public JSONObject serializePVec(PVector pv) {
		JSONObject obj = new JSONObject();
		obj.setFloat("x", pv.x);
		obj.setFloat("y", pv.y);
		obj.setFloat("z", pv.z);
		return obj;
	}

	public Vec3d deserializePVec(JSONObject obj) {
		float x = obj.getFloat("x");
		float y = obj.getFloat("y");
		float z = obj.getFloat("z");
		return new Vec3d(x, y, z);
	}

	public static void saveShape(String fileName, Shape3d s) {
		boolean exists = new File(fileName).exists();
		FileOutputStream fos;
		try {
			fos = new FileOutputStream(fileName, true);
			ObjectOutputStream oos = exists ? new ObjectOutputStream(fos) {
				protected void writeStreamHeader() throws IOException {
					reset();
				}
			} : new ObjectOutputStream(fos);
			oos.writeObject(s.planarPts);
			oos.writeObject(s.onPlane);
			oos.close();
			fos.close();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

	}

	// public void loadShape(String fileName) {
	// try {
	// FileInputStream fileIn = new FileInputStream(fileName);
	// ObjectInputStream in = new ObjectInputStream(fileIn);
	// System.out.println("Deserialized Data: \n" + in.readObject().toString());
	// in.close();
	// fileIn.close();
	// } catch (FileNotFoundException e) {
	// e.printStackTrace();
	// } catch (IOException e) {
	// e.printStackTrace();
	// }
	// }

	public Shape3d loadSavedShape(String fileName) {
		JSONObject json = loadJSONObject(fileName);
		JSONObject plane = json.getJSONObject("plane");
		JSONArray vertices = json.getJSONArray("vertices");

		int size = vertices.size();

		Shape3d sp = new Shape3d();
		ArrayList<Vec3d> pts = new ArrayList<Vec3d>();
		for (int i = 0; i < size; i++) {
			JSONObject obj = vertices.getJSONObject(i);
			pts.add(deserializePVec(obj));
		}
		return sp;
	}

	public void saveShape(String fileName, Plane3d onPlane, ArrayList<Shape3d> shapes) {
		// JSONObject json = new JSONObject();
		//
		// JSONObject plane = onPlane.serialize();
		// json.put("plane", plane);
		//
		// // translate(20, 20);
		// int size = curr.getVertexCount();
		// JSONArray vertices = new JSONArray();
		// for (int i = 0; i < size; i++) {
		// PVector v = curr.getVertex(i);
		// vertices.setJSONObject(i, serializePVec(v));
		// }
		// json.put("vertices", vertices);
		// saveJSONObject(json, "shape.json");
	}

}
