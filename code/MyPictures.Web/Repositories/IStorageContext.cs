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

namespace MyPictures.Web.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.WindowsAzure;

    public interface IStorageContext
    {
        CloudStorageAccount StorageAccount { get; }

        IQueryable<T> Query<T>(string tableName);

        void AddEntity<T>(T obj, string tableName);

        void AddEntities<T>(IEnumerable<T> objs, string tableName);
        
        void AddOrUpdateEntity<T>(T obj, string tableName) where T : ITableServiceEntity;

        void DeleteEntity<T>(T obj, string tableName);

        void DeleteEntities<T>(IEnumerable<T> objs, string tableName);

        string SaveBlob(string objId, byte[] content, string contentType);

        void DeleteBlob(string objId);
    }
}
