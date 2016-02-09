using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerminologyLauncher.Entities.System.Java;
using TerminologyLauncher.Utils;

namespace TerminologyLauncher.UnitTest
{
    [TestClass]
    public class UtilsTest
    {
        [TestMethod]
        public void ResourceUtilsTest()
        {

            var stream = ResourceUtils.ReadEmbedFileResource("TerminologyLauncher.UnitTest.Resources.default_large.png");
            Assert.IsNotNull(stream);
            var content = new StreamReader(stream).ReadToEnd();
            Console.WriteLine(content);
            Assert.IsTrue(String.IsNullOrEmpty(content));
        }

        [TestMethod]
        public void JavaIdentifyUnitTest()
        {
            var javaDetail = JavaUtils.GetJavaDetails(@"C:\Program Files\Java\jdk1.7.0_51\bin\java.exe");
            Assert.IsTrue(javaDetail.JavaType == JavaType.ServerX64);
        }

    }
}
