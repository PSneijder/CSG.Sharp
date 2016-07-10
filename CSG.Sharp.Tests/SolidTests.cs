using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSG.Sharp
{
    [TestClass]
    public class SolidTests
    {
        [TestMethod]
        public void TestSphereCreation()
        {
            var s = Sphere.Create();

            Assert.IsNotNull(s);
        }

        [TestMethod]
        public void TestCylinderCreation()
        {
            var c = Cylinder.Create();

            Assert.IsNotNull(c);
        }

        [TestMethod]
        public void TestCubeCreation()
        {
            var c = Cube.Create();

            Assert.IsNotNull(c);
        }
    }
}