import static org.junit.jupiter.api.Assertions.*;

import org.junit.BeforeClass;
import org.junit.jupiter.api.Test;

class Vec3dTest {

	Vec3d v1 = new Vec3d(); // first constructor
	Vec3d v2 = new Vec3d(1, 1, 1); // second constructor
	Vec3d v3 = new Vec3d(new double[] { 1, 2, 3 }); // third constructor
	Vec3d v4 = new Vec3d(3, 3, 3);
	Vec3d v5 = new Vec3d(1 / 3d, 1 / 3d, 1 / 3d);
	Vec3d v6 = new Vec3d(-1, -1, -1);
	Vec3d v7 = new Vec3d(1, 0, 0);

	@Test
	void testConstructors() {
		assertEquals("(0.0, 0.0, 0.0)", new Vec3d().toString());
		assertEquals("(1.0, 1.0, 1.0)", v2.toString());
		assertEquals("(1.0, 2.0, 3.0)", v3.toString());
	}

	@Test
	void testAdd() {
		assertTrue(v2.sameValue(v1.add(v2)));
		assertTrue(v2.sameValue(v2.add(v1)));
		Vec3d v = new Vec3d(4 / 3d, 4 / 3d, 4 / 3d);
		assertTrue(v.sameValue(v2.add(v5)));
	}

	@Test
	void testSubtract() {
		assertTrue(v2.sameValue(v2.subtract(v1)));
		assertEquals("(-1.0, -1.0, -1.0)", v1.subtract(v2).toString());
	}

	@Test
	void testScale() {
		assertTrue(v2.sameValue(v2.scale(1)));
		assertTrue(v1.sameValue(v2.scale(0)));
		assertTrue(v2.sameValue(v4.scale(1 / 3d)));
	}

	@Test
	void testDot() {
		assertEquals(0, v1.dot(v1));
		assertEquals(3, v2.dot(v2));
		assertEquals(6, v2.dot(v3));
		assertEquals(v5.dot(v4), v4.dot(v5));
	}

	@Test
	void testCross() {
		assertTrue(v1.sameValue(v1.cross(v1)));
		assertTrue(v1.sameValue(v2.cross(v2)));
		assertTrue(v1.sameValue(v1.cross(v2)));
		assertTrue(new Vec3d(-3, 6, -3).sameValue(v3.cross(v4)));
		// assertTrue(new Vec3d(-1 / 3d, 2 / 3d, -1 / 3d).sameValue(v3.cross(v5)));
	}

	@Test
	void testMagnitude() {
		assertEquals(0, v1.magnitude());
		assertEquals(v6.magnitude(), v2.magnitude());
		assertEquals((double) Math.sqrt(3), v2.magnitude());
		assertEquals((double) Math.sqrt(1 / 3d), v5.magnitude());
	}

	@Test
	void testDistanceTo() {
		assertEquals(0, v2.distanceTo(v2));
		assertEquals((double) Math.sqrt(3), v1.distanceTo(v2));
		assertEquals((double) Math.sqrt(48), v4.distanceTo(v6));
		assertEquals(v2.distanceTo(v6), v6.distanceTo(v2));
	}

	@Test
	void testNormalize() {
		assertTrue(new Vec3d(1d / Math.sqrt(3), 1d / Math.sqrt(3), 1d / Math.sqrt(3)).sameValue(v2.normalize()));
		assertTrue(v7.sameValue(v7.normalize()));
	}
}
