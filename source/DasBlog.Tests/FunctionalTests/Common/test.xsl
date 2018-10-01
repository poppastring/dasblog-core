<?xml version='1.0' encoding='utf-8'?>
<xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform'>
    <xsl:output indent='yes' method='xml' xml:space='default' encoding='utf-8' omit-xml-declaration='yes'/>

    <xsl:template match='@*|node()'>
        <xsl:copy>
            <xsl:apply-templates select='node()|@*'/>
        </xsl:copy>
    </xsl:template>

    <xsl:template match='/SiteConfig/Root'/>

    <xsl:template match='/SiteConfig'>
        <xsl:copy>
            <xsl:element name='Root'>abc</xsl:element>
            <xsl:apply-templates/>
        </xsl:copy>
    </xsl:template>
</xsl:stylesheet>
