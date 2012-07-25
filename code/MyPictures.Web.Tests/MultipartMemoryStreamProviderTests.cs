namespace MyPictures.Web.Tests
{
    using System;
    using System.IO;
    using System.Net.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MyPictures.Web.Helpers;
    
    [TestClass]
    public class MultipartMemoryStreamProviderTests
    {
        [TestMethod]
        public void ShouldHaveParameterLessConstructor()
        {
            var provider = new MultipartMemoryStreamProvider();

            Assert.IsInstanceOfType(provider, typeof(MultipartMemoryStreamProvider));
        }

        [TestMethod]
        public void ShouldRetrieveEmptyBodyPartFileNames()
        {
            var provider = new MultipartMemoryStreamProvider();
            provider.BodyPartFileNames.Add("key1", "value1");

            Assert.AreEqual(provider.BodyPartFileNames.Count, 0);
        }

        [TestMethod]
        public void ShouldThrowOnNullHeaders()
        {
            var provider = new MultipartMemoryStreamProvider();

            bool thrown = false;
            try
            {
                provider.GetStream(null);
            }
            catch (ArgumentNullException)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown);
        }

        [TestMethod]
        public void ShouldThrowOnNullContentDisposition()
        {
            var content = new StreamContent(new MemoryStream());

            var provider = new MultipartMemoryStreamProvider();

            bool thrown = false;
            try
            {
                provider.GetStream(content.Headers);
            }
            catch (IOException)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown);
        }
    }
}
