using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FileMoverLib;

namespace FileMoverTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestImpersonation()
        {
            Assert.IsTrue(FileMover.Impersonate("", "testuser", "testuser"));
        }
        [TestMethod]
        public void TestCopy()
        {
            Assert.IsTrue(FileMover.PerformCopy(@"\\yedijas\from\coba.txt", @"D:\Projects\test part\to\coba.txt"));
        }
    }
}
