import java.util.ArrayList;
import java.util.Random;

/**
 * Hold helper methods to perform the operations,
 * such as calculate distance and velocity, etc.
 * @author Zhiling
 *
 */

public class Computations {
	
	public static double diffX(Vertex j1, Vertex j2) {
		return j1.getX() - j2.getX();
	}
	
	public static double diffY(Vertex j1, Vertex j2) {
		return j1.getY() - j2.getY();
	}
	
	public static double diffZ(Vertex j1, Vertex j2) {
		return j1.getZ() - j2.getZ();
	}
	
	// linear distance between vertices in 3d space
	public static double trajDist(Vertex j1, Vertex j2)	{
		double x = diffX(j1, j2);
		double y = diffY(j1, j2);
		double z = diffZ(j1, j2);
		return Math.sqrt(Math.pow(x, 2) + Math.pow(y, 2) + Math.pow(z, 2));
	}
	
//	public static 
//	
//	public static double Tilt()	{
//		 // arctangent help find the angle given the ratio.
//	}
	
	// a plane represented by a quaternion
	// point (x, y, z)
	// should return the distancefrom the point to the plane
	
	// BodyFrame.FloorClipPlane. 
//	The (x,y,z) components are a unit vector indicating the normal of the plane, 
//	and w is the distance from the plane to the origin in meters.
	public static double pointPlaneDist(Quat plane, Vertex point)	{
		// normal vector(i.e. perpendicular) to the plane is given by [a, b, c]
		double a = plane.getX();
		double b = plane.getY();
		double c = plane.getZ();
		
		// a vector from the plane to the point is given by w
		double w = plane.getW();
		
		double x = point.getX();
		double y = point.getY();
		double z = point.getZ();
		
		// projecting w onto v is operated by formula below
		return Math.abs(a*x + b*y + c*z + w)/Math.sqrt(a*a + b*b + c*c);
				
	}
	
	public static double[] planeDetection(ArrayList<Vertex> points)	{
		if (points == null || points.size() == 0)
			return null;
		
		double[] plane = new double[4];
		int size = points.size();
		int maxFits = 0;
		
		// Pick several points at random.
		Random ran = new Random();
		Vertex v1 = points.get(ran.nextInt(size));
		Vertex v2 = points.get(ran.nextInt(size));
		Vertex v3 = points.get(ran.nextInt(size));
//		Make a plane.
//		Check if each other point lies on the plane.
//		If enough are on the plane - recalculate a best plane from all these points and remove them from the set
//		If not try another 3 points
//		Stop when you have enough planes, or too few points left.
	
	}
	
	// plot a plane from three vertices
	// ax + by + cz = d
	public static double[] plotPlane(Vertex v1, Vertex v2, Vertex v3)	{
		
	}
	
	public static double vectorNorm(double[] vector)	{
		double rslt = 0.0;
		for (int i = 0; i < vector.length; i++)
			rslt += Math.pow(vector[i], 2);
		return Math.sqrt(rslt);
	}
	
	public static double dot(double[] v1, double[] v2) {
		if (v1.length != v2.length)
			throw new IllegalArgumentException("Not in the same dimension");
		double rslt = 0.0;
		for (int i = 0; i < v1.length; i++)
			rslt += v1[i] * v2[i];
		return rslt;
	}
	
	public static double[] subtract(double[] v1, double[] v2) {
		if (v1.length != v2.length)
			throw new IllegalArgumentException("Not in the same dimension");
		double[] rslt = new double[v1.length];
		for (int i = 0; i < v1.length; i++)
			rslt[i] = v1[i] - v2[i];
		return rslt;
	}
	
	
}
