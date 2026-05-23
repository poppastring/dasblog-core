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

        [Fact]
        public void Constructor_WithValidDependencies_ConstructsInstance()
        {
            var manager = CreateManager();
            Assert.NotNull(manager);
        }

        [Fact]
        public void GetUser_NonExistingUser_ReturnsNull()
        {
            var manager = CreateManager();
            var user = manager.GetUser("nonexistent@example.com");
            Assert.Null(user);
        }

        private static string ComputeLegacyMd5(string password)
        {
            using var md5 = MD5.Create();
            return BitConverter.ToString(md5.ComputeHash(Encoding.Unicode.GetBytes(password)));
        }

        private static string ComputeLegacySha512(string password)
        {
            using var sha = SHA512.Create();
            return BitConverter.ToString(sha.ComputeHash(Encoding.Unicode.GetBytes(password)));
        }

        [Fact]
        public void IsLegacyHash_TrueForMd5FormattedHash()
        {
            var hash = ComputeLegacyMd5("password");
            Assert.True(SiteSecurityManager.IsLegacyHash(hash, out var algorithm));
            Assert.IsAssignableFrom<MD5>(algorithm);
            algorithm?.Dispose();
        }

        [Fact]
        public void IsLegacyHash_TrueForSha512FormattedHash()
        {
            var hash = ComputeLegacySha512("password");
            Assert.True(SiteSecurityManager.IsLegacyHash(hash, out var algorithm));
            Assert.IsAssignableFrom<SHA512>(algorithm);
            algorithm?.Dispose();
        }

        [Fact]
        public void IsLegacyHash_FalseForNonHexStringOfMd5Length()
        {
            // 47-char string (MD5 legacy length) but containing non-hex chars must not be treated as legacy.
            string fakeMd5Length = new string('Z', 47);
            Assert.False(SiteSecurityManager.IsLegacyHash(fakeMd5Length, out var algorithm));
            Assert.Null(algorithm);
        }

        [Fact]
        public void IsLegacyHash_FalseForIdentityHash()
        {
            var manager = CreateManager();
            var identityHash = manager.HashPassword("password");
            Assert.False(SiteSecurityManager.IsLegacyHash(identityHash, out var algorithm));
            Assert.Null(algorithm);
        }

        [Fact]
        public void IsLegacyHash_FalseForNullOrEmpty()
        {
            Assert.False(SiteSecurityManager.IsLegacyHash(null, out var a1));
            Assert.Null(a1);
            Assert.False(SiteSecurityManager.IsLegacyHash(string.Empty, out var a2));
            Assert.Null(a2);
        }

        [Fact]
        public void VerifyHashedPassword_TrueForLegacyMd5Hash()
        {
            var manager = CreateManager();
            var legacy = ComputeLegacyMd5("password");
            Assert.True(manager.VerifyHashedPassword(legacy, "password"));
        }

        [Fact]
        public void VerifyHashedPassword_FalseForLegacyMd5WithWrongPassword()
        {
            var manager = CreateManager();
            var legacy = ComputeLegacyMd5("password");
            Assert.False(manager.VerifyHashedPassword(legacy, "wrongpassword"));
        }

        [Fact]
        public void VerifyHashedPassword_TrueForLegacySha512Hash()
        {
            var manager = CreateManager();
            var legacy = ComputeLegacySha512("password");
            Assert.True(manager.VerifyHashedPassword(legacy, "password"));
        }

        [Fact]
        public void VerifyHashedPassword_FalseForLegacySha512WithWrongPassword()
        {
            var manager = CreateManager();
            var legacy = ComputeLegacySha512("password");
            Assert.False(manager.VerifyHashedPassword(legacy, "wrongpassword"));
        }

        [Fact]
        public void VerifyHashedPassword_MigrationFromLegacyMd5ToIdentityHash()
        {
            var manager = CreateManager();
            const string password = "password";

            // Old stored hash verifies successfully.
            var legacyHash = ComputeLegacyMd5(password);
            Assert.True(manager.VerifyHashedPassword(legacyHash, password));

            // Simulate upgrade: replace the stored hash with a new Identity hash.
            var upgradedHash = manager.HashPassword(password);
            Assert.NotEqual(legacyHash, upgradedHash);
            Assert.False(SiteSecurityManager.IsLegacyHash(upgradedHash, out _));

            // New hash still verifies the same password, and rejects wrong ones.
            Assert.True(manager.VerifyHashedPassword(upgradedHash, password));
            Assert.False(manager.VerifyHashedPassword(upgradedHash, "wrongpassword"));
        }

        [Fact]
        public void VerifyHashedPassword_MigrationFromLegacySha512ToIdentityHash()
        {
            var manager = CreateManager();
            const string password = "password";

            var legacyHash = ComputeLegacySha512(password);
            Assert.True(manager.VerifyHashedPassword(legacyHash, password));

            var upgradedHash = manager.HashPassword(password);
            Assert.False(SiteSecurityManager.IsLegacyHash(upgradedHash, out _));
            Assert.True(manager.VerifyHashedPassword(upgradedHash, password));
        }
    }
}
