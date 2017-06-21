<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
    <xsl:output method="html" indent="yes"/>

    <xsl:template match="@* | node()">
      <h4>
        <span class="glyphicon glyphicon-exclamation-sign"></span>
        Program Exception
      </h4>
      <p style="clear:both">
        This object represents a program exception that is related to event that caused this audit. Usually exceptions are tied to 
        a failed operation. This text can be useful for diagnosing system issues. The reason exceptions are included in these audits
        are they provide more details as to the nature of the failure.
      </p>
      <div class="container-fluid">
        <div class="row">
          <div class="col-md-12">
            <strong>Message:</strong>
            <xsl:value-of select="//Message"/>
          </div>
        </div>
        <div class="row">
          <div class="col-md-12">
            <strong>Source:</strong>
            <xsl:value-of select="//Source"/>
          </div>
        </div>
        <div class="row">
          <div class="col-md-12">
            <pre class="pre-scrollable">
              <xsl:value-of select="//StackTrace"/>
            </pre>
          </div>
        </div>
      </div>
    </xsl:template>
</xsl:stylesheet>
