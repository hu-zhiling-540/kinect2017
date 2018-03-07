import java.util.ArrayList;
import Jama.Matrix;
import Jama.SingularValueDecomposition;

/**
 * Fitting of a plane to a set of points in 3D, where the orthogonal distance
 * from the points to the plane is minimized.
 * 
 * @author Aaron Spettl, Institute of Stochastics, Ulm University
 * @see "P.P.N. de Groen: An introduction to total least squares. Nieuw Archief
 *      voor Wiskunde, 4th Series 14, 237–253 (1996)"
 * 
 *      This class is modified and extended version of this:
 *      https://github.com/stochastics-ulm-university/laguerre-approximation
 */
public class OrthogonalRegression3D {

	/** The centroid of the points. */
	protected Point3d c;

	/**
	 * The matrix consisting the coordinates of the points shifted by the
	 * (reflected) centroid.
	 */
	protected Matrix M;

	/** The singular value decomposition of the matrix <code>M</code>. */
	protected SingularValueDecomposition svd;

	/**
	 * Constructs a new object for orthogonal regression in 3D.
	 * 
	 * @param points
	 *            the set of points
	 */
	public OrthogonalRegression3D(ArrayList<Point3d> points) {
		// compute centroid of points (and check input data)
		double[] cCoord = new double[] { 0.0, 0.0, 0.0 };
		int n = 0;
		for (Point3d obj : points) {
			double[] coord = obj.getArr();
			cCoord[0] += coord[0];
			cCoord[1] += coord[1];
			cCoord[2] += coord[2];
			n++;
		}
		cCoord[0] /= n;
		cCoord[1] /= n;
		cCoord[2] /= n;
		c = new Point3d(cCoord);

		// construct matrix with shifted point coordinates
		M = new Matrix(n, 3);
		int i = 0;
		for (Point3d obj : points) {
			double[] coord = obj.getArr();
			M.set(i, 0, coord[0] - cCoord[0]);
			M.set(i, 1, coord[1] - cCoord[1]);
			M.set(i, 2, coord[2] - cCoord[2]);
			i++;
		}
//		System.out.println(strung(M));
		svd = null;
	}

	public static String strung(Matrix m) {
		StringBuffer sb = new StringBuffer();
		for (int r = 0; r < m.getRowDimension(); ++r) {
			for (int c = 0; c < m.getColumnDimension(); ++c)
				sb.append(m.get(r, c)).append("\t");
			sb.append("\n");
		}
		return sb.toString();
	}

	/**
	 * Returns the centroid of the points.
	 * 
	 * @return the centroid of the points
	 */
	public Point3d getCentroid() {
		return c;
	}

	/**
	 * Returns the matrix consisting the coordinates of the points shifted by the
	 * (reflected) centroid.
	 * 
	 * @return the matrix consisting the coordinates of the points shifted by the
	 *         (reflected) centroid
	 */
	public Matrix getShiftedCoordinates() {
		return M;
	}

	/**
	 * Returns the singular value decomposition of the matrix containing the shifted
	 * coordinates.
	 * 
	 * @return the singular value decomposition of the matrix containing the shifted
	 *         coordinates
	 */
	public SingularValueDecomposition getSVD() {
		if (svd == null) {
			svd = M.svd();
		}
		return svd;
	}

	/**
	 * Estimates the parameters of a plane by minimizing the orthogonal distance of
	 * the given points to the plane.
	 * 
	 * 
	 * @return the plane
	 * @throws IllegalArgumentException
	 *             if there is a problem with the points (e.g., not enough points)
	 */
	public Plane3d fitPlane() {
		return fitPlane(1.0);
	}

	/**
	 * Estimates the parameters of a plane by minimizing the orthogonal distance of
	 * the given points to the plane.
	 * 
	 * @param singularValueRatio
	 *            the maximum value allowed for the ratio of the smallest singular
	 *            value to the second-smallest (this can be used to avoid detecting
	 *            planes for vertex- or edge-like shapes), i.e., <code>1.0</code> is
	 *            no restriction
	 * @return the plane or <code>null</code> if the
	 *         <code>singularValueRatio</code>-condition is not fulfilled
	 * @throws IllegalArgumentException
	 *             if there is a problem with the points (e.g., not enough points)
	 */
	public Plane3d fitPlane(double singularValueRatio) {
		// use singular value decomposition to get smallest singular value and its
		// right-singular vector
		SingularValueDecomposition svd = getSVD();
		int r = svd.rank();
		if (r < 3) {
			throw new IllegalArgumentException("Not enough points (which are not collinear)!");
		}
		double[] diagS = svd.getSingularValues();
		if (diagS[1] * singularValueRatio < diagS[2]) {
			// points are too different from a plane (smallest singular value is not that
			// much smaller than the second-smallest)
			return null;
		}
		Matrix V = svd.getV();

		// right-singular vector (of smallest singular value) is the normal vector of
		// the plane
		Vec3d normalVector = new Point3d(V.getMatrix(0, 2, r - 1, r - 1).getColumnPackedCopy());

		// centroid lies on the plane, use this to construct distance to origin
		// double distance = normalVector.getScalarProduct(c);
		double distance = normalVector.dot(c);

		// parameterization of our plane requires d to be non-negative
		if (distance < 0.0) {
			normalVector = normalVector.reflectOrigin();
//			System.out.println(normalVector.toString());
			distance = -distance;
		}

		return new Plane3d(normalVector, distance);
	}

}