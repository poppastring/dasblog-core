namespace DasBlog.Tests.FunctionalTests.ComponentTests
{
	public partial class BlogManagerTests2
	{
		private const string minimalBlogPostXml = @"
<DayEntry xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='urn:newtelligence-com:dasblog:runtime:data'>
  <Date>{0}</Date>
  <Entries>
    <Entry>
      <Content />
      <Created>{1}</Created>
      <Modified>{2}</Modified>
      <EntryId>{3}</EntryId>
      <Description />
      <Title>No Title</Title>
      <Categories />
      <IsPublic>true</IsPublic>
      <Syndicated>true</Syndicated>
      <ShowOnFrontPage>true</ShowOnFrontPage>
      <AllowComments>true</AllowComments>
      <Attachments />
      <Crossposts />
      <Latitude xsi:nil='true' />
      <Longitude xsi:nil='true' />
    </Entry>
  </Entries>
</DayEntry>
";
	}
}
