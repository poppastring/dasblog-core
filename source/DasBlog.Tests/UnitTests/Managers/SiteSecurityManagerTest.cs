using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using DasBlog.Managers;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.Users;
using DasBlog.Core.Security;
using DasBlog.Managers.Interfaces;
using DasBlog.Web.Identity;
using Microsoft.AspNetCore.Identity;
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
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(service => service.GetAllUsers()).Returns(users);
            return new SiteSecurityManager(settingsMock.Object, new PasswordHasher<object>(), userServiceMock.Object);
        }

        [Fact]
        public void HashPassword_ReturnsHashedString()
        {
            var manager = CreateManager();
            var hash = manager.HashPassword("password");
            Assert.False(string.IsNullOrEmpty(hash));
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
        public void VerifyAppPassword_ValidToken_PersistsLastUsedDetails()
        {
            const string token = "dbapp-test-token";
            var passwordHasher = new PasswordHasher<object>();
            var user = users[0];
            user.AppPasswords = new List<AppPassword>
            {
                new AppPassword { Id = "app-id", Name = "Open Live Writer", Hash = passwordHasher.HashPassword(null, token) }
            };
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(service => service.GetAllUsers()).Returns(users);

            var manager = new SiteSecurityManager(settingsMock.Object, passwordHasher, userServiceMock.Object);
            var result = manager.VerifyAppPassword(user, token, "192.0.2.1");

            Assert.True(result);
            Assert.False(string.IsNullOrEmpty(user.AppPasswords[0].LastUsed));
            Assert.Equal("192.0.2.1", user.AppPasswords[0].LastIp);
            userServiceMock.Verify(service => service.SaveUsers(users), Times.Once);
        }

        [Fact]
        public void VerifyAppPassword_InvalidToken_DoesNotSaveUsage()
        {
            var passwordHasher = new PasswordHasher<object>();
            var user = users[0];
            user.AppPasswords = new List<AppPassword>
            {
                new AppPassword { Id = "app-id", Hash = passwordHasher.HashPassword(null, "valid-token") }
            };
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(service => service.GetAllUsers()).Returns(users);

            var manager = new SiteSecurityManager(settingsMock.Object, passwordHasher, userServiceMock.Object);

            Assert.False(manager.VerifyAppPassword(user, "invalid-token", "192.0.2.1"));
            userServiceMock.Verify(service => service.SaveUsers(It.IsAny<List<User>>()), Times.Never);
        }

        [Fact]
        public void AppPassword_Serialization_PreservesMetadata()
        {
            var configuration = new SiteSecurityConfigData
            {
                Users = new List<User>
                {
                    new User
                    {
                        EmailAddress = "test@example.com",
                        AppPasswords = new List<AppPassword>
                        {
                            new AppPassword
                            {
                                Id = "app-id",
                                Name = "Open Live Writer",
                                Hash = "hashed-token",
                                Created = "2026-05-26T20:00:00.0000000Z",
                                LastUsed = "2026-05-26T20:42:00.0000000Z",
                                LastIp = "192.0.2.1"
                            }
                        }
                    }
                }
            };
            var serializer = new XmlSerializer(typeof(SiteSecurityConfigData));
            string serialized;
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, configuration);
                serialized = writer.ToString();
            }

            SiteSecurityConfigData roundTripped;
            using (var reader = new StringReader(serialized))
            {
                roundTripped = (SiteSecurityConfigData)serializer.Deserialize(reader);
            }

            var appPassword = Assert.Single(Assert.Single(roundTripped.Users).AppPasswords);
            Assert.Equal("Open Live Writer", appPassword.Name);
            Assert.Equal("hashed-token", appPassword.Hash);
            Assert.Equal("2026-05-26T20:42:00.0000000Z", appPassword.LastUsed);
            Assert.Equal("192.0.2.1", appPassword.LastIp);
        }

        [Fact]
        public void LegacyUserXml_WithoutAppPasswords_LoadsWithNoAppPasswords()
        {
            const string xml = "<SiteSecurityConfig><Users><User><EmailAddress>test@example.com</EmailAddress></User></Users></SiteSecurityConfig>";
            var serializer = new XmlSerializer(typeof(SiteSecurityConfigData));
            SiteSecurityConfigData configuration;
            using (var reader = new StringReader(xml))
            {
                configuration = (SiteSecurityConfigData)serializer.Deserialize(reader);
            }

            Assert.Empty(Assert.Single(configuration.Users).AppPasswords);
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
        public void IsLegacyHash_FalseForNonHexStringOfSha512Length()
        {
            // 191-char string (SHA-512 legacy length) but containing non-hex chars must not be treated as legacy.
            string fakeSha512Length = new string('Z', 191);
            Assert.False(SiteSecurityManager.IsLegacyHash(fakeSha512Length, out var algorithm));
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
        public void IsLegacyHash_FalseForUndashedHexMd5()
        {
            // A bare 32-char hex MD5 (no dashes) is not a legacy dasBlog hash:
            // dasBlog always stored BitConverter.ToString output, which is dash-separated.
            string undashed = new string('A', 32);
            Assert.False(SiteSecurityManager.IsLegacyHash(undashed, out var algorithm));
            Assert.Null(algorithm);
        }

        [Fact]
        public void IsLegacyHash_FalseForUndashedHexSha512()
        {
            string undashed = new string('A', 128);
            Assert.False(SiteSecurityManager.IsLegacyHash(undashed, out var algorithm));
            Assert.Null(algorithm);
        }

        [Fact]
        public void IsLegacyHash_InstanceMethod_DelegatesToStatic()
        {
            var manager = CreateManager();
            var legacyMd5 = ComputeLegacyMd5("password");
            var legacySha = ComputeLegacySha512("password");
            var identity = manager.HashPassword("password");

            Assert.True(manager.IsLegacyHash(legacyMd5));
            Assert.True(manager.IsLegacyHash(legacySha));
            Assert.False(manager.IsLegacyHash(identity));
            Assert.False(manager.IsLegacyHash(null));
            Assert.False(manager.IsLegacyHash(string.Empty));
        }

        [Fact]
        public void IsLegacyHash_TrueForLowerCaseHexMd5()
        {
            var hash = ComputeLegacyMd5("password").ToLowerInvariant();
            Assert.True(SiteSecurityManager.IsLegacyHash(hash, out var algorithm));
            Assert.IsAssignableFrom<MD5>(algorithm);
            algorithm?.Dispose();
        }

        [Fact]
        public void IsLegacyHash_TrueForLowerCaseHexSha512()
        {
            var hash = ComputeLegacySha512("password").ToLowerInvariant();
            Assert.True(SiteSecurityManager.IsLegacyHash(hash, out var algorithm));
            Assert.IsAssignableFrom<SHA512>(algorithm);
            algorithm?.Dispose();
        }

        [Fact]
        public void VerifyHashedPassword_FalseForNullHashedPassword()
        {
            var manager = CreateManager();
            Assert.False(manager.VerifyHashedPassword(null, "password"));
        }

        [Fact]
        public void VerifyHashedPassword_FalseForEmptyHashedPassword()
        {
            var manager = CreateManager();
            Assert.False(manager.VerifyHashedPassword(string.Empty, "password"));
        }

        [Fact]
        public void VerifyHashedPassword_FalseForNullProvidedPassword()
        {
            var manager = CreateManager();
            var hash = manager.HashPassword("password");
            Assert.False(manager.VerifyHashedPassword(hash, null));
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

        [Fact]
        public void DasBlogPasswordHasher_ReturnsSuccessRehashNeededForLegacyMd5()
        {
            var manager = CreateManager();
            var hasher = new DasBlogPasswordHasher(manager);
            var legacyHash = ComputeLegacyMd5("password");

            var result = hasher.VerifyHashedPassword(new DasBlogUser(), legacyHash, "password");

            Assert.Equal(PasswordVerificationResult.SuccessRehashNeeded, result);
        }

        [Fact]
        public void DasBlogPasswordHasher_ReturnsSuccessRehashNeededForLegacySha512()
        {
            var manager = CreateManager();
            var hasher = new DasBlogPasswordHasher(manager);
            var legacyHash = ComputeLegacySha512("password");

            var result = hasher.VerifyHashedPassword(new DasBlogUser(), legacyHash, "password");

            Assert.Equal(PasswordVerificationResult.SuccessRehashNeeded, result);
        }

        [Fact]
        public void DasBlogPasswordHasher_ReturnsFailedForLegacyHashWithWrongPassword()
        {
            var manager = CreateManager();
            var hasher = new DasBlogPasswordHasher(manager);
            var legacyHash = ComputeLegacyMd5("password");

            var result = hasher.VerifyHashedPassword(new DasBlogUser(), legacyHash, "wrongpassword");

            Assert.Equal(PasswordVerificationResult.Failed, result);
        }

        [Fact]
        public void DasBlogPasswordHasher_ReturnsSuccessForIdentityHash()
        {
            var manager = CreateManager();
            var hasher = new DasBlogPasswordHasher(manager);
            var identityHash = manager.HashPassword("password");

            var result = hasher.VerifyHashedPassword(new DasBlogUser(), identityHash, "password");

            Assert.Equal(PasswordVerificationResult.Success, result);
        }
    }
}
