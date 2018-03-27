using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZeroCDN_Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroCDN_Client.Tests
{
    [TestClass()]
    public class ApiZeroCDNTests
    {
        ApiZeroCDN api = new ApiZeroCDN();

        [TestMethod()]
        public void AuthLoginPasswordTest()
        {
            var actual = api.AuthLoginPassword("nikulux", "rhjrjlbkmz109");

            Assert.AreEqual(actual, "1");
        }

        [TestMethod()]
        public void AuthLoginKeyTest()
        {
            var actual = api.AuthLoginKey("nikulux", "1234");

            Assert.AreEqual(actual, "1");
        }

        [TestMethod()]
        public void LoadFileToDirectoryTest()
        {

        }

        [TestMethod()]
        public void LoadFileToDirectoryOnLinkTest()
        {

        }

        [TestMethod()]
        public void RenameFileTest()
        {

        }

        [TestMethod()]
        public void DeleteFilesTest()
        {

        }

        [TestMethod()]
        public void CreateDirectoryTest()
        {

        }

        [TestMethod()]
        public void DeleteDirectoryTest()
        {

        }

        [TestMethod()]
        public void MovingDirectoryTest()
        {

        }

        [TestMethod()]
        public void RenameDirectoryTest()
        {

        }
    }
}