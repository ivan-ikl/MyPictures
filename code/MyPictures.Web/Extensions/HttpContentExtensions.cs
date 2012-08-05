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


namespace MyPictures.Web.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    public static class HttpContentExtensions
    {
        public static bool TryGetFormFieldValue(this IEnumerable<HttpContent> contents, string dispositionName, out string formFieldValue)
        {
            if (contents == null)
            {
                throw new ArgumentNullException("contents");
            }

            HttpContent content = contents.FirstDispositionNameOrDefault(dispositionName);

            if (content != null)
            {
                formFieldValue = content.ReadAsStringAsync().Result;
                return true;
            }

            formFieldValue = null;

            return false;
        }
    }
}