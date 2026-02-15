using System;
using Moq;
using Xunit;
using DasBlog.Managers;
using DasBlog.Services;
using DasBlog.Services.ConfigFile.Interfaces;
using newtelligence.DasBlog.Runtime;
using Microsoft.Extensions.Logging;

namespace DasBlog.Tests.UnitTests.Managers
{
    public class CommentManagerTest
    {
        private Mock<IDasBlogSettings> settingsMock;
        private Mock<ISiteConfig> siteConfigMock;
        private Mock<ILogger<CommentManager>> loggerMock;
        private Mock<IBlogDataService> dataServiceMock;

        public CommentManagerTest()
        {
            settingsMock = new Mock<IDasBlogSettings>();
            siteConfigMock = new Mock<ISiteConfig>();
            siteConfigMock.SetupAllProperties();
            settingsMock.Setup(s => s.SiteConfiguration).Returns(siteConfigMock.Object);

            loggerMock = new Mock<ILogger<CommentManager>>();
            dataServiceMock = new Mock<IBlogDataService>();
        }

        private CommentManager CreateManager()
        {
            return new CommentManager(loggerMock.Object, settingsMock.Object, dataServiceMock.Object);
        }

        [Fact]
        public void Constructor_WithValidDependencies_ConstructsInstance()
        {
            var manager = CreateManager();
            Assert.NotNull(manager);
        }

        [Fact]
        public void GetComments_ReturnsComments()
        {
            var comments = new CommentCollection();
            comments.Add(new Comment { Author = "Test" });
            dataServiceMock.Setup(d => d.GetCommentsFor("post1", false)).Returns(comments);

            var manager = CreateManager();
            var result = manager.GetComments("post1", false);
            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
        }

        [Fact]
        public void GetAllComments_ReturnsAllComments()
        {
            var comments = new CommentCollection();
            comments.Add(new Comment { Author = "Author1" });
            comments.Add(new Comment { Author = "Author2" });
            dataServiceMock.Setup(d => d.GetAllComments()).Returns(comments);

            var manager = CreateManager();
            var result = manager.GetAllComments();
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void DeleteComment_ValidEntry_ReturnsDeleted()
        {
            var entry = new Entry { Title = "Test Entry" };
            dataServiceMock.Setup(d => d.GetEntry("post1")).Returns(entry);

            var manager = CreateManager();
            var result = manager.DeleteComment("post1", "comment1");
            Assert.Equal(CommentSaveState.Deleted, result);
        }

        [Fact]
        public void DeleteComment_InvalidEntry_ReturnsNotFound()
        {
            dataServiceMock.Setup(d => d.GetEntry("nonexistent")).Returns((Entry)null);

            var manager = CreateManager();
            var result = manager.DeleteComment("nonexistent", "comment1");
            Assert.Equal(CommentSaveState.NotFound, result);
        }

        [Fact]
        public void ApproveComment_ValidEntry_ReturnsApproved()
        {
            var entry = new Entry { Title = "Test Entry" };
            dataServiceMock.Setup(d => d.GetEntry("post1")).Returns(entry);

            var manager = CreateManager();
            var result = manager.ApproveComment("post1", "comment1");
            Assert.Equal(CommentSaveState.Approved, result);
        }
    }
}
