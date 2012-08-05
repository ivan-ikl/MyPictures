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


namespace MyPictures.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using MyPictures.Web.Extensions;
    using MyPictures.Web.Helpers;
    using MyPictures.Web.Models;
    using MyPictures.Web.Repositories;

    public class PicturesController : ApiController
    {
        private readonly IPictureRepository pictureRepository;
        private readonly ITagRepository tagRepository;

        public PicturesController()
            : this(new PictureRepository(), new TagRepository())
        {
        }

        public PicturesController(IPictureRepository pictureRepository, ITagRepository tagRepository)
        {
            this.pictureRepository = pictureRepository;
            this.tagRepository = tagRepository;
        }

        // GET /api/pictures
        public IQueryable<Picture> Get()
        {
            return this.pictureRepository.GetAll();
        }

        // GET /api/pictures/68D58E01-B8D0-47FB-8790-F9F3934F1F97
        public Picture Get(Guid id)
        {
            return this.pictureRepository.Get(id);
        }

        // POST /api/pictures
        [HandleBrowserResponseType]
        public Picture Post()
        {
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                throw new HttpResponseException(
                    new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType)
                    );
            }

            var streamProvider = new MultipartMemoryStreamProvider();

            var bodyparts = Request.Content.ReadAsMultipartAsync(streamProvider).Result;

            IDictionary<string, string> bodyPartFileNames = streamProvider.BodyPartFileNames;

            var data = streamProvider.FilesBytes.FirstOrDefault();

            var fileName = bodyPartFileNames.Select(kv => kv.Value).FirstOrDefault();

            string nameField;
            string descriptionField;
            string tagField;

            if (!bodyparts.TryGetFormFieldValue("name", out nameField))
            {
                nameField = fileName;
            }

            bodyparts.TryGetFormFieldValue("description", out descriptionField);

            bodyparts.TryGetFormFieldValue("tags", out tagField);

            Picture picture = null;
            var image = streamProvider.FilesBytes.FirstOrDefault();
            if (image != null)
            {
                picture = this.pictureRepository.Save(new Picture() { Name = nameField, Description = descriptionField, Tags = tagField }, image.ToArray());

                if (!string.IsNullOrEmpty(picture.Tags))
                {
                    foreach (var tag in picture.Tags.Split(','))
                    {
                        this.tagRepository.IncrementPictureCount(tag);
                    }
                }
            }

            return picture;
        }

        // DELETE /api/pictures/68D58E01-B8D0-47FB-8790-F9F3934F1F97
        public void Delete(Guid id)
        {
            var picture = this.pictureRepository.Get(id);
            if (picture != null)
            {
                this.pictureRepository.Delete(id);
                foreach (var tag in picture.Tags.Split(','))
                {
                    this.tagRepository.DecrementPictureCount(tag.Trim());
                }
            }
        }
    }
}