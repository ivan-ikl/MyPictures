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
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using MyPictures.Web.Models;
    using MyPictures.Web.Repositories;

    public class TagsController : ApiController
    {
        private readonly ITagRepository repository;

        public TagsController()
            : this(new TagRepository())
        {
        }

        public TagsController(ITagRepository repository)
        {
            this.repository = repository;
        }

        // GET /api/tags
        public IQueryable<Tag> Get()
        {
            return this.repository.GetAll();
        }

        // GET /api/tags/tag1
        public Tag Get(string name)
        {
            return this.repository.Get(name);
        }

        // GET /api/tags/tag1/pictures
        public IQueryable<PictureTag> Pictures(string name)
        {
            return this.repository.GetPictures(name);
        }
    }
}