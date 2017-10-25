import java.util.ArrayList;
import java.util.Random;

/**
 * Hold helper methods to perform the operations, such as calculate distance and
 * velocity, etc.
 * 
 * @author Zhiling
 *
 */

public class Computations {

	public static final double percent = 0.6;
	public static final double tol = 0.05; // max distance in meters to classify points as close to the plane

	// BodyFrame.FloorClipPlane.

//	/**
//	 * Calculates the distance between a point and a plane in 3D
//	 * 
//	 * @param plane
//	 * @param point
//	 * @return
//	 */
//	public static double pointPlaneDist(double[] plane, Vec3d point) {
//		// (a, b, c) components are a unit vector indicating the normal of the plane
//		double a = plane[0];
//		double b = plane[1];
//		double c = plane[2];
//
//		// w is the distance from the plane to the origin in meters
//		double w = plane[3];
//
//		double x = point.getX();
//		double y = point.getY();
//		double z = point.getZ();
//
//		// projecting w onto v is operated by formula below
//		return Math.abs(a * x + b * y + c * z + w) / Math.sqrt(a * a + b * b + c * c);
//
//	}

	/**
	 * Uses an iterative method called RANSAC to pick up a plane that fits as many
	 * as possible in a set of points in 3d
	 * 
	 * @param points
	 * @return
	 */
	public static double[] planeDetection(ArrayList<Vec3d> points) {
		if (points == null || points.size() == 0)
			return null;

		int size = points.size();

		int numFits = 0;
		int maxFits = numFits;
		int estimate = (int) (size * percent);

		// Pick several points at random.
		Random ran = new Random();

		Vec3d p1;
		Vec3d p2;
		Vec3d p3;
		double[] plane = new double[4];
		double[] tempPlane = new double[4];
		
		// if not enough points are on the plane
		while (numFits < estimate) {
			p1 = points.get(ran.nextInt(size));
			p2 = points.get(ran.nextInt(size));
			p3 = points.get(ran.nextInt(size));
			// make sure three points to form a plane are not collinear
			while (collinear3dPoints(p1, p2, p3))
				p3 = points.get(ran.nextInt(size));

			// Make a plane w/ three points given
			tempPlane = plotPlane(p1, p2, p3);

			// Check if each other point lies on the plane
			for (int i = 0; i < size; i++) {
				Vec3d tempPoint = points.get(i);
				if (pointPlaneDist(plane, tempPoint) <= tol)
					numFits++;
			}
			if (numFits > maxFits) {
				maxFits = numFits;
				plane = tempPlane;
			}
		}
		return plane;
	}

//	/**
//	 * Finds the projection of a point on a plane
//	 * @param plane - defined by norm 
//	 * @param planePoint - point on the plane
//	 * @param projPoint - point to be projected
//	 * @return
//	 */
//	public static Vec3d ptProjOnPlane(double[] plane, Vec3d planePt, Vec3d projPt) {
//		double[] norm = normalize(new double[] {plane[0], plane[1], plane[2]});
//		double resize = dot(subtract(projPt.inArr(), planePt.inArr()), norm);
//		double[] rslt = subtract(projPt.inArr(), resizeVector(norm, resize));
//		return new Vec3d(rslt);
//	}
	
	/**
	 * Checks if the point is on the plane
	 * In order for a point (x,y,z) to be in the plane, it must satisfy Ax+By+Cz+d=0".
	 * @param plane
	 * @param point
	 * @return
	 */
	public static boolean isPtOnPlane(double[] plane, Vec3d point) {
		if (plane[0] * point.getX()+ plane[1] * point.getY() + plane[2] * point.getZ() + plane[3] == 0)
			return true;
		if (pointPlaneDist(plane, point) <= tol)
			return true;
		return false;
	}
	
	/**
	 * L^2-Norm, i.e: the magnitude
	 * 
	 * @param vector
	 * @return
	 */
	public static double magnitude(double[] vector) {
		double mag = 0.0;

		for (int i = 0; i < vector.length; i++) {
			mag += Math.pow(vector[i], 2);
		}
		return Math.sqrt(mag);
	}

