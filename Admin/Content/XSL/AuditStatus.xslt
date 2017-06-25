<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="html" indent="no" />

    <xsl:template match="/AuditStatus">
      <p>Audit <xsl:value-of select="//AuditId" />'s status code was set to
      <span class="label label-primary">
        <xsl:choose>
          <xsl:when test="//StatusCodeId = 0">
            <span class="glyphicon glyphicon-certificate"></span> NEW
          </xsl:when>
          <xsl:when test="//StatusCodeId = 1">
            <span class="glyphicon glyphicon-eye-open"></span> ACTIVE
          </xsl:when>
          <xsl:when test="//StatusCodeId = 2">
            <span class="glyphicon glyphicon-star-empty"></span> STARRED
          </xsl:when>
          <xsl:when test="//StatusCodeId = 3">
            UNONE
          </xsl:when>
          <xsl:when test="//StatusCodeId = 4">
            <span class="glyphicon glyphicon-trash"></span> DELETED
          </xsl:when>
          <xsl:when test="//StatusCodeId = 5">
            <span class="glyphicon glyphicon-briefcase"></span> ARCHIVED
          </xsl:when>
          <xsl:when test="//StatusCodeId = 6">
            <span class="glyphicon glyphicon-cog"></span> SYSTEM
          </xsl:when>
        </xsl:choose>
      </span>
    </p>
    </xsl:template>
</xsl:stylesheet>