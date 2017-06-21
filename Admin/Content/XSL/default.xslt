<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="html" indent="no"/>

    <xsl:template match="/*">
      <table class="table" style="width:80%">
        <xsl:apply-templates select="child::node()"/>
      </table>
    </xsl:template>
  <xsl:template match="* | @*" priority="-1">
    <tr>
      <th>
        <xsl:value-of select="local-name()"/>
      </th>
      <td>
        <xsl:choose>
          <xsl:when test="count(*) = 0">
            <xsl:value-of select="text()"/>
          </xsl:when>
          <xsl:otherwise>
            <table>
              <xsl:apply-templates/>
            </table>
          </xsl:otherwise>
        </xsl:choose>
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>