	/**
	 * To normalize a vector is to keep the pointing in the same direction, while
	 * change its length to 1 --> a unit vector. Calc: simply divide each component
	 * by its magnitude
	 * 
	 * @param vector
	 * @return
	 */
	public static double[] normalize(double[] vector) {
		double[] unitV = new double[vector.length];
		double mag = magnitude(vector);
		for (int i = 0; i < vector.length; i++)
			unitV[i] = vector[i] / mag;
		return unitV;
	}

	public static boolean collinear3dPoints(Vec3d p1, Vec3d p2, Vec3d p3) {
		double area = p1.getX() * (p2.getY() - p3.getY()) + p2.getX() * (p3.getY() - p1.getY())
				+ p3.getX() * (p1.getY() - p2.getY());
		return (area == 0);

	}

	public static boolean collinear3dVectors(double[] v1, double[] v2) {
		double[] vectorZero = { 0, 0, 0 };
		if (cross3(v1, v2) == vectorZero)
			return true;
		return false;
	}

	/**
	 * Plot a plane from three points given Equation 1: a(x-x0) + b(y-y0) + c(z-z0) = 0 
	 * Equation 2: ax + by + cz + d = 0 where {a, b, c} is the vector norm to
	 * the plane
	 * @param p1 - a point supposed to be on the plane
	 * @param p2
	 * @param p3
	 * @return
	 */
	public static double[] plotPlane(Vec3d p1, Vec3d p2, Vec3d p3) {
		double[] v1 = subtract(p2.inArr(), p1.inArr()); // a vector goes from p1 to p2
		double[] v2 = subtract(p3.inArr(), p1.inArr()); // a vector goes from p1 to p3
		double[] cp = cross3(v1, v2); // cross product of two vectors

		double[] neg_cp = { -cp[0], -cp[1], -cp[2] };
		double d = dot(neg_cp, p1.inArr());

		double[] plane = { cp[0], cp[1], cp[2], d };
		return plane;
	}

//	public static double vectorNorm(double[] vector) {
//		double rslt = 0.0;
//		for (int i = 0; i < vector.length; i++)
//			rslt += Math.pow(vector[i], 2);
//		return Math.sqrt(rslt);
//	}

//	/**
//	 * Returns dot product of two vectors in any number of dimensions
//	 * 
//	 * @param v1
//	 * @param v2
//	 * @return
//	 */
//	public static double dot(double[] v1, double[] v2) {
//		if (v1.length != v2.length)
//			throw new IllegalArgumentException("Not in the same dimension");
//		double rslt = 0.0;
//		for (int i = 0; i < v1.length; i++)
//			rslt += v1[i] * v2[i];
//		return rslt;
//	}

//	/**
//	 * Returns cross product between two vectors in 3D
//	 * @param v1
//	 * @param v2
//	 * @return
//	 */
//	public static double[] cross3(double[] v1, double[] v2) {
//		if (v1.length != v2.length)
//			throw new IllegalArgumentException("Not in the same dimension");
//		double[] rslt = { (v1[1] * v2[2] - v1[2] * v2[1]), // yz - zy
//				(v1[2] * v2[0] - v1[0] * v2[2]), // zx - xz
//				(v1[0] * v2[1] - v1[1] * v2[0]) }; // xy -yx
//		return rslt;
//	}

//	/**
//	 * Subtracts vector v2 from vector v1
//	 * @param v1
//	 * @param v2
//	 * @return
//	 */
//	public static double[] subtract(double[] v1, double[] v2) {
//		if (v1.length != v2.length)
//			throw new IllegalArgumentException("Not in the same dimension");
//		double[] rslt = new double[v1.length];
//		for (int i = 0; i < v1.length; i++)
//			rslt[i] = v1[i] - v2[i];
//		return rslt;
//	}

//	public static double[] resizeVector(double[] vector, double size)	{
//		for (int i = 0; i < vector.length; i++)
//			vector[i] *= size;
//		return vector;
//	}
	


}
