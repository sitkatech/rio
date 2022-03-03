using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rio.UnitTest
{
    [TestClass]
    public class UnitTests
    {
        //These are pure unit tests, can't do integration tests in Azure DevOps. No DB, and no API requests.
        [TestMethod]
        public void TestMethodPass()
        {
            Assert.IsTrue(true, "Test Pass.");
        }

        [TestMethod]
        [Ignore] //So the pipeline doesn't run this test.
        public void TestMethodFail()
        {
            Assert.Fail("Test Failure.");
        }
    }
}
