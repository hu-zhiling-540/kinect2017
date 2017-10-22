
public class Plane {

	double a;
	double b;
	double c;
	double d;
	
	double[] n;	// normal vector of the plane
	
//	n = a normal vector
//	(x0, y0, z0) point on the plane
			
	/**
	 * Creates a plane with the specified values.
	 * a normal vector for the plane
	 * @param a
	 * @param b
	 * @param c
	 * @param d
	 */
	public Plane(double a, double b, double c, double d)	{
		this.a = a;
		this.b = b;
		this.c = c;
		this.d = d;
	}
	
	/**
	 * Creates a plane with a vector of three components and d
	 * @param norm
	 * @param d
	 */
	public Plane(double[] norm, double d)	{
		
	}
	
	/**
	 * Creates a plane from a set of three vertex positions, which must all be different and not in a straight line.
	 * @param p1
	 * @param p2
	 * @param p3
	 */
	public void plotPlaneFromPts(Point3D p1, Point3D p2, Point3D p3)	{
		
	}
	
	/**
	 * Changes the coefficients of the normal vector of a plane to make it of unit length
	 */
	public void normalize()	{
		
	}
	
	/**
	 * Transforms a normalized plane by a quaternion rotation.
	 * @param quat
	 */
	public void transform(Quat quat)	{
		
	}
	
	/**
	 * Calculates the dot product of a plane with a vector.
	 * @param vector
	 * @return
	 */
	public double dot(double[] vector)	{
		
	}
	
}
