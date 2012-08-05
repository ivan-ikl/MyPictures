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
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using MyPictures.Web.Controllers;
    using MyPictures.Web.Models;
    using MyPictures.Web.Repositories;

    [TestClass]
    public class PicturesControllerTests
    {
        [TestMethod]
        public void ShouldHaveParameterLessConstructor()
        {
            var controller = new PicturesController();

            Assert.IsInstanceOfType(controller, typeof(PicturesController));
        }

        [TestMethod]
        public void ShouldThrowWhenMimeIsNotFormData()
        {
            try
            {
                var controller = new PicturesController();
                controller.Request = new HttpRequestMessage() { Content = new StreamContent(new MemoryStream()) };
                controller.Post();
            }
            catch (HttpResponseException ex)
            {
                Assert.AreEqual(ex.Response.StatusCode, HttpStatusCode.UnsupportedMediaType);
            }
        }

        [TestMethod]
        public void ShouldRetrieveAllPictures()
        {
            var mockRepository = new Mock<IPictureRepository>();

            mockRepository.Setup(r => r.GetAll()).Returns(() =>
            {
                var result = new List<Picture>();
                result.Add(new Picture());
                result.Add(new Picture());
                return result.AsQueryable();
            });

            var pictureController = new PicturesController(mockRepository.Object, null);
            var pictures = pictureController.Get();

            Assert.AreEqual(pictures.Count(), 2);
        }

        [TestMethod]
        public void ShouldRetrieveAPicture()
        {
            var id = Guid.NewGuid();

            var mockRepository = new Mock<IPictureRepository>();
            mockRepository.Setup(r => r.Get(id)).Returns(() =>
            {
                return new Picture() { Id = id };
            });

            var pictureController = new PicturesController(mockRepository.Object, null);
            var pictures = pictureController.Get(id);

            Assert.AreEqual(pictures.Id, id);
        }

        [TestMethod]
        public void ShouldSaveAPicture()
        {
            var id = Guid.NewGuid();

            var mockPictureRepository = new Mock<IPictureRepository>();
            mockPictureRepository.Setup(r => r.Save(It.IsAny<Picture>(), It.IsAny<byte[]>())).Returns<Picture, byte[]>((p, i) =>
            {
                p.Id = id;
                p.Url = new Uri("http://foo", UriKind.Absolute);
                return p;
            });

            var mockTagRepository = new Mock<ITagRepository>();

            var pictureController = new PicturesController(mockPictureRepository.Object, mockTagRepository.Object);

            var content = new StreamContent(File.OpenRead(Path.Combine(Environment.CurrentDirectory, "multipartStream.txt")));
            content.Headers.Add("Content-Type", "multipart/form-data; boundary=---------------------------7dcfd134057c");
            pictureController.Request = new HttpRequestMessage() { Content = content };

            var picture = pictureController.Post();

            Assert.AreEqual(picture.Id, id);
            Assert.AreEqual(picture.Url.ToString(), "http://foo/");
            Assert.AreEqual(picture.Name, "test");
            Assert.AreEqual(picture.Description, "testDescription");
            Assert.AreEqual(picture.Tags.Split(',').Count(), 3);

            mockTagRepository.Verify(r => r.IncrementPictureCount("tag1"), Times.Once());
            mockTagRepository.Verify(r => r.IncrementPictureCount("tag2"), Times.Once());
            mockTagRepository.Verify(r => r.IncrementPictureCount("tag3"), Times.Once());
            mockTagRepository.Verify(r => r.IncrementPictureCount(It.IsAny<string>()), Times.Exactly(3));
        }

        [TestMethod]
        public void ShouldSaveAPictureWithFileName()
        {
            var id = Guid.NewGuid();

            var mockPictureRepository = new Mock<IPictureRepository>();
            mockPictureRepository.Setup(r => r.Save(It.IsAny<Picture>(), It.IsAny<byte[]>())).Returns<Picture, byte[]>((p, i) =>
            {
                p.Id = id;
                p.Url = new Uri("http://foo", UriKind.Absolute);
                return p;
            });

            var mockTagRepository = new Mock<ITagRepository>();

            var pictureController = new PicturesController(mockPictureRepository.Object, mockTagRepository.Object);

            var content = new StreamContent(File.OpenRead(Path.Combine(Environment.CurrentDirectory, "multipartStreamEmpty.txt")));
            content.Headers.Add("Content-Type", "multipart/form-data; boundary=---------------------------7dcfd134057c");
            pictureController.Request = new HttpRequestMessage() { Content = content };

            var picture = pictureController.Post();

            Assert.AreEqual(picture.Id, id);
            Assert.AreEqual(picture.Url.ToString(), "http://foo/");
            Assert.AreEqual(picture.Name, "test.txt");
            Assert.IsNull(picture.Description);
            Assert.IsNull(picture.Tags);

            mockTagRepository.Verify(r => r.IncrementPictureCount(It.IsAny<string>()), Times.Never());
        }

        [TestMethod]
        public void ShouldDeleteAPicture()
        {
            var id = Guid.NewGuid();

            var mockPictureRepository = new Mock<IPictureRepository>();
            mockPictureRepository.Setup(r => r.Get(id)).Returns(() =>
                {
                    return new Picture() { Tags = "tag1,tag2" };
                });

            var mockTagRepository = new Mock<ITagRepository>();

            var pictureController = new PicturesController(mockPictureRepository.Object, mockTagRepository.Object);
            pictureController.Delete(id);

            mockPictureRepository.Verify(r => r.Get(id), Times.Once());
            mockPictureRepository.Verify(r => r.Delete(id), Times.Once());
            mockTagRepository.Verify(r => r.DecrementPictureCount("tag1"), Times.Once());
            mockTagRepository.Verify(r => r.DecrementPictureCount("tag2"), Times.Once());
            mockTagRepository.Verify(r => r.DecrementPictureCount(It.IsAny<string>()), Times.Exactly(2));
        }
    }
}