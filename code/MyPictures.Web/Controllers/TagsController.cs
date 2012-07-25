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