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