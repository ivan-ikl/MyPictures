// ---------------------------------------------------------------------------------- 
// Microsoft Developer & Platform Evangelism 
//  
// Copyright (c) Microsoft Corporation. All rights reserved. 
//  
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,  
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES  
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
// ---------------------------------------------------------------------------------- 
// The example companies, organizations, products, domain names, 
// e-mail addresses, logos, people, places, and events depicted 
// herein are fictitious.  No association with any real company, 
// organization, product, domain name, email address, logo, person, 
// places, or events is intended or should be inferred. 
// ---------------------------------------------------------------------------------- 

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
