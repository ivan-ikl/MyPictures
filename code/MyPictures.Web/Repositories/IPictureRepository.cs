namespace MyPictures.Web.Repositories
{
    using System;
    using System.Linq;
    using MyPictures.Web.Models;

    public interface IPictureRepository
    {
        IQueryable<Picture> GetAll();

        Picture Get(Guid id);

        Picture Save(Picture picture, byte[] image);

        void Delete(Guid id);
    }
}