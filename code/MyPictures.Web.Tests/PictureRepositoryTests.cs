namespace MyPictures.Web.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyPictures.Web.Models;
    using MyPictures.Web.Repositories;

    [TestClass]
    public class PictureRepositoryTests
    {
        [TestMethod]
        public void ShouldHaveParameterLessConstructor()
        {
            var repository = new PictureRepository();

            Assert.IsInstanceOfType(repository, typeof(PictureRepository));
        }

        [TestMethod]
        public void ShouldRetrieveAllPictures()
        {
            var mockStorage = new Mock<IStorageContext>();
            mockStorage.Setup(s => s.Query<Picture>(StorageAccountConfiguration.PicturesTable)).Returns(() =>
                    {
                        var result = new List<Picture>();
                        result.Add(new Picture());
                        result.Add(new Picture());
                        return result.AsQueryable();
                    });

            var repository = new PictureRepository(mockStorage.Object);
            var pictures = repository.GetAll();

            Assert.AreEqual(pictures.Count(), 2);
        }

        [TestMethod]
        public void ShouldRetrieveAPicture()
        {
            var id = Guid.NewGuid();

            var mockStorage = new Mock<IStorageContext>();
            mockStorage.Setup(s => s.Query<Picture>(StorageAccountConfiguration.PicturesTable)).Returns(() =>
                    {
                        var result = new List<Picture>();
                        result.Add(new Picture() { Id = id });
                        result.Add(new Picture() { Id = Guid.NewGuid() });
                        return result.AsQueryable();
                    });

            var repository = new PictureRepository(mockStorage.Object);
            var picture = repository.Get(id);

            Assert.AreEqual(picture.Id, id);
        }

        [TestMethod]
        public void ShouldSaveAPicture()
        {
            var picture = new Picture() { Name = "picture1", Description = "desc of picture1", Tags = string.Empty };
            var image = new byte[] { 1, 2, 3 };

            var mockStorage = new Mock<IStorageContext>();
            mockStorage.Setup(s => s.SaveBlob(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>())).Returns(() =>
                        {
                            return "http://foo";
                        });

            var repository = new PictureRepository(mockStorage.Object);
            picture = repository.Save(picture, image);

            Assert.AreNotEqual(picture.Id, Guid.Empty);
            Assert.AreEqual(picture.Url, new Uri("http://foo"));

            mockStorage.Verify(s => s.SaveBlob(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>()), Times.Once());
            mockStorage.Verify(s => s.AddEntity(It.IsAny<Picture>(), StorageAccountConfiguration.PicturesTable), Times.Once());
            mockStorage.Verify(s => s.AddEntities(It.IsAny<IEnumerable<PictureTag>>(), StorageAccountConfiguration.PictureTagTable), Times.Once());
        }

        [TestMethod]
        public void ShouldDeleteAPicture()
        {
            var id = Guid.NewGuid();

            var mockStorage = new Mock<IStorageContext>();
            mockStorage.Setup(s => s.Query<Picture>(StorageAccountConfiguration.PicturesTable)).Returns(() =>
                {
                    var result = new List<Picture>();
                    result.Add(new Picture() { Id = id, Tags = string.Empty });
                    return result.AsQueryable();
                });

            var repository = new PictureRepository(mockStorage.Object);
            repository.Delete(id);

            mockStorage.Verify(s => s.DeleteBlob(It.IsAny<string>()), Times.Once());
            mockStorage.Verify(s => s.DeleteEntity(It.IsAny<Picture>(), StorageAccountConfiguration.PicturesTable), Times.Once());
            mockStorage.Verify(s => s.DeleteEntities(It.IsAny<IEnumerable<PictureTag>>(), StorageAccountConfiguration.PictureTagTable), Times.Once());
        }
    }
}
