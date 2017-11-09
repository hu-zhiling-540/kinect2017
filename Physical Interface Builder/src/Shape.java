import java.util.ArrayList;

/**
 * 
 */

/**
 * @author Zhiling
 *
 */
public abstract class Shape {

	long shapeID;

	Plane3d onPlane;
	boolean closed;
	ArrayList<Vec3d> points; // points representing the polygon

	public Shape() {

	}

	public abstract void transform();
}
