

public class Point3D {

	static double posX;
	static double posY;
	static double posZ;
	
	
	
	public Point3D(double x, double y, double z)	{
		posX = x;
		posY = y;
		posZ = z;
	}
	
	public Point3D(double[] arr)	{
		posX = arr[0];
		posY = arr[1];
		posZ = arr[2];
	}
	
	public double getX()	{
		return posX;
	}
	
	public double getY()	{
		return posY;
	}
	public double getZ()	{
		return posZ;
	}
	
	public double[] inArr() {
		double[] arr = {posX, posY, posZ};
		return arr;
	}
	
	
	// linear distance between vertices in 3d space
	public double trajDist(Point3D p) {
		return Math.sqrt(Math.pow(diffX(p), 2) + Math.pow(diffY(p), 2) + Math.pow(diffZ(p), 2));
	}
	
	public double diffX(Point3D p) {
		return posX - p.getX();
	}

	public double diffY(Point3D p) {
		return posY - p.getY();
	}
	
	public double diffZ(Point3D p) {
		return posZ - p.getZ();
	}
	
}
