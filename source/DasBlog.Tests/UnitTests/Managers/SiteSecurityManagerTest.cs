using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using DasBlog.Managers;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Core.Security;
using Moq;
using Xunit;
using DasBlog.Services;

namespace DasBlog.Tests.UnitTests.Managers
{
    public class SiteSecurityManagerTest
    {
        private Mock<IDasBlogSettings> settingsMock;
        private Mock<ISiteSecurityConfig> securityConfigMock;
        private List<User> users;

        public SiteSecurityManagerTest()
        {
            settingsMock = new Mock<IDasBlogSettings>();
            securityConfigMock = new Mock<ISiteSecurityConfig>();

            users = new List<User>
            {
                new User { DisplayName = "Test User1", EmailAddress = "test1@example.com", Password = "pw", Active = true, Role = Role.Admin },
				new User { DisplayName = "Test User2", EmailAddress = "test2@example.com", Password = "pw", Active = true, Role = Role.Contributor}
			};

            securityConfigMock.SetupGet(s => s.Users).Returns(users);
            settingsMock.SetupGet(s => s.SecurityConfiguration).Returns(securityConfigMock.Object);
        }

        private SiteSecurityManager CreateManager()
        {
            return new SiteSecurityManager(settingsMock.Object);
        }

        [Fact]
        public void HashPassword_ReturnsHashedString()
        {
            var manager = CreateManager();
            var hash = manager.HashPassword("password");
            Assert.False(string.IsNullOrEmpty(hash));
        }

        [Fact]
        public void IsMd5Hash_TrueForMd5Format()
        {
            var manager = CreateManager();
            string md5 = "A1-B2-C3-D4-E5-F6-07-18-29-3A-4B-5C-6D-7E-8F-90";
            Assert.True(manager.IsMd5Hash(md5));
        }

        [Fact]
        public void IsMd5Hash_FalseForNonMd5()
        {
            var manager = CreateManager();
            string notMd5 = "not-an-md5-hash";
            Assert.False(manager.IsMd5Hash(notMd5));
        }

        [Fact]
        public void VerifyHashedPassword_TrueForCorrectPassword()
        {
            var manager = CreateManager();
            string password = "password";
            string hash = manager.HashPassword(password);
            Assert.True(manager.VerifyHashedPassword(hash, password));
        }

        [Fact]
        public void VerifyHashedPassword_FalseForIncorrectPassword()
        {
            var manager = CreateManager();
            string password = "password";
            string hash = manager.HashPassword(password);
            Assert.False(manager.VerifyHashedPassword(hash, "wrongpassword"));
        }

        [Fact]
        public void GetUser_ReturnsUserByName()
        {
            var manager = CreateManager();
            var user = manager.GetUser("test1@example.com");
			Assert.NotNull(user);
            Assert.Equal("test1@example.com", user.Name);
        }

        [Fact]
        public void GetUserByDisplayName_ReturnsUser()
        {
            var manager = CreateManager();
            var user = manager.GetUserByDisplayName("Test User1");
            Assert.NotNull(user);
            Assert.Equal("Test User1", user.DisplayName);
        }

        [Fact]
        public void GetUserByEmail_ReturnsUser()
        {
            var manager = CreateManager();
            var user = manager.GetUserByEmail("test1@example.com");
            Assert.NotNull(user);
            Assert.Equal("test1@example.com", user.EmailAddress);
        }
    }
}
