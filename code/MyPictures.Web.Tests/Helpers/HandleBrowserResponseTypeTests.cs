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
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MyPictures.Web.Helpers;

    [TestClass]
    public class HandleBrowserResponseTypeTests
    {
        [TestMethod]
        public void ChangeContentTypeToPlainTextIfMSIE()
        {
            var agent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";

            var context = new HttpActionExecutedContext
                {
                    ActionContext = new HttpActionContext
                        {
                            ControllerContext = new HttpControllerContext()
                                {
                                    Request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/pictures/")
                                }
                        }
                };

            context.ActionContext.Request.Headers.Add("User-Agent", agent);
            context.Response = new HttpResponseMessage(HttpStatusCode.OK);
            context.Response.Content = new HttpMessageContent(new HttpResponseMessage(HttpStatusCode.OK));
            context.Response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var handler = new HandleBrowserResponseTypeAttribute();
            handler.OnActionExecuted(context);

            Assert.AreEqual("text/plain", context.Response.Content.Headers.ContentType.ToString());
        }

        [TestMethod]
        public void DontChangeContentTypeIfNotMSIE()
        {
            var agent = "Some_Client";

            var context = new HttpActionExecutedContext
            {
                ActionContext = new HttpActionContext
                {
                    ControllerContext = new HttpControllerContext()
                    {
                        Request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/pictures/")
                    }
                }
            };

            context.ActionContext.Request.Headers.Add("User-Agent", agent);
            context.Response = new HttpResponseMessage(HttpStatusCode.OK);
            context.Response.Content = new HttpMessageContent(new HttpResponseMessage(HttpStatusCode.OK));
            context.Response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var handler = new HandleBrowserResponseTypeAttribute();
            handler.OnActionExecuted(context);

            Assert.AreEqual("application/json", context.Response.Content.Headers.ContentType.ToString());
        }
    }
}