namespace MyPictures.Web.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MyPictures.Web.Extensions;

    [TestClass]
    public class HttpContentExtensionsTests
    {
        [TestMethod]
        public void ShouldThrowOnNullInstance()
        {
            string name;
            IEnumerable<HttpContent> contents = null;
            bool thrown = false;
            try
            {
                contents.TryGetFormFieldValue("name", out name);
            }
            catch (ArgumentNullException)
            {
                thrown = true;
            }

            Assert.IsTrue(thrown);
        }
    }
}
