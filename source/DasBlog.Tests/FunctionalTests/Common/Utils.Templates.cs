namespace DasBlog.Tests.FunctionalTests.Common
{
	internal static partial class Utils
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
		private const string minimalComment = @"
<DayExtra xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='urn:newtelligence-com:dasblog:runtime:data'>
  <Date>2018-02-24T00:00:00+00:00</Date>
  <Comments>
    <Comment>
      <Content>I am the Architect. I created the Matrix. I've been waiting for you. You have many questions, and although the process has altered your consciousness, you remain irrevocably human. Ergo, some of my answers you will understand, and some of them you will not. Concordantly, while your first question may be the most pertinent, you may or may not realize it is also the most irrelevant.</Content>
      <Created>2018-02-25T00:00:00+00:00</Created>
      <Modified>2018-02-25T00:00:00+00:00</Modified>
      <EntryId>3a162ecd-982d-4277-b4c7-204e547df749</EntryId>
      <TargetTitle>Congratulations, you've installed dasBlog!</TargetTitle>
      <TargetEntryId>b705c37b-b47f-4e8d-8f8b-091efc4cb684</TargetEntryId>
      <Author>The Architect</Author>
      <AuthorEmail>architect@matrix.com</AuthorEmail>
      <AuthorHomepage>http://www.poppastring.com/blog</AuthorHomepage>
      <AuthorIPAddress>1.1.1.1</AuthorIPAddress>
      <IsPublic>true</IsPublic>
      <OpenId>false</OpenId>
      <SpamState>NotChecked</SpamState>
      <AuthorUserAgent>Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Safari/537.36 Edge/13.10586</AuthorUserAgent>
    </Comment>
  </Comments>
  <Trackings />
</DayExtra>
";
	}
}
