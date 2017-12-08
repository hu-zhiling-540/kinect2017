import java.util.ArrayList;
import java.util.Random;

import processing.data.JSONArray;
import processing.data.JSONObject;

public class Plane3d {

	private double a; // component of the unit norm vector
	private double b; // component of the unit norm vector
	private double c; // component of the unit norm vector
	private double d; // distance of the plane along its unit norm vector from the origin.

	private Vec3d norm; // normalized norm vector to the plane
	private Point3d planePt; // point on the plane
	// private double[] plane;

	// private final int len = 4; // length of the vector

	public Plane3d() {
		this(new Vec3d(0, 0, 1), 0);
	}

	/**
	 * Creates a plane with a norm vector and d
	 * 
	 * @param norm
	 * @param d
	 */
	public Plane3d(Vec3d norm, double d) {
		setNorm(norm);
		this.d = d;
		setPlanePt();
	}

	public Plane3d(Vec3d norm, Point3d planePt) {
		setNorm(norm);
		this.planePt = planePt;
	}

	/**
	 * Creates a plane from a set of three vertex positions
	 * 
	 * @param p1
	 *            a point on the plane
	 * @param p2
	 * @param p3
	 */
	public Plane3d(Point3d p1, Point3d p2, Point3d p3) {
		// Make two vectors originating from p1
		this(p2.subtract(p1).cross(p3.subtract(p1)), p1);
	}

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
	public Plane3d(double a, double b, double c, double d) {
		setNorm(new Vec3d(a, b, c));
		this.d = d;
	}

	public void setNorm(Vec3d norm) {
		this.norm = norm.normalize();
		this.a = this.norm.getX();
		this.b = this.norm.getY();
		this.c = this.norm.getZ();

	}

	public Vec3d getNorm() {
		return this.norm;
	}

	/**
	 * Returns signed distance between a point and a plane by inserting point into
	 * the plane equation. A negative value means the point lies below the plane.
	 * 
	 * @param pt
	 * @return
 */
	public double signedPtDist(Point3d pt) {
		return pt.subtract(planePt).dot(norm);
	}

	/**
	 * Returns true if point is on the plane by checking the distance between a
	 * point and this plane is 0/ within tolerance.
	 * 
	 * @param pt
	 * @param tol
	 * @return
	 */
	public boolean hasPoint(Point3d pt, double tol) {
		return signedPtDist(pt) <= tol || signedPtDist(pt) >= -1f * tol;
	}

	/**
	 * Finds the projection of a point onto a plane
	 * 
	 * @param projPt
	 *            point to be projected
	 * @return
	 */
	public Point3d ptProjOnPlane(Point3d projPt) {
		double dist = signedPtDist(projPt);
		return projPt.subtractP(this.norm.scale(dist));
	}

	/**
	 * Find the projection of a vector onto a plane
	 * 
	 * @param projV
	 *            vector to be projected
	 * @return
	 */
	public Vec3d vecProjOnPlane(Vec3d projV) {
		return projV.subtract(projV.projOn(norm));
	}

	/**
	 * Uses an iterative method called RANSAC to pick up a plane that fits as many
	 * as possible in a set of points in 3d
	 * 
	 * @param points
	 * @return
	 */
	public static Plane3d ransacPlane(ArrayList<Point3d> points, double tol, double percent) {
		if (points == null || points.size() == 0)
			return null;

		int size = points.size();

		int numFits = 0;
		int maxFits = numFits;
		int estimate = (int) (size * percent);

		// Pick several points at random.
		Random ran = new Random();

		Point3d p1 = new Point3d();
		Point3d p2 = new Point3d();
		Point3d p3 = new Point3d();

		// double[] plane = new double[4];
		// double[] tempPlane = new double[4];

		Plane3d plane = new Plane3d();
		Plane3d tempPlane = new Plane3d();

		// if not enough points are on the plane
		while (numFits < estimate) {
			p1 = points.get(ran.nextInt(size));
			p2 = points.get(ran.nextInt(size));
			p3 = points.get(ran.nextInt(size));
			// make sure three points to form a plane are not collinear
			while (p1.collinear(p2, p3))
				p3 = points.get(ran.nextInt(size));

			// Make a plane w/ three points given
			tempPlane = new Plane3d(p1, p2, p3);

			// Check if each other point lies on the plane
			for (int i = 0; i < size; i++) {
				Point3d tempPoint = points.get(i);
				if (plane.signedPtDist(tempPoint) <= tol)
					numFits++;
			}
			if (numFits > maxFits) {
				maxFits = numFits;
				plane = tempPlane;
			}
		}
		return plane;
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
		this.planePt = new Point3d(0, 0, z);
	}

	public JSONObject serialize() {
		JSONObject json = new JSONObject();
		Vec3d norm = this.getNorm();
		json.put("norm", norm.getArr());
		json.put("dist", this.getD());
		return json;
	}

	/**
	 * Transforms a normalized plane by a quaternion rotation.
	 * 
	 * @param quat
	 */
	public void transform(Quat quat) {

	}

	public String toString() {
		String str = this.a + " x" + " + " + this.b + " y " + " + " + this.c + " z " + " = " + this.d;
		return str;
	}

}
