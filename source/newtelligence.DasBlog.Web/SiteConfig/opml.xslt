<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="html" indent="no"/>
    <xsl:template match="/">
        <xsl:for-each select="opml/body/outline">
            <a href="{@xmlUrl}"><img src="images/rssButton.gif" height="9" width="19" border="0" alt="RSS Feed"/></a>
            <xsl:text disable-output-escaping="yes">   </xsl:text>
            <a href="{@htmlUrl}"><xsl:value-of select="@title"/></a><BR/>
        </xsl:for-each>
    </xsl:template>  
</xsl:stylesheet>
