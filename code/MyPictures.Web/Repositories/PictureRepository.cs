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
    using System.Linq;
    using MyPictures.Web.Models;

    public class PictureRepository : IPictureRepository
    {
        private readonly IStorageContext storageContext;

        public PictureRepository()
            : this(StorageContext.DefaultStorageContext())
        {
        }

        public PictureRepository(IStorageContext storageContext)
        {
            this.storageContext = storageContext;
        }

        public IQueryable<Picture> GetAll()
        {
            return this.storageContext.Query<Picture>(StorageAccountConfiguration.PicturesTable);
        }

        public Picture Get(Guid id)
        {
            return this.storageContext.Query<Picture>(StorageAccountConfiguration.PicturesTable).Where(p => p.Id.Equals(id)).FirstOrDefault();
        }

        public Picture Save(Picture picture, byte[] image)
        {
            picture.Id = Guid.NewGuid();
            picture.Url = new Uri(this.storageContext.SaveBlob(picture.RowKey, image, "image/jpeg"));
            this.storageContext.AddEntity(picture, StorageAccountConfiguration.PicturesTable);
            this.storageContext.AddEntities(picture.Tags.Split(',').Select(t => new PictureTag { TagName = t, PictureId = picture.Id }), StorageAccountConfiguration.PictureTagTable);

            return picture;
        }

        public void Delete(Guid id)
        {
            var picture = this.Get(id);

            if (picture != null)
            {
                var pictureTags = picture.Tags.Split(',').Select(s => new PictureTag { TagName = s.Trim(), PictureId = picture.Id });
                this.storageContext.DeleteBlob(picture.Id.ToString());
                this.storageContext.DeleteEntity(picture, StorageAccountConfiguration.PicturesTable);
                this.storageContext.DeleteEntities(pictureTags, StorageAccountConfiguration.PictureTagTable);
            }
        }
    }
}