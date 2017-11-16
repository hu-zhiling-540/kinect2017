import static org.junit.jupiter.api.Assertions.*;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

class Shape3dTest {

	Point3d p1 = new Point3d(0, 0);
	Point3d p2 = new Point3d(10, 0);
	Point3d p3 = new Point3d(10, 10);
	Point3d p4 = new Point3d(0, 10);
	Shape3d shape1;

	@BeforeEach
	void setUp() throws Exception {
		shape1 = new Shape3d();
		shape1.addVertex(p1);
		shape1.addVertex(p2);
		shape1.addVertex(p3);
		shape1.addVertex(p4);
		shape1 = shape1.buildShape();
	}

	@Test
	void testShape3d() {
		Shape3d shape = new Shape3d();
		assertEquals(shape.tempPts.size(), 0);
		assertFalse(shape.isClosed);
		assertNull(shape.extrusion);
	}

	@Test
	void testShape3dPlane3dArrayListOfPoint3d() {
		fail("Not yet implemented"); // TODO
	}

	@Test
	void testShape3dPlane3dArrayListOfPoint3dVec3d() {
		fail("Not yet implemented"); // TODO
	}

	@Test
	void testAddVertex() {
		Shape3d shape = new Shape3d();
		shape.addVertex(p1);
		shape.addVertex(p2);
		shape.addVertex(p3);
		shape.addVertex(p4);
		assertFalse(shape.isClosed);
		assertEquals(shape.tempPts.size(), 4);
		assertEquals(0, shape.xMin);
		assertEquals(10, shape.xMax);
		assertEquals(0, shape.yMin);
		assertEquals(10, shape.yMax);

	}

	@Test
	void testValidate() {
		Shape3d shape = new Shape3d();
		// no vertices
		try {
			shape.validate();
			fail("My method didn't throw when I expected it to");
		} catch (IllegalArgumentException expectedException) {
		}

		// less than three vertices
		shape.addVertex(p1);
		shape.addVertex(p2);
		try {
			shape.validate();
			fail("My method didn't throw when I expected it to");
		} catch (IllegalArgumentException expectedException) {
		}

		// three vertices
		shape.addVertex(p3);
		shape.validate();
		assertTrue(shape.isClosed);

		// try to add a vertex after closing the shape
		try {
			shape.addVertex(p4);
			fail("My method didn't throw when I expected it to");
		} catch (IllegalArgumentException expectedException) {
		}

		try {
			shape.validate();
			fail("My method didn't throw when I expected it to");
		} catch (IllegalArgumentException expectedException) {
		}
	}

	@Test
	void testBuildShape() {
		// ONLY TESTED on 2d case
		Shape3d shape = new Shape3d();
		shape.addVertex(p1);
		shape.addVertex(p2);
		shape.addVertex(p3);
		shape.addVertex(p4);
		assertEquals(shape, shape.buildShape());
	}

	@Test
	void testOffFixedBounds() {
		assertTrue(shape1.offFixedBounds(new Point3d(20, 20)));
		assertFalse(shape1.offFixedBounds(new Point3d(0, 10)));
		assertFalse(shape1.offFixedBounds(new Point3d(10, 10)));
	}

	@Test
	void testCreateRay() {
		fail("Not yet implemented"); // TODO
	}

	@Test
	void testContains() {
		assertTrue(shape1.contains(new Point3d(5, 5)));
		assertFalse(shape1.contains(new Point3d(20, 20)));
		assertTrue(shape1.contains(new Point3d(10, 10)));
	}

	@Test
	void testIs2dShape() {
		fail("Not yet implemented"); // TODO
	}

	@Test
	void testSetExtr() {
		fail("Not yet implemented"); // TODO
	}

	@Test
	void testGetExtr() {
		fail("Not yet implemented"); // TODO
	}

}
