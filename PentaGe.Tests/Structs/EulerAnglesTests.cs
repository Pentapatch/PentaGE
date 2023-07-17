using PentaGE.Structs;
using System.Numerics;

namespace PentaGe.Tests.Structs
{
    [TestClass]
    public class EulerAnglesTests
    {
        private EulerAngles sut = new(45f, 30f, 0f);

        [TestMethod]
        public void EulerAngles_GetUpVector_Should_compute_correctly()
        {
            Vector3 upVector = sut.GetUpVector();
            Assert.AreEqual(new Vector3(0.383f, 0.924f, -0.001f), upVector);
        }
    }
}