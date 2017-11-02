import java.util.Arrays;

public class Point3d extends Vec3d {

	private double x;
	private double y;
	private double z;
	private double[] arr = new double[3]; // array of vector's components

	private final int len = 3; // length of the vector

	public Point3d() {
		this(0, 0, 0);
	}

	public Point3d(double x, double y, double z) {
		this.x = x;
		this.y = y;
		this.z = z;
		this.arr = new double[] { x, y, z };
	}

	public Point3d(double[] arr) {
		super(arr);
	}

	/**
	 * Returns a vector from this point to that point
	 * 
	 * @param that
	 *            endpoint
	 * @return
	 */
	public Vec3d subtract(Point3d that) {
		double[] rslt = new double[3];
		for (int i = 0; i < 3; i++)
			rslt[i] = this.arr[i] - that.arr[i];
		return new Vec3d(rslt);
	}

	@Override
	public String toString() {
		StringBuilder sb = new StringBuilder();
		sb.append("(");
		sb.append(this.x).append(", ");
		sb.append(this.y).append(", ");
		sb.append(this.z);
		sb.append(")");
		return sb.toString();
	}

	/**
	 * Returns the Euclidean distance between this point and that point
	 * 
	 * @param that
	 * @return
	 */
	public double distanceTo(Point3d that) {
		return Math.abs(this.subtract(that).magnitude());
	}

	/**
	 * Returns true if two three points are collinear by checking two vectors
	 * between points are collinear
	 * 
	 * @param p1
	 * @param p2
	 * @return
	 */
	public boolean collinear(Point3d p1, Point3d p2) {
		Vec3d v1 = p1.subtract(this);// a vector from this to p1
		Vec3d v2 = p2.subtract(this);// a vector from this to p2
		return v1.collinear(v2);
	}

	/**
	 * @return the arr
	 */
	public double[] getArr() {
		return arr;
	}
	// public static boolean collinear3dPoints(Vec3d p1, Vec3d p2, Vec3d p3) {
	// double area = p1.getX() * (p2.getY() - p3.getY()) + p2.getX() * (p3.getY() -
	// p1.getY())
	// + p3.getX() * (p1.getY() - p2.getY());
	// return (area == 0);
	//
	// }
}
