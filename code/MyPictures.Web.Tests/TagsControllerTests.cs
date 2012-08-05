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


namespace MyPictures.Web.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyPictures.Web.Controllers;
    using MyPictures.Web.Models;
    using MyPictures.Web.Repositories;

    [TestClass]
    public class TagsControllerTests
    {
        [TestMethod]
        public void ShouldHaveParameterLessConstructor()
        {
            var controller = new TagsController();

            Assert.IsInstanceOfType(controller, typeof(TagsController));
        }

        [TestMethod]
        public void ShouldRetrieveAllTags()
        {
            var mockRepository = new Mock<ITagRepository>();

            mockRepository.Setup(r => r.GetAll()).Returns(() =>
            {
                var result = new List<Tag>();
                result.Add(new Tag());
                result.Add(new Tag());
                return result.AsQueryable();
            });

            var tagsController = new TagsController(mockRepository.Object);
            var tags = tagsController.Get();

            Assert.AreEqual(tags.Count(), 2);
        }

        [TestMethod]
        public void ShouldRetrieveATag()
        {
            var tagName = "myTag";

            var mockRepository = new Mock<ITagRepository>();

            mockRepository.Setup(r => r.Get(tagName)).Returns(() =>
            {
                return new Tag() { Name = tagName };
            });

            var tagsController = new TagsController(mockRepository.Object);
            var tag = tagsController.Get(tagName);

            Assert.AreEqual(tag.Name, tagName);
        }

        [TestMethod]
        public void ShouldRetrieveTagPictures()
        {
            var tagName = "myTag";

            var mockRepository = new Mock<ITagRepository>();

            mockRepository.Setup(r => r.GetPictures(tagName)).Returns(() =>
            {
                var result = new List<PictureTag>();
                result.Add(new PictureTag() { PictureId = Guid.NewGuid(), TagName = tagName });
                result.Add(new PictureTag() { PictureId = Guid.NewGuid(), TagName = tagName });
                return result.AsQueryable();
            });

            var tagsController = new TagsController(mockRepository.Object);
            var pictureTags = tagsController.Pictures(tagName);

            Assert.AreEqual(pictureTags.Count(), 2);
            foreach (var pictureTag in pictureTags)
            {
                Assert.AreEqual(pictureTag.TagName, tagName);
            }
        }
    }
}
