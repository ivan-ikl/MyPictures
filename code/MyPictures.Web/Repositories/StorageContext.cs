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
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Services.Client;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;
    using MyPictures.Web.Models;

    public class StorageContext : IStorageContext
    {
        public StorageContext(CloudStorageAccount storageAccount)
        {
            this.StorageAccount = storageAccount;
        }

        public CloudStorageAccount StorageAccount { get; private set; }

        public static StorageContext DefaultStorageContext()
        {
            // NOTE:  For the Preview we need to provide custom storage Endpoint URIs
            var context = new StorageContext(Microsoft.WindowsAzure.CloudStorageAccount.Parse(ConfigurationManager.AppSettings.Get(StorageAccountConfiguration.StorageAccount)));

            var blobContainer = context.GetBlobContainer(StorageAccountConfiguration.BlobContainer);
            blobContainer.CreateIfNotExist();
            var permissions = new BlobContainerPermissions();
            blobContainer.SetPermissions(new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Container });

            context.CreateIfNotExist<Picture>(StorageAccountConfiguration.PicturesTable);
            context.CreateIfNotExist<Tag>(StorageAccountConfiguration.TagsTable);
            context.CreateIfNotExist<PictureTag>(StorageAccountConfiguration.PictureTagTable);

            return context;
        }

        public IQueryable<T> Query<T>(string tableName)
        {
            TableServiceContext context = this.CreateContext<T>();
            return context.CreateQuery<T>(tableName).AsTableServiceQuery();
        }

        public void AddEntity<T>(T obj, string tableName)
        {
            this.AddEntities(new[] { obj }, tableName);
        }

        public void AddEntities<T>(IEnumerable<T> objs, string tableName)
        {
            TableServiceContext context = this.CreateContext<T>();

            foreach (var obj in objs)
            {
                context.AddObject(tableName, obj);
            }

            var saveChangesOptions = SaveChangesOptions.None;
            if (objs.Cast<ITableServiceEntity>().Distinct(new PartitionKeyComparer()).Count() == 1)
            {
                saveChangesOptions = SaveChangesOptions.Batch;
            }

            context.SaveChanges(saveChangesOptions);
        }

        public void AddOrUpdateEntity<T>(T obj, string tableName) where T : ITableServiceEntity
        {
            var pk = obj.PartitionKey;
            var rk = obj.RowKey;
            T existingObj = default(T);

            try
            {
                existingObj = (from o in this.Query<T>(tableName)
                               where o.PartitionKey == pk && o.RowKey == rk
                               select o).SingleOrDefault();
            }
            catch
            {
            }

            if (existingObj == null)
            {
                this.AddEntity(obj, tableName);
            }
            else
            {
                TableServiceContext context = this.CreateContext<T>();
                context.AttachTo(tableName, obj, "*");
                context.UpdateObject(obj);
                context.SaveChanges(SaveChangesOptions.ReplaceOnUpdate);
            }
        }

        public void DeleteEntity<T>(T obj, string tableName)
        {
            this.DeleteEntities(new[] { obj }, tableName);
        }

        public void DeleteEntities<T>(IEnumerable<T> objs, string tableName)
        {
            TableServiceContext context = this.CreateContext<T>();
            foreach (var obj in objs)
            {
                context.AttachTo(tableName, obj, "*");
                context.DeleteObject(obj);
            }

            try
            {
                context.SaveChanges();
            }
            catch (DataServiceRequestException ex)
            {
                var dataServiceClientException = ex.InnerException as DataServiceClientException;
                if (dataServiceClientException != null)
                {
                    if (dataServiceClientException.StatusCode == 404)
                    {
                        return;
                    }
                }

                throw;
            }
        }

        public string SaveBlob(string objId, byte[] content, string contentType)
        {
            CloudBlob blob = this.GetBlobContainer(StorageAccountConfiguration.BlobContainer).GetBlobReference(objId);
            blob.Properties.ContentType = contentType;
            blob.UploadByteArray(content);
            return blob.Uri.ToString();
        }

        public void DeleteBlob(string objId)
        {
            CloudBlob blob = this.GetBlobContainer(StorageAccountConfiguration.BlobContainer).GetBlobReference(objId);
            blob.DeleteIfExists();
        }

        private TableServiceContext CreateContext<T>()
        {
            var context = new TableServiceContext(this.StorageAccount.TableEndpoint.ToString(), this.StorageAccount.Credentials)
            {
                ResolveType = t => typeof(T),
                RetryPolicy = RetryPolicies.RetryExponential(RetryPolicies.DefaultClientRetryCount, RetryPolicies.DefaultClientBackoff)
            };

            return context;
        }

        private bool CreateIfNotExist<T>(string tableName) where T : ITableServiceEntity
        {
            var cloudTableClient = new CloudTableClient(this.StorageAccount.TableEndpoint.ToString(), this.StorageAccount.Credentials);
            bool result = cloudTableClient.CreateTableIfNotExist(tableName);

            if (cloudTableClient.BaseUri.IsLoopback)
            {
                TableServiceContext context = cloudTableClient.GetDataServiceContext();
                DateTime now = DateTime.UtcNow;
                ITableServiceEntity entity = Activator.CreateInstance(typeof(T)) as ITableServiceEntity;
                entity.PartitionKey = Guid.NewGuid().ToString();
                entity.RowKey = Guid.NewGuid().ToString();
                Array.ForEach(
                    entity.GetType().GetProperties(
                        BindingFlags.Public | BindingFlags.Instance),
                        p =>
                        {
                            if ((p.Name != "PartitionKey") && (p.Name != "RowKey") && (p.Name != "Timestamp"))
                            {
                                if (p.PropertyType == typeof(string))
                                {
                                    p.SetValue(entity, Guid.NewGuid().ToString(), null);
                                }
                                else if (p.PropertyType == typeof(DateTime))
                                {
                                    p.SetValue(entity, now, null);
                                }
                            }
                        });

                context.AddObject(tableName, entity);
                context.SaveChangesWithRetries();
                context.DeleteObject(entity);
                context.SaveChangesWithRetries();
            }

            return result;
        }

        private CloudBlobContainer GetBlobContainer(string containerName)
        {
            var client = this.StorageAccount.CreateCloudBlobClient();
            client.RetryPolicy = RetryPolicies.Retry(3, TimeSpan.FromSeconds(5));
            return client.GetContainerReference(containerName);
        }

        private class PartitionKeyComparer : IEqualityComparer<ITableServiceEntity>
        {
            public bool Equals(ITableServiceEntity x, ITableServiceEntity y)
            {
                return string.Compare(x.PartitionKey, y.PartitionKey, true, CultureInfo.InvariantCulture) == 0;
            }

            public int GetHashCode(ITableServiceEntity obj)
            {
                return obj.PartitionKey.GetHashCode();
            }
        }
    }
}