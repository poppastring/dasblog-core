using System;
using System.Threading;
using System.Threading.Tasks;
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
        private Mock<ISpamBlockingService> spamBlockingServiceMock;

        public CommentManagerTest()
        {
            settingsMock = new Mock<IDasBlogSettings>();
            siteConfigMock = new Mock<ISiteConfig>();
            siteConfigMock.SetupAllProperties();
            settingsMock.Setup(s => s.SiteConfiguration).Returns(siteConfigMock.Object);

            loggerMock = new Mock<ILogger<CommentManager>>();
            dataServiceMock = new Mock<IBlogDataService>();
            spamBlockingServiceMock = new Mock<ISpamBlockingService>();
        }

        private CommentManager CreateManager()
        {
            return new CommentManager(loggerMock.Object, settingsMock.Object, dataServiceMock.Object, spamBlockingServiceMock.Object);
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

        // --- Spam blocking integration ---

        private void ConfigureCommentsEnabled(int daysAllowed = 30)
        {
            siteConfigMock.Object.EnableComments = true;
            siteConfigMock.Object.DaysCommentsAllowed = daysAllowed;
            siteConfigMock.Object.SendCommentsByEmail = false;
        }

        private static Entry CreateEntry()
        {
            return new Entry
            {
                Title = "Test Entry",
                AllowComments = true,
                CreatedUtc = DateTime.UtcNow
            };
        }

        private static Comment CreateIncomingComment(SpamState state = SpamState.NotChecked)
        {
            return new Comment
            {
                Author = "Visitor",
                Content = "Hello",
                TargetTitle = "Test Entry",
                SpamState = state,
                IsPublic = true
            };
        }

        [Fact]
        public async Task AddComment_SpamServiceEnabledAndIsSpam_HoldsForModeration()
        {
            ConfigureCommentsEnabled();
            var entry = CreateEntry();
            dataServiceMock.Setup(d => d.GetEntry("post1")).Returns(entry);
            settingsMock.Setup(s => s.FilterHtml(It.IsAny<string>())).Returns<string>(s => s);
            spamBlockingServiceMock.SetupGet(s => s.IsEnabled).Returns(true);
            spamBlockingServiceMock.Setup(s => s.IsSpamAsync(It.IsAny<IFeedback>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var incoming = CreateIncomingComment();
            var manager = CreateManager();

            var result = await manager.AddCommentAsync("post1", incoming);

            Assert.Equal(CommentSaveState.Added, result);
            Assert.Equal(SpamState.Spam, incoming.SpamState);
            Assert.False(incoming.IsPublic);
            dataServiceMock.Verify(d => d.AddComment(incoming), Times.Once);
        }

        [Fact]
        public async Task AddComment_AdminAuthoredComment_SkipsSpamCheck()
        {
            ConfigureCommentsEnabled();
            var entry = CreateEntry();
            dataServiceMock.Setup(d => d.GetEntry("post1")).Returns(entry);
            settingsMock.Setup(s => s.FilterHtml(It.IsAny<string>())).Returns<string>(s => s);
            spamBlockingServiceMock.SetupGet(s => s.IsEnabled).Returns(true);

            var incoming = CreateIncomingComment(SpamState.NotSpam);
            var manager = CreateManager();

            await manager.AddCommentAsync("post1", incoming);

            spamBlockingServiceMock.Verify(s => s.IsSpamAsync(It.IsAny<IFeedback>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.Equal(SpamState.NotSpam, incoming.SpamState);
        }

        [Fact]
        public async Task AddComment_SpamServiceDisabled_DoesNotCallIsSpam()
        {
            ConfigureCommentsEnabled();
            var entry = CreateEntry();
            dataServiceMock.Setup(d => d.GetEntry("post1")).Returns(entry);
            settingsMock.Setup(s => s.FilterHtml(It.IsAny<string>())).Returns<string>(s => s);
            spamBlockingServiceMock.SetupGet(s => s.IsEnabled).Returns(false);

            var incoming = CreateIncomingComment();
            var manager = CreateManager();

            await manager.AddCommentAsync("post1", incoming);

            spamBlockingServiceMock.Verify(s => s.IsSpamAsync(It.IsAny<IFeedback>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.Equal(SpamState.NotChecked, incoming.SpamState);
        }

        [Fact]
        public void DeleteComment_SpamCommentAndServiceEnabled_ReportsSpam()
        {
            var entry = new Entry { Title = "Test Entry" };
            dataServiceMock.Setup(d => d.GetEntry("post1")).Returns(entry);
            var existing = new Comment { SpamState = SpamState.Spam };
            dataServiceMock.Setup(d => d.GetCommentById("post1", "comment1")).Returns(existing);
            spamBlockingServiceMock.SetupGet(s => s.IsEnabled).Returns(true);

            var manager = CreateManager();
            var result = manager.DeleteComment("post1", "comment1");

            Assert.Equal(CommentSaveState.Deleted, result);
            spamBlockingServiceMock.Verify(s => s.ReportSpam(existing), Times.Once);
            dataServiceMock.Verify(d => d.DeleteComment("post1", "comment1"), Times.Once);
        }

        [Fact]
        public void DeleteComment_NonSpamComment_DoesNotReportSpam()
        {
            var entry = new Entry { Title = "Test Entry" };
            dataServiceMock.Setup(d => d.GetEntry("post1")).Returns(entry);
            var existing = new Comment { SpamState = SpamState.NotSpam };
            dataServiceMock.Setup(d => d.GetCommentById("post1", "comment1")).Returns(existing);
            spamBlockingServiceMock.SetupGet(s => s.IsEnabled).Returns(true);

            var manager = CreateManager();
            manager.DeleteComment("post1", "comment1");

            spamBlockingServiceMock.Verify(s => s.ReportSpam(It.IsAny<IFeedback>()), Times.Never);
        }

        [Fact]
        public void ApproveComment_PreviouslyFlaggedSpam_ReportsNotSpam()
        {
            var entry = new Entry { Title = "Test Entry" };
            dataServiceMock.Setup(d => d.GetEntry("post1")).Returns(entry);
            var existing = new Comment { SpamState = SpamState.Spam };
            dataServiceMock.Setup(d => d.GetCommentById("post1", "comment1")).Returns(existing);
            spamBlockingServiceMock.SetupGet(s => s.IsEnabled).Returns(true);

            var manager = CreateManager();
            var result = manager.ApproveComment("post1", "comment1");

            Assert.Equal(CommentSaveState.Approved, result);
            spamBlockingServiceMock.Verify(s => s.ReportNotSpam(existing), Times.Once);
        }

        [Fact]
        public void ApproveComment_NotPreviouslyFlaggedSpam_DoesNotReportNotSpam()
        {
            var entry = new Entry { Title = "Test Entry" };
            dataServiceMock.Setup(d => d.GetEntry("post1")).Returns(entry);
            var existing = new Comment { SpamState = SpamState.NotChecked };
            dataServiceMock.Setup(d => d.GetCommentById("post1", "comment1")).Returns(existing);
            spamBlockingServiceMock.SetupGet(s => s.IsEnabled).Returns(true);

            var manager = CreateManager();
            manager.ApproveComment("post1", "comment1");

            spamBlockingServiceMock.Verify(s => s.ReportNotSpam(It.IsAny<IFeedback>()), Times.Never);
        }

        [Fact]
        public void MarkCommentAsSpam_ServiceEnabled_ReportsSpamAndMarksLocally()
        {
            var entry = new Entry { Title = "Test Entry" };
            dataServiceMock.Setup(d => d.GetEntry("post1")).Returns(entry);
            var existing = new Comment { SpamState = SpamState.NotChecked };
            dataServiceMock.Setup(d => d.GetCommentById("post1", "comment1")).Returns(existing);
            spamBlockingServiceMock.SetupGet(s => s.IsEnabled).Returns(true);

            var manager = CreateManager();
            var result = manager.MarkCommentAsSpam("post1", "comment1");

            Assert.Equal(CommentSaveState.Unapproved, result);
            spamBlockingServiceMock.Verify(s => s.ReportSpam(existing), Times.Once);
            dataServiceMock.Verify(d => d.MarkCommentAsSpam("post1", "comment1"), Times.Once);
        }

        [Fact]
        public void MarkCommentAsSpam_ServiceDisabled_StillMarksLocally()
        {
            var entry = new Entry { Title = "Test Entry" };
            dataServiceMock.Setup(d => d.GetEntry("post1")).Returns(entry);
            spamBlockingServiceMock.SetupGet(s => s.IsEnabled).Returns(false);

            var manager = CreateManager();
            var result = manager.MarkCommentAsSpam("post1", "comment1");

            Assert.Equal(CommentSaveState.Unapproved, result);
            spamBlockingServiceMock.Verify(s => s.ReportSpam(It.IsAny<IFeedback>()), Times.Never);
            dataServiceMock.Verify(d => d.MarkCommentAsSpam("post1", "comment1"), Times.Once);
        }

        [Fact]
        public void MarkCommentAsSpam_InvalidEntry_ReturnsNotFound()
        {
            dataServiceMock.Setup(d => d.GetEntry("nonexistent")).Returns((Entry)null);

            var manager = CreateManager();
            var result = manager.MarkCommentAsSpam("nonexistent", "comment1");

            Assert.Equal(CommentSaveState.NotFound, result);
            dataServiceMock.Verify(d => d.MarkCommentAsSpam(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
