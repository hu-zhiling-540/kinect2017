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

	class LineSegment {
		Point3d start;
		Point3d end;
		// 2d mode
		double k; // slope
		double b; // intercept

		LineSegment(Point3d start, Point3d end) {
			if (start == null)
				throw new NullPointerException("Start point is null.");
			if (end == null)
				throw new NullPointerException("End point is null.");
			this.start = start;
			this.end = end;
		}

		// (y2-y1)/(x2-x1)
		double slope() {
			if (isVertical())
				throw new IllegalArgumentException("Vertical line has an undefined slope.");
			return (end.getY() - start.getY()) / (end.getX() - start.getX());
		}

		// y1 - k*x1
		double intercept() {
			return start.getY() - slope() * start.getX();
		}

		boolean isVertical() {
			return start.getX() == end.getX();
		}

		boolean isParallel(LineSegment that) {
			return this.slope() == that.slope();
		}

		// y = mx + b

		// the angle is the arctangent2 of (y2 - y1, x2 - x1)
		double angle() {
			return Math.atan2(end.getY() - start.getY(), end.getX() - start.getX()) * 180.0 / Math.PI;
		}

		// line segment connecting points (x1, y1) and (x2, y2) is the square root of
		// (x2 - x1)2 + (y2 - y1)2
		double length() {
			return Math.sqrt(Math.pow(end.getX() - start.getX(), 2) + Math.pow(end.getY() - start.getY(), 2));
		}

		/**
		 * Checks if point lies on the line segment
		 * 
		 * @param pt
		 * @return
		 */
		boolean containsPt(Point3d pt) {
			if (pt.getX() <= Math.max(start.getX(), end.getX()) && pt.getX() >= Math.min(start.getX(), end.getX())
					&& pt.getY() <= Math.max(start.getY(), end.getY())
					&& pt.getY() >= Math.min(start.getY(), end.getY()))
				return true;
			return false;
		}

		boolean crossLine(LineSegment that) {
			Point3d xingPt;
			if (this.isVertical() && that.isVertical()) {
				if (this.start.getX() != that.start.getX())
					return false;
				// overlap
				// if (this.start.getY() == that.start.getY() || this.start.getY() ==
				// that.end.getY())
			}
			// if only one line segment is vertical
			if (this.isVertical() && !that.isVertical()) {
				double x = this.start.getX();
				double y = that.slope() * x + that.intercept();
				xingPt = new Point3d(x, y);
			} else if (that.isVertical() && !this.isVertical()) {
				double x = that.start.getX();
				double y = this.slope() * x + this.intercept();
				xingPt = new Point3d(x, y);
			}
			// neither is vertical
			else {
				if (this.isParallel(that))
					return false;
				// x = (b2 - b2) / (k1 - k2)
				double x = (that.intercept() - this.intercept()) / (this.slope() - that.slope());
				double y = that.slope() * x + that.intercept();
				xingPt = new Point3d(x, y);
			}
			// check if this point is within both segments
			if (this.containsPt(xingPt) && that.containsPt(xingPt))
				return true;
			return false;

		}
	}

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

	public Shape3d(Plane3d plane, ArrayList<Point3d> points) {
		planarPts = new ArrayList<Point3d>();
		for (int i = 0; i < points.size(); i++)
			planarPts.add(plane.ptProjOnPlane(points.get(i)));
		this.extrusion = null;
	}

	public Shape3d(Plane3d plane, ArrayList<Point3d> points, Vec3d extrusion) {
		planarPts = new ArrayList<Point3d>();
		for (int i = 0; i < points.size(); i++)
			planarPts.add(plane.ptProjOnPlane(points.get(i)));
		this.extrusion = extrusion;
	}

	public void addVertex(Point3d pt) {
		if (isClosed)
			throw new RuntimeException("Shape is closed.");
		// shape is new
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
		Plane3d plane;
		// Shape3d sp = this(plane, tempPts);
		Shape3d sp = null;
		return sp;
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

	public void validate() {
		// it takes at least three points to form a polygon/shape
		if (tempPts.size() < 3)
			return;
		// check if a list of points are collinear

		isClosed = true;
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