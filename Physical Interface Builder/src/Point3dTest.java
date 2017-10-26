import static org.junit.Assert.assertFalse;
import static org.junit.Assert.assertTrue;
import static org.junit.jupiter.api.Assertions.assertEquals;

import java.util.Arrays;

import org.junit.jupiter.api.Test;

class Point3dTest {

	Point3d p1 = new Point3d(1.0, 1.0, 1.0);
	Point3d p2 = new Point3d(0, 0, 0);
	Point3d p3 = new Point3d(3, 3, 3);
	Point3d p4 = new Point3d(1, 2, 3);

	@Test
	void test() {
		assertEquals("[1.0, 1.0, 1.0]", p1.subtract(p2).toString());
	}

	@Test
	void testReflectOrigin() {
		assertEquals("(-1.0, -1.0, -1.0)", p1.reflectOrigin().toString());
	}

	@Test
	void testDistanceTo() {
		assertEquals(Math.sqrt(3), p1.distanceTo(p2));
		assertEquals(Math.sqrt(5), p3.distanceTo(p4));

	}

	@Test
	void testCollinear() {
		assertFalse(p1.collinear(p4, p3));
		assertTrue(p3.collinear(p1, p2));
	}

}
