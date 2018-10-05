<?xml version='1.0' encoding='utf-8'?>
<xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:post='urn:newtelligence-com:dasblog:runtime:data'>
    <xsl:output indent='yes' method='xml' xml:space='default' encoding='utf-8' omit-xml-declaration='yes'/>
        // we must output the xml declaration programmatically (C#) as otherwise
        // it outputs then encoding as utf-16 - I think because we use a StringWriter

    <xsl:template match='@*|node()'>
        <xsl:copy>
            <xsl:apply-templates select='node()|@*'/>
        </xsl:copy>
    </xsl:template>

    <xsl:template match='/post:DayExtra/post:Comments/post:Comment[post:EntryId="5d8c292c-ebd8-46fc-95ed-64ca5912c3fc"]/post:IsPublic'/>

    <xsl:template match='/post:DayExtra/post:Comments/post:Comment[post:EntryId="5d8c292c-ebd8-46fc-95ed-64ca5912c3fc"]'>
        <xsl:copy>
            <xsl:element name='post:IsPublic'>false</xsl:element>
            <xsl:apply-templates select='node()|@*'/>
        </xsl:copy>
    </xsl:template>
</xsl:stylesheet>
