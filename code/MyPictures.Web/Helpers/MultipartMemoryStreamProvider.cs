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


namespace MyPictures.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    
    public class MultipartMemoryStreamProvider : IMultipartStreamProvider
    {
        private Dictionary<string, string> bodyPartFileNames;
        private object thisLock = new object();

        public MultipartMemoryStreamProvider()
        {
            this.bodyPartFileNames = new Dictionary<string, string>();
            this.FilesBytes = new List<MemoryStream>();
        }

        public IList<MemoryStream> FilesBytes { get; set; }
        
        public IDictionary<string, string> BodyPartFileNames
        {
            get
            {
                lock (this.thisLock)
                {
                    return (this.bodyPartFileNames != null) ? new Dictionary<string, string>(this.bodyPartFileNames) : new Dictionary<string, string>();
                }
            }
        }

        public Stream GetStream(HttpContentHeaders headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            return this.OnGetStream(headers);
        }
        
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception is propagated.")]
        protected virtual string GetLocalFileName(HttpContentHeaders headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            string str = null;

            try
            {
                ContentDispositionHeaderValue contentDisposition = headers.ContentDisposition;
                if (contentDisposition != null)
                {
                    str = ExtractLocalFileName(contentDisposition);
                }
            }
            catch (Exception)
            {
            }

            if (str == null)
            {
                str = string.Format(CultureInfo.InvariantCulture, "BodyPart_{0}", new object[] { Guid.NewGuid() });
            }

            return str;
        }

        protected virtual Stream OnGetStream(HttpContentHeaders headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            ContentDispositionHeaderValue contentDisposition = headers.ContentDisposition;

            if (contentDisposition == null)
            {
                throw new IOException("content-disposition null");
            }

            if (string.IsNullOrEmpty(contentDisposition.FileName))
            {
                return new MemoryStream();
            }

            lock (this.thisLock)
            {
                this.bodyPartFileNames.Add(contentDisposition.FileName, Path.GetFileName(this.GetLocalFileName(headers)));
            }

            var memoryStream = new MemoryStream();
            this.FilesBytes.Add(memoryStream);
            return memoryStream;
        }

        private static string ExtractLocalFileName(ContentDispositionHeaderValue contentDisposition)
        {
            if (contentDisposition == null)
            {
                throw new ArgumentNullException("contentDisposition");
            }

            string fileNameStar = contentDisposition.FileNameStar;
            if (string.IsNullOrEmpty(fileNameStar))
            {
                fileNameStar = contentDisposition.FileName;
            }

            if (string.IsNullOrWhiteSpace(fileNameStar))
            {
                throw new ArgumentException("fileNameStar empty");
            }

            var fileName = UnquoteToken(fileNameStar);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("fileNameStar empty");
            }

            return Path.GetFileName(fileName);
        }

        private static string UnquoteToken(string token)
        {
            if (!string.IsNullOrWhiteSpace(token) && ((token.StartsWith("\"", StringComparison.Ordinal) && token.EndsWith("\"", StringComparison.Ordinal)) && (token.Length > 1)))
            {
                return token.Substring(1, token.Length - 2);
            }

            return token;
        }
    }
}