
public class Point3d extends Vec3d {

	private double x;
	private double y;
	private double z;
	private double[] arr; // array of vector's components

	private final int len = 3; // length of the vector

	public Point3d() {
		super(0, 0, 0);
	}

	public Point3d(double x, double y, double z) {
		super(x, y, z);
	}

	public Point3d(double[] arr) {
		super(arr);
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
		return this.subtract(that).magnitude();
	}
}
