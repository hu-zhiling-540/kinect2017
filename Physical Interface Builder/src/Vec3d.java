
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
	private double[] arr; // array of vector's components

	private final int len = 3; // length of the vector

	public static final Vec3d ZERO_V3 = new Vec3d(0, 0, 0);
	public static final Vec3d ONE_V3 = new Vec3d(1, 1, 1);

	/**
	 * Creates the zero vector in 3d
	 */
	public Vec3d() {
		this.arr = new double[3];
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
		arr = new double[] { x, y, z };
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
	 * return the Euclidean distance between this and that
	 * 
	 * @param that
	 * @return
	 */
	public double distanceTo(Vec3d that) {
		return this.subtract(that).magnitude();
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
	 * Normalizes this vector to a unit length of 1
	 * 
	 * @return
	 */
	public Vec3d normalize() {
		return this.scale(1.0 / magnitude());
	}

	/**
	 * Returns the slope of this vector in (x, y)
	 * 
	 * @return
	 */
	public double slope() {
		return (double) arr[1] / arr[0];
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
	 * Returns cross product between two vectors in 3d
	 * 
	 * @param that
	 * @return
	 */
	public double[] cross(double[] that) {
		if (arr.length != that.length)
			throw new IllegalArgumentException("Not in the same dimension");
		double[] rslt = { (arr[1] * that[2] - arr[2] * that[1]), // yz - zy
				(arr[2] * that[0] - arr[0] * that[2]), // zx - xz
				(arr[0] * that[1] - arr[1] * that[0]) }; // xy -yx
		return rslt;
	}

	public double vectorNorm() {
		double rslt = 0.0;
		for (int i = 0; i < arr.length; i++)
			rslt += Math.pow(arr[i], 2);
		return Math.sqrt(rslt);
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

}
