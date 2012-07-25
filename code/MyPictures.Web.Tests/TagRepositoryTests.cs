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
    public class TagRepositoryTests
    {
        [TestMethod]
        public void ShouldHaveParameterLessConstructor()
        {
            var repository = new TagRepository();

            Assert.IsInstanceOfType(repository, typeof(TagRepository));
        }

        [TestMethod]
        public void ShouldRetrieveAllTags()
        {
            var mockStorage = new Mock<IStorageContext>();
            mockStorage.Setup(s => s.Query<Tag>(StorageAccountConfiguration.TagsTable)).Returns(() =>
                {
                    var result = new List<Tag>();
                    result.Add(new Tag());
                    result.Add(new Tag());
                    return result.AsQueryable();
                });

            var repository = new TagRepository(mockStorage.Object);
            var tags = repository.GetAll();

            Assert.AreEqual(tags.Count(), 2);
        }

        [TestMethod]
        public void ShouldRetrieveATag()
        {
            var tagName = "tag1";

            var mockStorage = new Mock<IStorageContext>();
            mockStorage.Setup(s => s.Query<Tag>(StorageAccountConfiguration.TagsTable)).Returns(() =>
                {
                    var result = new List<Tag>();
                    result.Add(new Tag() { Name = tagName });
                    result.Add(new Tag() { Name = "tag2" });
                    return result.AsQueryable();
                });

            var repository = new TagRepository(mockStorage.Object);
            var tag = repository.Get(tagName);

            Assert.AreEqual(tag.Name, tagName);
        }

        [TestMethod]
        public void ShouldRetrievePicturesByTag()
        {
            var tagName = "tag1";

            var mockStorage = new Mock<IStorageContext>();
            mockStorage.Setup(s => s.Query<PictureTag>(StorageAccountConfiguration.PictureTagTable)).Returns(() =>
                {
                    var result = new List<PictureTag>();
                    result.Add(new PictureTag() { TagName = tagName, PictureId = Guid.NewGuid() });
                    result.Add(new PictureTag() { TagName = "tag2", PictureId = Guid.NewGuid() });
                    result.Add(new PictureTag() { TagName = tagName, PictureId = Guid.NewGuid() });
                    result.Add(new PictureTag() { TagName = "tag3", PictureId = Guid.NewGuid() });
                    result.Add(new PictureTag() { TagName = tagName, PictureId = Guid.NewGuid() });
                    return result.AsQueryable();
                });

            var repository = new TagRepository(mockStorage.Object);
            var picturesPerTag = repository.GetPictures(tagName);

            Assert.AreEqual(picturesPerTag.Count(), 3);
            foreach (var pictureTag in picturesPerTag)
            {
                Assert.AreEqual(pictureTag.TagName, tagName);
            }
        }

        [TestMethod]
        public void ShouldIncrementPictureCount()
        {
            var tagName = "tag1";

            var targetTag = new Tag() { Name = tagName, PicturesCount = 3 };

            var mockStorage = new Mock<IStorageContext>();
            mockStorage.Setup(s => s.Query<Tag>(StorageAccountConfiguration.TagsTable)).Returns(() =>
                {
                    var result = new List<Tag>();
                    result.Add(targetTag);
                    result.Add(new Tag() { Name = "tag2" });
                    result.Add(new Tag() { Name = "tag3" });
                    return result.AsQueryable();
                });

            var repository = new TagRepository(mockStorage.Object);
            repository.IncrementPictureCount(tagName);

            Assert.AreEqual(targetTag.PicturesCount, 4);
            mockStorage.Verify(s => s.AddOrUpdateEntity(It.IsAny<Tag>(), StorageAccountConfiguration.TagsTable), Times.Once());
        }

        [TestMethod]
        public void ShouldDecrementPictureCount()
        {
            var tagName = "tag1";

            var targetTag = new Tag() { Name = tagName, PicturesCount = 3 };

            var mockStorage = new Mock<IStorageContext>();
            mockStorage.Setup(s => s.Query<Tag>(StorageAccountConfiguration.TagsTable)).Returns(() =>
            {
                var result = new List<Tag>();
                result.Add(targetTag);
                result.Add(new Tag() { Name = "tag2" });
                result.Add(new Tag() { Name = "tag3" });
                return result.AsQueryable();
            });

            var repository = new TagRepository(mockStorage.Object);
            repository.DecrementPictureCount(tagName);

            Assert.AreEqual(targetTag.PicturesCount, 2);
            mockStorage.Verify(s => s.AddOrUpdateEntity(It.IsAny<Tag>(), StorageAccountConfiguration.TagsTable), Times.Once());
        }

        [TestMethod]
        public void ShouldIncrementPictureCountOnNewTag()
        {
            var tagName = "newtag";

            Tag newTag = null;

            var mockStorage = new Mock<IStorageContext>();
            mockStorage.Setup(s => s.AddOrUpdateEntity<Tag>(It.IsAny<Tag>(), StorageAccountConfiguration.TagsTable)).Callback<Tag, string>((t, n) =>
                {
                    newTag = t;
                });

            var repository = new TagRepository(mockStorage.Object);
            repository.IncrementPictureCount(tagName);

            Assert.IsNotNull(newTag);
            Assert.AreEqual(newTag.PicturesCount, 1);
            mockStorage.Verify(s => s.AddOrUpdateEntity(It.IsAny<Tag>(), StorageAccountConfiguration.TagsTable), Times.Once());
        }

        [TestMethod]
        public void ShouldIgnoreDecrementPictureCountOnNewTag()
        {
            var tagName = "newtag";

            Tag newTag = null;

            var mockStorage = new Mock<IStorageContext>();
            mockStorage.Setup(s => s.AddOrUpdateEntity<Tag>(It.IsAny<Tag>(), StorageAccountConfiguration.TagsTable)).Callback<Tag, string>((t, n) =>
                {
                    newTag = t;
                });

            var repository = new TagRepository(mockStorage.Object);
            repository.DecrementPictureCount(tagName);

            Assert.IsNull(newTag);
            mockStorage.Verify(s => s.AddOrUpdateEntity(It.IsAny<Tag>(), StorageAccountConfiguration.TagsTable), Times.Never());
        }
    }
}
