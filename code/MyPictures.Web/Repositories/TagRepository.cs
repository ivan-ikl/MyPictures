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
    using System.Linq;
    using MyPictures.Web.Models;

    public class TagRepository : ITagRepository
    {
        private readonly IStorageContext storageContext;

        public TagRepository()
            : this(StorageContext.DefaultStorageContext())
        {
        }

        public TagRepository(IStorageContext storageContext)
        {
            this.storageContext = storageContext;
        }

        public IQueryable<Tag> GetAll()
        {
            return this.storageContext.Query<Tag>(StorageAccountConfiguration.TagsTable);
        }

        public Tag Get(string tagName)
        {
            return this.storageContext.Query<Tag>(StorageAccountConfiguration.TagsTable).Where(t => t.Name.Equals(tagName)).FirstOrDefault();
        }

        public IQueryable<PictureTag> GetPictures(string tagName)
        {
            return this.storageContext.Query<PictureTag>(StorageAccountConfiguration.PictureTagTable).Where(pt => pt.TagName.Equals(tagName));
        }

        public void IncrementPictureCount(string tagName)
        {
            this.PictureCountOperation(tagName, 1);
        }

        public void DecrementPictureCount(string tagName)
        {
            this.PictureCountOperation(tagName, -1);
        }

        private void PictureCountOperation(string tagName, int offset)
        {
            var tag = this.Get(tagName);
            if (tag == null)
            {
                tag = new Tag() { Name = tagName };
            }

            tag.PicturesCount += offset;
            if (tag.PicturesCount > 0)
            {
                this.storageContext.AddOrUpdateEntity(tag, StorageAccountConfiguration.TagsTable);
            }
            else
            {
                this.storageContext.DeleteEntity(tag, StorageAccountConfiguration.TagsTable);
            }
        }
    }
}