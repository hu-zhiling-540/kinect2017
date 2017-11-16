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
	boolean isClosed; // whether shape is finished adding vertex
	Plane3d onPlane; // for 2d shape

	ArrayList<Point3d> tempPts; // rough points added to the shape
	ArrayList<Point3d> planarPts; // points outlining the shape
	Vec3d extrusion;

	double xMin = Double.NEGATIVE_INFINITY;
	double xMax = Double.POSITIVE_INFINITY;
	double yMin = Double.NEGATIVE_INFINITY;
	double yMax = Double.POSITIVE_INFINITY;
	double zMin = Double.NEGATIVE_INFINITY;
	double zMax = Double.POSITIVE_INFINITY;

	public Shape3d() {
		tempPts = new ArrayList<Point3d>();
		planarPts = new ArrayList<Point3d>();
		isClosed = false;
		extrusion = null;
	}

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
			throw new IllegalArgumentException("Shape is closed.");
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
			else if (pt.getX() < xMin)
				xMin = pt.getX();
			if (pt.getY() > yMax)
				yMax = pt.getY();
			else if (pt.getY() < yMin)
				yMin = pt.getY();
		}
		tempPts.add(pt);
	}

	public void validate() {
		if (isClosed)
			throw new IllegalArgumentException("Shape is closed.");
		if (tempPts.size() == 0)
			throw new IllegalArgumentException("No points to be validate a shape.");
		// it takes at least three points to form a polygon/shape
		if (tempPts.size() < 3)
			throw new IllegalArgumentException("Number of points is " + tempPts.size());
		// check if a list of points are collinear

		isClosed = true;
	}

	public Shape3d buildShape() {
		validate();
		if (extrusion == null) {
			planarPts = tempPts;
			return this;
		}
		OrthogonalRegression3D or = new OrthogonalRegression3D(tempPts);
		Plane3d plane = or.fitPlane(1.0);
		Shape3d sp = new Shape3d(plane, tempPts);
		// Shape3d sp = null;
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
			throw new IllegalArgumentException("Shape is not closed yet.");
		if (pt.getX() > xMax || pt.getX() < xMin || pt.getY() > yMax || pt.getY() < yMin)
			return true;
		// is a 3d shape
		if (!is2dShape()) {
			if (pt.getZ() > zMax || pt.getZ() < zMin)
				return true;
		}
		return false;
	}

	// a half-line, together with its start point.
	public LineSeg createRay(Point3d pt) {
		double epsilon = (xMax - xMin) / 10e6;
		Point3d extreme = new Point3d(xMax + epsilon, yMax);
		return new LineSeg(pt, extreme);
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
	public boolean contains(Point3d pt) {
		if (offFixedBounds(pt))
			return false;
		LineSeg ray = createRay(pt);
		int xings = 0;
		for (int i = 1; i < planarPts.size(); i++) {
			LineSeg edge = new LineSeg(planarPts.get(i - 1), planarPts.get(i));
			if (edge.crossLine(ray))
				xings += 1;
		}
		LineSeg edge = new LineSeg(planarPts.get(planarPts.size() - 1), planarPts.get(0));
		if (edge.crossLine(ray))
			xings += 1;
		if (xings % 2 == 1)
			return true;
		return false;
	}

	/**
	 * Extrusion is null for a 2d shape
	 * 
	 * @return
	 */
	public boolean is2dShape() {
		return extrusion == null;
	}

	public void setExtr(Vec3d extrusion) {
		this.extrusion = extrusion;
	}

	public Vec3d getExtr() {
		return this.extrusion;
	}

	class LineSeg {
		Point3d start;
		Point3d end;
		// 2d mode
		double k; // slope
		double b; // intercept

		LineSeg(Point3d start, Point3d end) {
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

		// the angle is the arctangent2 of (y2 - y1, x2 - x1)
		double angle() {
			return Math.atan2(end.getY() - start.getY(), end.getX() - start.getX()) * 180.0 / Math.PI;
		}

		// line segment connecting points (x1, y1) and (x2, y2) is the square root of
		// (x2 - x1)2 + (y2 - y1)2
		double length() {
			return Math.sqrt(Math.pow(end.getX() - start.getX(), 2) + Math.pow(end.getY() - start.getY(), 2));
		}

		boolean isParallel(LineSeg that) {
			return this.slope() == that.slope();
		}

		// y = mx + b

		boolean isVertical() {
			return start.getX() == end.getX();
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

		boolean crossLine(LineSeg that) {
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
}