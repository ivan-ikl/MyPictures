namespace MyPictures.Web.Repositories
{
    using System.Linq;
    using MyPictures.Web.Models;

    public interface ITagRepository
    {
        IQueryable<Tag> GetAll();

        Tag Get(string tagName);

        IQueryable<PictureTag> GetPictures(string tagName);

        void IncrementPictureCount(string tagName);

        void DecrementPictureCount(string tagName);
    }
}