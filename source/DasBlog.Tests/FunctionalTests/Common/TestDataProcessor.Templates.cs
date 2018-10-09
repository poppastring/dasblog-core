namespace DasBlog.Tests.FunctionalTests.Common
{
	public partial class TestDataProcesor
	{
		/// <summary>
		/// {0} = key within SiteConfige.g. NotificationEMailAddress
		/// {1} = value to be inserted e.g. "myemail@email.com"
		/// does not handle compound config values such as Ping Services
		/// </summary>
		private const string siteConfigTrasnsform = @"
<?xml version='1.0' encoding='utf-8'?>
<xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform'>
    <xsl:output indent='yes' method='xml' xml:space='default' encoding='utf-8' omit-xml-declaration='yes'/>
    
    <xsl:template match='@*|node()'>
        <xsl:copy>
            <xsl:apply-templates select='node()|@*'/>
        </xsl:copy>
    </xsl:template>
    
    <xsl:template match='/SiteConfig/{0}'/>

    <xsl:template match='/SiteConfig'>
        <xsl:copy>
            <xsl:element name='{0}'>{1}</xsl:element>
            <xsl:apply-templates select='node()|@*'/>
        </xsl:copy>
    </xsl:template>
 </xsl:stylesheet>
";

		private const string siteSecurityConfigTransform = @"
<?xml version='1.0' encoding='utf-8'?>
<xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform'>
    <xsl:output indent='yes' method='xml' xml:space='default' encoding='utf-8' omit-xml-declaration='yes'/>

    <xsl:template match='@*|node()'>
        <xsl:copy>
            <xsl:apply-templates select='node()|@*'/>
        </xsl:copy>
    </xsl:template>

    <xsl:template match='/SiteSecurityConfig/Users/User[EmailAddress=""{0}""]'>
        <xsl:copy>
            <xsl:element name='{1}'>{2}</xsl:element>
            <xsl:apply-templates select='node()|@*'/>
        </xsl:copy>
    </xsl:template>

    <xsl:template match='/SiteSecurityConfig/Users/User[EmailAddress=""{0}""]/{1}'/>
</xsl:stylesheet>
";

		private const string dayExtraTransform = @"
<?xml version='1.0' encoding='utf-8'?>
<xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:post='urn:newtelligence-com:dasblog:runtime:data'>
    <xsl:output indent='yes' method='xml' xml:space='default' encoding='utf-8' omit-xml-declaration='yes'/>
        <!-- we must output the xml declaration programmatically (C#) as otherwise
         it outputs then encoding as utf-16 - I think because we use a StringWriter -->

    <xsl:template match='@*|node()'>
        <xsl:copy>
            <xsl:apply-templates select='node()|@*'/>
        </xsl:copy>
    </xsl:template>

    <xsl:template match='/post:DayExtra/post:Comments/post:Comment[post:EntryId=""{0}""]/post:{1}'/>

	<xsl:template match='/post:DayExtra/post:Comments/post:Comment[post:EntryId=""{0}""]'>
		<xsl:copy>
			<xsl:element name='post:{1}'>{2}</xsl:element>
			<xsl:apply-templates select='node()|@*'/>
		</xsl:copy>
	</xsl:template>
</xsl:stylesheet>
";
	}
}
