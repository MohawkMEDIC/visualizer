<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="html" indent="no" />

    <xsl:template match="/Request">
      <div class="container-fluid">
        <div class="row" style="padding-bottom:4px;">
          <div class="col-md-12">
            <span class="badge badge-primary">
              <xsl:value-of select="substring-before(., ' ')" />
            </span>
            <xsl:value-of select="substring-before(substring-after(., ' '), 'HTTP')" />
          </div>
        </div>
        <div class="row">
          <div class="col-md-12">
            <pre class="pre-scrollable">
              <xsl:value-of select="." />
            </pre>
          </div>
        </div>
      </div>
    </xsl:template>
</xsl:stylesheet>