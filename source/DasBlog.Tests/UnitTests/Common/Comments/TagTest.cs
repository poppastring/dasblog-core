using DasBlog.Core.Common.Comments;
using Xunit;

namespace DasBlog.Tests.UnitTests.Common.Comments
{
    public class TagTest
    {
        [Fact]
        public void Attributes_SetWithCommaSeparatedValues_PopulatesIsValidAttribute()
        {
            // Simulates what happens during XML deserialization from site.config
            var tag = new Tag();
            tag.Name = "a";
            tag.Attributes = "href,title";
            tag.Allowed = true;

            Assert.True(tag.IsValidAttribute("href"));
            Assert.True(tag.IsValidAttribute("title"));
        }

        [Fact]
        public void Attributes_SetWithCommaSeparatedValues_RoundTripsCorrectly()
        {
            var tag = new Tag();
            tag.Attributes = "href,title";

            Assert.Equal("href,title", tag.Attributes);
        }

        [Fact]
        public void Attributes_SetWithSingleValue_IsValid()
        {
            var tag = new Tag();
            tag.Attributes = "href";

            Assert.True(tag.IsValidAttribute("href"));
            Assert.Equal("href", tag.Attributes);
        }

        [Fact]
        public void Attributes_SetNull_ResultsInNoValidAttributes()
        {
            var tag = new Tag();
            tag.Attributes = null;

            Assert.False(tag.IsValidAttribute("href"));
            Assert.Equal("", tag.Attributes);
        }

        [Fact]
        public void Attributes_SetEmpty_ResultsInNoValidAttributes()
        {
            var tag = new Tag();
            tag.Attributes = "";

            Assert.False(tag.IsValidAttribute("href"));
            Assert.Equal("", tag.Attributes);
        }

        [Fact]
        public void IsValidAttribute_RejectsUnlistedAttribute()
        {
            var tag = new Tag();
            tag.Attributes = "href,title";

            Assert.False(tag.IsValidAttribute("onclick"));
            Assert.False(tag.IsValidAttribute("style"));
        }

        [Fact]
        public void AttributesArray_StaysSyncedWithAttributes()
        {
            var tag = new Tag();
            tag.Attributes = "href,title";

            Assert.Equal("href,title", tag.AttributesArray);
        }

        [Fact]
        public void Constructor_WithTagDefinition_SetsAttributesCorrectly()
        {
            // Tags defined as "tagname@att1@att2"
            var tag = new Tag("a@href@title");

            Assert.Equal("a", tag.Name);
            Assert.True(tag.IsValidAttribute("href"));
            Assert.True(tag.IsValidAttribute("title"));
        }

        [Fact]
        public void Constructor_WithTagDefinitionNoAttributes_HasNoValidAttributes()
        {
            var tag = new Tag("b");

            Assert.Equal("b", tag.Name);
            Assert.False(tag.IsValidAttribute("href"));
        }

        [Fact]
        public void XmlDeserialization_PreservesAttributes()
        {
            // Simulate full XML round-trip as done by site.config loading
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ValidCommentTags));
            var xml = @"<ValidCommentTags name=""0"">
                <tag name=""a"" attributes=""href,title"" allowed=""true"" />
                <tag name=""b"" allowed=""true"" />
            </ValidCommentTags>";

            using var reader = new System.IO.StringReader(xml);
            var validTags = (ValidCommentTags)serializer.Deserialize(reader);

            var anchorTag = validTags.Tag.Find(t => t.Name == "a");
            Assert.NotNull(anchorTag);
            Assert.True(anchorTag.Allowed);
            Assert.True(anchorTag.IsValidAttribute("href"));
            Assert.True(anchorTag.IsValidAttribute("title"));
            Assert.False(anchorTag.IsValidAttribute("onclick"));

            var boldTag = validTags.Tag.Find(t => t.Name == "b");
            Assert.NotNull(boldTag);
            Assert.False(boldTag.IsValidAttribute("href"));
        }
    }
}
