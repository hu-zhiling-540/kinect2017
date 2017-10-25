
public class Plane {

	private double a; // component of the norm vector
	private double b; // component of the norm vector
	private double c; // component of the norm vector
	private double d; // distance from the origin

	private Vec3d norm; // normalized norm vector to the plane
	private Vec3d planePt; // point on the plane
	// private double[] plane;

	// private final int len = 4; // length of the vector

	/**
	 * Creates a plane with the specified values. a normal vector for the plane
	 * 
	 * @param a
	 * @param b
	 * @param c
	 * @param d
	 */
	public Plane(double a, double b, double c, double d) {
		setNorm(new Vec3d(a, b, c));
		this.d = d;
	}

	/**
	 * Creates a plane with a norm vector and d
	 * 
	 * @param norm
	 * @param d
	 */
	public Plane(Vec3d norm, double d) {
		setNorm(norm);
		this.d = d;
	}

	public Plane(Vec3d norm, Vec3d planePt) {
		setNorm(norm);
		this.planePt = planePt;
	}

	public void setNorm(Vec3d norm) {
		this.norm = norm.normalize();
		this.a = this.norm.getX();
		this.b = this.norm.getY();
		this.c = this.norm.getZ();

	}

	/**
	 * @return the d
	 */
	public double getD() {
		return d;
	}

	/**
	 * Sets d based on norm and planePt
	 */
	public void setD() {
		this.d = this.norm.dot(planePt);
	}

	/**
	 * @return the planePt
	 */
	public Vec3d getPlanePt() {
		return planePt;
	}

	/**
	 * Sets planePt based on norm and d
	 */
	public void setPlanePt() {
		double z = (double) d / norm.getZ();
		this.planePt = new Vec3d(0, 0, z);
	}

	/**
	 * Returns true if point is on the plane by checking the distance between a
	 * point and this plane is 0.
	 * 
	 * @param pt
	 * @return
	 */
	public boolean hasPoint(Vec3d pt) {
		return signedPtDist(pt) == 0;
	}

	/**
	 * Returns signed distance between a point and a plane by inserting point into
	 * the plane equation. A negative value means the point lies below the plane.
	 * 
	 * @param pt
	 * @return
	 */
	public double signedPtDist(Vec3d pt) {
		return this.norm.dot(pt) - this.d;
	}

	/**
	 * Finds the projection of a point on a plane
	 * 
	 * @param projPt
	 *            point to be projected
	 * @return
	 */
	public Vec3d ptProjOnPlane(Vec3d projPt) {
		double dot = projPt.subtract(this.planePt).dot(this.norm);
		return projPt.subtract(this.norm.scale(dot));
	}

	/**
	 * Creates a plane from a set of three vertex positions, which must all be
	 * different and not in a straight line.
	 * 
	 * @param p1
	 * @param p2
	 * @param p3
	 */
	public void plotPlaneFromPts(Vec3d p1, Vec3d p2, Vec3d p3) {

	}

	/**
	 * Transforms a normalized plane by a quaternion rotation.
	 * 
	 * @param quat
	 */
	public void transform(Quat quat) {

	}

	// /**
	// * Calculates the dot product of a plane with a vector.
	// *
	// * @param vector
	// * @return
	// */
	// public double dot(double[] vector) {
	//
	// }

}
