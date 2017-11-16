import java.util.ArrayList;

import processing.core.PApplet;
import processing.core.PShape;
import processing.core.PVector;
import processing.data.JSONArray;
import processing.data.JSONObject;

/**
 * Default 2d shape is defined by a circular sequence of points; Plus extrusion
 * represented by a 3d vector is a 3d shape. Connects the points without
 * crossings.
 * 
 * @author Zhiling
 *
 */
public class Shape3d {

	long shapeID;

	Plane3d onPlane;
	PShape curr;

	boolean isClosed;
	ArrayList<Point3d> tempPts; // points outlining the shape
	ArrayList<Point3d> planarPts; // points outlining the shape

	Vec3d extrusion;

	double xMin = Double.NEGATIVE_INFINITY;
	double xMax = Double.POSITIVE_INFINITY;
	double yMin = Double.NEGATIVE_INFINITY;
	double yMax = Double.POSITIVE_INFINITY;
	double zMin = Double.NEGATIVE_INFINITY;
	double zMax = Double.POSITIVE_INFINITY;

	public Shape3d(Plane3d plane, ArrayList<Point3d> points, Vec3d extrusion) {
		planarPts = new ArrayList<Point3d>();
		for (int i = 0; i < points.size(); i++)
			planarPts.add(plane.ptProjOnPlane(points.get(i)));
		this.extrusion = extrusion;
	}

	/**
	 * Roughly checks if the point is within the min and max bounds in different
	 * divisions
	 * 
	 * @param pt
	 * @return
	 */
	public boolean offFixedBounds(Point3d pt) {
		if (!isClosed)
			throw new RuntimeException("Shape is not closed yet.");
		if (pt.getX() > xMax || pt.getX() < xMin || pt.getY() > yMax || pt.getY() < yMin)
			return false;
		// is a 3d shape
		if (!is2dShape()) {
			if (pt.getZ() > zMax || pt.getZ() < zMin)
				return false;
		}
		return true;
	}

	public void validate() {
		// it takes at least three points to form a polygon/shape
		if (tempPts.size() < 3)
			return;
		// check if a list of points are collinear

		isClosed = true;
	}

	public void addVertex(Point3d pt) {
		if (isClosed)
			throw new RuntimeException("Shape is closed.");
		if (tempPts.size() == 0) {
			xMin = pt.getX();
			xMax = pt.getX();
			yMin = pt.getY();
			yMax = pt.getY();
			// might only be updated when extrusion gets changed
			// zMin = pt.getZ();
			// zMax = pt.getZ();
		} else {
			if (pt.getX() > xMax)
				xMax = pt.getX();
			if (pt.getX() < xMin)
				xMin = pt.getX();
			if (pt.getY() > yMax)
				yMax = pt.getY();
			if (pt.getY() < yMin)
				yMin = pt.getY();
		}
		tempPts.add(pt);

	}

	public Shape3d buildShape() {
		validate();
		return this;
	}

	/**
	 * Returns true if this shape contains the point. Algorithm: Draw a horizontal
	 * line to the right of each point and extend it to infinity Count the number of
	 * times a line intersects the polygon. even number ⇒ point is outside; odd
	 * number ⇒ point is inside
	 * 
	 * @param pt
	 * @return
	 */
	public boolean isInside2d(Point3d pt) {
		if (offFixedBounds(pt))
			return false;
		int xings = 0;
		for (int i = 0; i < planarPts.size(); i++) {
		}

		if (xings % 2 == 1)
			return true;
		return false;

	}

	public boolean is2dShape() {
		return extrusion == null;
	}

	public void setExtr(Vec3d extrusion) {
		this.extrusion = extrusion;
	}

	public Vec3d getExtr() {
		return this.extrusion;
	}
}