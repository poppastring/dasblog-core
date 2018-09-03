using DasBlog.Web;
using DasBlog.Core.Configuration;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.IO;
using System.Xml.Serialization;
using Xunit;

namespace DasBlog.Tests.UnitTests.Managers
{
    public class SiteSecurityManagerTest
    {
        /// <summary>
        /// All test methods should follow this naming pattern
        /// </summary>
        [Fact]
        public void UnitOfWork_StateUnderTest_ExpectedBehavior()
        {

        }

        [Fact]
        public void ValidateContributor_RoleAsAdmin_ReturnTrue()
        {

        }
    }
}
