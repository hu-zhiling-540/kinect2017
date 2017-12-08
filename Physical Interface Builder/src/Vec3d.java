import java.util.Arrays;

/**
 * This class is modified and extended version of Vector.java linked below:
 * https://introcs.cs.princeton.edu/java/33design/Vector.java.html
 * 
 * @author Zhiling
 *
 */
public class Vec3d {

	private double x;
	private double y;
	private double z;
	protected double[] arr = new double[3]; // array of vector's components

	// private final int len = 3; // length of the vector

	public static final Vec3d ZERO_V3 = new Vec3d(0, 0, 0);
	public static final Vec3d ONE_V3 = new Vec3d(1, 1, 1);

	/**
	 * Creates the zero vector in 3d
	 */
	public Vec3d() {
		this(0, 0, 0);
	}

	/**
	 * Creates the vector with specified x, y, z values
	 * 
	 * @param x
	 * @param y
	 * @param z
	 */
	public Vec3d(double x, double y, double z) {
		this.x = x;
		this.y = y;
		this.z = z;
		this.arr = new double[] { x, y, z };
	}

	/**
	 * Creates a vector from an array
	 * 
	 * @param arr
	 */
	public Vec3d(double[] arr) {
		this.arr = arr;
		this.x = arr[0];
		this.y = arr[1];
		this.z = arr[2];
	}

	/**
	 * Adds that vector to this vector
	 * 
	 * @param that
	 * @return
	 */
	public Vec3d add(Vec3d that) {
		double[] rslt = new double[3];
		for (int i = 0; i < 3; i++)
			rslt[i] = this.arr[i] + that.arr[i];
		return new Vec3d(rslt);
	}

	/**
	 * Subtracts that vector from this vector
	 * 
	 * @param that
	 * @return
	 */
	public Vec3d subtract(Vec3d that) {
		double[] rslt = new double[3];
		for (int i = 0; i < 3; i++)
			rslt[i] = this.arr[i] - that.arr[i];
		return new Vec3d(rslt);
	}

	public Vec3d scale(double size) {
		double[] rslt = new double[3];
		for (int i = 0; i < 3; i++)
			rslt[i] = this.arr[i] * size;
		return new Vec3d(rslt);
	}

	/**
	 * Returns the dot product of this vector and that vector
	 * 
	 * @param that
	 * @return
	 */
	public double dot(Vec3d that) {
		double rslt = 0.0;
		for (int i = 0; i < 3; i++)
			rslt += this.arr[i] * that.arr[i];
		return rslt;
	}

	/**
	 * Returns the cross product of this vector and that vector
	 * 
	 * @param that
	 * @return
	 */
	public Vec3d cross(Vec3d that) {
		double[] rslt = { (this.arr[1] * that.arr[2] - this.arr[2] * that.arr[1]), // yz - zy
				(this.arr[2] * that.arr[0] - this.arr[0] * that.arr[2]), // zx - xz
				(this.arr[0] * that.arr[1] - this.arr[1] * that.arr[0]) }; // xy -yx
		return new Vec3d(rslt);
	}

	public Boolean isPerpendicular(Vec3d that) {
		return this.dot(that) == 0;
	}

	/**
	 * Returns the Euclidean norm of this vector
	 * 
	 * @return
	 */
	public double magnitude() {
		return Math.sqrt(this.dot(this));
	}

	/**
	 * Normalizes this vector to a unit length of 1
	 * 
	 * @return
	 */
	public Vec3d normalize() {
		return this.scale(1.0 / magnitude());
	}

	public Vec3d projOn(Vec3d that) {
		if (this.isPerpendicular(that))
			return ZERO_V3;
		double dot = this.dot(that);
		dot /= Math.pow(that.magnitude(), 2);
		return that.scale(dot);
	}

	/**
	 * Reflects the point at the origin without changing the original point.
	 * 
	 * @return the resulting reflected point
	 */
	public Point3d reflectOrigin() {
		return new Point3d(-1 * this.x, -1 * this.y, -1 * this.z);
	}

	/**
	 * Checks if cross product of two vectors is zero
	 * 
	 * @param that
	 * @return
	 */
	public boolean collinear(Vec3d that) {
		return this.cross(that).sameValue(ZERO_V3);
	}

	/**
	 * @return the x
	 */
	public double getX() {
		return x;
	}

	/**
	 * @param x
	 *            the x to set
	 */
	public void setX(double x) {
		this.x = x;
	}

	/**
	 * @return the y
	 */
	public double getY() {
		return y;
	}

	/**
	 * @param y
	 *            the y to set
	 */
	public void setY(double y) {
		this.y = y;
	}

	/**
	 * @return the z
	 */
	public double getZ() {
		return z;
	}

	/**
	 * @param z
	 *            the z to set
	 */
	public void setZ(double z) {
		this.z = z;
	}

	/**
	 * @return the arr
	 */
	public double[] getArr() {
		return arr;
	}

	/**
	 * @param arr
	 *            the arr to set
	 */
	public void setArr(double[] arr) {
		this.arr = arr;
	}

	/**
	 * Checks if this and that vectors hold same value
	 * 
	 * @param that
	 * @return
	 */
	public boolean sameValue(Vec3d that) {
		// System.out.println(Arrays.toString(this.arr));
		// System.out.println(Arrays.toString(that.arr));
		return Arrays.equals(this.arr, that.arr);
	}

	public String toString() {
		StringBuilder sb = new StringBuilder();
		sb.append("[");
		sb.append(this.x).append(", ");
		sb.append(this.y).append(", ");
		sb.append(this.z);
		sb.append("]");
		return sb.toString();
	}

}
