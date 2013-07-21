<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
                xmlns:xaml="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                xmlns:sdk="http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk"
>
  <xsl:output method="xml" indent="no"/>

  <xsl:template match="HL7v2xConformanceProfile">
    <ArticleCollection>
      <xsl:apply-templates/>
    </ArticleCollection>
  </xsl:template>

  <xsl:template name="usageDesc">
    <xsl:param name="code"/>
    <xsl:choose>
      <xsl:when test="$code = 'R'">
        <Span xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
          <Run>R - Required</Run><LineBreak/>
          <Italic>Comforming applications must populate this element</Italic>
      
      </Span>
      </xsl:when>
      <xsl:when test="$code = 'X'">
        <Span xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
          <Run>X - Restricted</Run><LineBreak/>
          <Italic>Comforming applications will not send this element.</Italic>
        </Span>
      </xsl:when>
      <xsl:when test="$code = 'O'">
        <Span xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
          <Run>O - Optional</Run><LineBreak/>
          <Italic>Conforming applications may send this element however it may not be populated. Conformance may not be tested on these fields</Italic>
        </Span>
      </xsl:when>
      <xsl:when test="$code = 'C'">
        <Span xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
          <Run>C - Conditional</Run><LineBreak/>
          <Italic>Usage has an associated condition predicate which, if true, conformant applications must send.</Italic>
        
      </Span>
      </xsl:when>
      <xsl:when test="$code = 'CE'">
        <Span xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
          <Run>CE - Conditional/Empty</Run><LineBreak/>
          <Italic>Usage has an associated condition predicate which, if true, conformant applications <Bold>may</Bold> send.</Italic>
        </Span>
      </xsl:when>
      <xsl:when test="$code = 'RE'">
        <Span xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
          <Run>RE - Required/Empty</Run><LineBreak/>
          <Italic>
            Conforming applications must be sent if there is relevant data but may remain empty if no relevant data is found
          </Italic>
        
      </Span>
      </xsl:when>
      <xsl:otherwise>
        <Span xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
          <Run>Other</Run>
      </Span>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  
  <xsl:template match="HL7v2xStaticDef">
    <xsl:variable name="msg_id" select="concat(concat(@MsgType,'^'), @EventType)"/>
    <Article contentType="application/xaml+xml">
      <xsl:attribute name="kbid">
        <xsl:value-of select="$msg_id"/>
      </xsl:attribute>
      <Section FontFamily="Calibri" FontSize="12" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
        <xsl:attribute name="xml:space">
          <xsl:value-of select="'preserve'"/>
        </xsl:attribute>
        <Paragraph FontFamily="Arial Black" FontSize="16">
          <xsl:value-of select="@EventDesc"/> (Message)
        </Paragraph>
        <Paragraph>
          <Bold>Event:</Bold>
          <Run>
            <xsl:value-of select="@MsgType"/>^<xsl:value-of select="@EventType"/>
          </Run>
        </Paragraph>
        <Paragraph>
          <Bold>Message:</Bold>
          <Run>
            <xsl:value-of select="@MsgStructID"/>
          </Run>
        </Paragraph>
        <Paragraph>
          <Bold>HL7 Version:</Bold>
          <Run>
            <xsl:choose>
              <xsl:when test="../MetaData/@Version">
                <xsl:value-of select="../MetaData/@Version"/>
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="/HL7v2xConformanceProfile/@HL7Version"/>
              </xsl:otherwise>
            </xsl:choose>
          </Run>
        </Paragraph>
        <Paragraph>
          <Bold>Role:</Bold>
          <Run>
            <xsl:value-of select="@Role"/>
          </Run>
        </Paragraph>
        <Paragraph FontFamily="Arial Black" FontSize="14">Permitted Segments</Paragraph>

        <Paragraph>
          <InlineUIContainer>
            <Grid>
              <Grid.ColumnDefinitions>
                <ColumnDefinition Width="Auto"/>
                <ColumnDefinition Width="Auto"/>
                <ColumnDefinition Width="Auto"/>
                <ColumnDefinition Width="Auto"/>
              </Grid.ColumnDefinitions>
              <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>
                <xsl:for-each select=".//Segment">
                  <RowDefinition Height="Auto"/>
                </xsl:for-each>
              </Grid.RowDefinitions>

              <!-- Header -->
              <Border Padding="3"  Grid.Row="0" Grid.Column="0" Background="LightGray">
                <TextBlock>Segment</TextBlock>
              </Border>
              <Border Padding="3" Grid.Row="0" Grid.Column="1" Background="LightGray">
                <TextBlock>Opt</TextBlock>
              </Border>
              <Border Padding="3" Grid.Row="0" Grid.Column="2" Background="LightGray">
                <TextBlock>Mult</TextBlock>
              </Border>
              <Border Padding="3" Grid.Row="0" Grid.Column="3" Background="LightGray">
                <TextBlock>Description</TextBlock>
              </Border>
              <xsl:for-each select=".//Segment">
                <xsl:variable name="rowNumber" select="count(preceding::Segment) + 1"/>
                <Border Grid.Column="0" BorderBrush="Black" BorderThickness="1"  Padding="3" Grid.Row="{$rowNumber}">
                  <TextBlock>
                    <xsl:value-of select="@Name"/>
                  </TextBlock>
                </Border>
                <Border Grid.Column="1"  BorderBrush="Black" BorderThickness="1"  Padding="3" Grid.Row="{$rowNumber}">
                  <TextBlock>
                      <xsl:value-of select="@Usage"/>
                  </TextBlock>
                </Border>
                <Border Grid.Column="2"  BorderBrush="Black" BorderThickness="1"  Padding="3" Grid.Row="{$rowNumber}">
                  <TextBlock>
                    <xsl:value-of select="@Max"/>
                  </TextBlock>
                </Border>
                <Border Grid.Column="3"  BorderBrush="Black" BorderThickness="1"  Padding="3" Grid.Row="{$rowNumber}">
                  <TextBlock>
                    <xsl:value-of select="@LongName"/>
                  </TextBlock>
                </Border>
              </xsl:for-each>

            </Grid>
          </InlineUIContainer><LineBreak/>
          <Bold>Usage:</Bold><LineBreak/>
          <Span>R - Required</Span>
          <LineBreak/>
          <Span>X - Restricted</Span>
          <LineBreak/>
          <Span>O - Optional</Span>
          <LineBreak/>
          <Span>C - Conditional</Span>
          <LineBreak/>
          <Span>CE - Conditional/Empty</Span>
          <LineBreak/>
          <Span>RE - Required/Empty</Span>
          <LineBreak/>
        </Paragraph>
      </Section>
    </Article>
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="Segment[not(@Usage = 'X')]">
    <xsl:variable name="msg_id" select="concat(concat(//HL7v2xStaticDef/@MsgType,'^'), //HL7v2xStaticDef/@EventType)"/>
    <Article contentType="application/xaml+xml">
      <xsl:attribute name="kbid">
        <xsl:value-of select="concat(concat($msg_id,':'), @Name)"/>
      </xsl:attribute>
      <Section FontFamily="Calibri" FontSize="12"  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
        <xsl:attribute name="xml:space">
          <xsl:value-of select="'preserve'"/>
        </xsl:attribute>
        <Paragraph FontFamily="Arial Black" FontSize="16">
          <xsl:value-of select="@LongName"/> (Segment)
        </Paragraph>
        <Paragraph>
          <Bold>Segment:</Bold>
          <Run>
            <xsl:value-of select="@Name"/>
          </Run>
        </Paragraph>
        <Paragraph>
          <Bold>Use:</Bold>
            <xsl:call-template name="usageDesc">
              <xsl:with-param name="code" select="@Usage"/>
            </xsl:call-template>
        </Paragraph>
        <Paragraph>
          <Bold>Multiplicity:</Bold>
          <Run>
            <xsl:value-of select="@Min"/> .. <xsl:value-of select="@Max"/>
          </Run>
        </Paragraph>
        <xsl:if test="./Field">

          <Paragraph FontFamily="Arial Black" FontSize="14">Fields</Paragraph>

          <Paragraph>
            <InlineUIContainer>
              <Grid>
                <Grid.ColumnDefinitions>
                  <ColumnDefinition Width="Auto"/>
                  <ColumnDefinition Width="Auto"/>
                  <ColumnDefinition Width="Auto"/>
                  <ColumnDefinition Width="Auto"/>
                  <ColumnDefinition Width="Auto"/>
                  <ColumnDefinition Width="Auto"/>
                  <ColumnDefinition Width="Auto"/>
                </Grid.ColumnDefinitions>
                <Grid.RowDefinitions>
                  <RowDefinition Height="Auto"/>
                  <xsl:for-each select=".//Field">
                    <RowDefinition Height="Auto"/>
                  </xsl:for-each>
                </Grid.RowDefinitions>

                <!-- Header -->
                <Border Grid.Row="0" Padding="3" Grid.Column="0" Background="LightGray">
                  <TextBlock>Seq</TextBlock>
                </Border>
                <Border Grid.Row="0" Padding="3" Grid.Column="1" Background="LightGray">
                  <TextBlock>DT</TextBlock>
                </Border>
                <Border Grid.Row="0" Padding="3" Grid.Column="2" Background="LightGray">
                  <TextBlock>Opt</TextBlock>
                </Border>
                <Border Grid.Row="0" Padding="3" Grid.Column="3" Background="LightGray">
                  <TextBlock>Mult</TextBlock>
                </Border>
                <Border Grid.Row="0" Padding="3" Grid.Column="4" Background="LightGray">
                  <TextBlock>Len</TextBlock>
                </Border>
                <Border Grid.Row="0" Padding="3" Grid.Column="5" Background="LightGray">
                  <TextBlock>Table</TextBlock>
                </Border>
                <Border Grid.Row="0" Padding="3" Grid.Column="6" Background="LightGray">
                  <TextBlock>Name</TextBlock>
                </Border>
                <xsl:for-each select=".//Field">
                  <xsl:variable name="sequence" select="count(preceding-sibling::Field) + 1"/>
                  <Border BorderThickness="1"  Padding="3" Grid.Row="{$sequence}" Grid.Column="0" BorderBrush="Black">
                    <TextBlock>
                      <xsl:value-of select="$sequence"/>
                    </TextBlock>
                  </Border>
                  <Border BorderThickness="1"  Padding="3" Grid.Row="{$sequence}" Grid.Column="1" BorderBrush="Black">
                    <TextBlock>
                      <xsl:value-of select="@Datatype"/>
                    </TextBlock>
                  </Border>
                  <Border BorderThickness="1"  Padding="3" Grid.Row="{$sequence}" Grid.Column="2" BorderBrush="Black">
                    <TextBlock>
                        <xsl:value-of select="@Usage"/>
                    </TextBlock>
                  </Border>
                  <Border BorderThickness="1"  Padding="3" Grid.Row="{$sequence}" Grid.Column="3" BorderBrush="Black">
                    <TextBlock>
                      <xsl:value-of select="@Max"/>
                    </TextBlock>
                  </Border>
                  <Border BorderThickness="1"  Padding="3" Grid.Row="{$sequence}" Grid.Column="4" BorderBrush="Black">
                    <TextBlock>
                      <xsl:value-of select="@Length"/>
                    </TextBlock>
                  </Border>
                  <Border BorderThickness="1"  Padding="3" Grid.Row="{$sequence}" Grid.Column="5" BorderBrush="Black">
                    <TextBlock>
                      <xsl:value-of select="@Table"/>
                    </TextBlock>
                  </Border>
                  <Border BorderThickness="1"  Padding="3" Grid.Row="{$sequence}" Grid.Column="6" BorderBrush="Black">
                    <TextBlock>
                      <xsl:value-of select="@Name"/>
                    </TextBlock>
                  </Border>
                </xsl:for-each>

              </Grid>
            </InlineUIContainer><LineBreak/>
          <Bold>Usage:</Bold><LineBreak/>
          
          <Span>R - Required</Span>
          <LineBreak/>
          
          <Span>X - Restricted</Span>
          <LineBreak/>
          
          <Span>O - Optional</Span>
          <LineBreak/>
          
          <Span>C - Conditional</Span>
          <LineBreak/>
          
          <Span>CE - Conditional/Empty</Span>
          <LineBreak/>
          
          <Span>RE - Required/Empty</Span>
          
          </Paragraph>
        </xsl:if>
      </Section>
    </Article>
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="Field[not(@Usage = 'X')]">
    <xsl:variable name="msg_id" select="concat(concat(//HL7v2xStaticDef/@MsgType,'^'), //HL7v2xStaticDef/@EventType)"/>
    <xsl:variable name="segment_id" select="../@Name"/>
    <xsl:variable name="fieldNo" select="count(preceding-sibling::Field) + 1"/>
    <Article contentType="application/xaml+xml">
      <xsl:attribute name="kbid">
        <xsl:value-of select="concat(concat($msg_id,':'), $segment_id)"/>-<xsl:value-of select="$fieldNo"/>
      </xsl:attribute>
      <Section FontFamily="Calibri" FontSize="12"  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
        <xsl:attribute name="xml:space">
          <xsl:value-of select="'preserve'"/>
        </xsl:attribute>
        <Paragraph FontFamily="Arial Black" FontSize="16">
          <xsl:value-of select="@Name"/> (Field)
        </Paragraph>
        <Paragraph>
          <Bold>Path:</Bold>
          <Run>
            <xsl:value-of select="$segment_id"/>-<xsl:value-of select="$fieldNo"/>
          </Run>
        </Paragraph>
        <Paragraph>
          <Bold>Use:</Bold>
            <xsl:call-template name="usageDesc">
              <xsl:with-param name="code" select="@Usage"/>
            </xsl:call-template>
        </Paragraph>
        <Paragraph>
          <Bold>Multiplicity:</Bold>
          <Run>
            <xsl:value-of select="@Min"/> .. <xsl:value-of select="@Max"/>
          </Run>
        </Paragraph>
        <Paragraph>
          <Bold>Type:</Bold>
          <Run>
            <xsl:value-of select="@Datatype"/>
          </Run>
        </Paragraph>
        <Paragraph>
          <Bold>HL7 Chapter:</Bold>
          <Run>
            <xsl:value-of select="./Reference/text()"/>
          </Run>
        </Paragraph>
        <xsl:if test="./Component">
          <Paragraph FontFamily="Arial Black" FontSize="14">Components</Paragraph>
          <Paragraph>
            <InlineUIContainer>
              <Grid>
                <Grid.ColumnDefinitions>
                  <ColumnDefinition Width="Auto"/>
                  <ColumnDefinition Width="Auto"/>
                  <ColumnDefinition Width="Auto"/>
                  <ColumnDefinition Width="Auto"/>
                  <ColumnDefinition Width="Auto"/>
                  <ColumnDefinition Width="Auto"/>
                </Grid.ColumnDefinitions>
                <Grid.RowDefinitions>
                  <RowDefinition Height="Auto"/>
                  <xsl:for-each select="./Component">
                    <RowDefinition Height="Auto"/>
                  </xsl:for-each>
                </Grid.RowDefinitions>

                <!-- Header -->
                <Border Grid.Row="0" Padding="3" Grid.Column="0" Background="LightGray">
                  <TextBlock>Seq</TextBlock>
                </Border>
                <Border Grid.Row="0" Padding="3" Grid.Column="1" Background="LightGray">
                  <TextBlock>DT</TextBlock>
                </Border>
                <Border Grid.Row="0" Padding="3" Grid.Column="2" Background="LightGray">
                  <TextBlock>Opt</TextBlock>
                </Border>
                <Border Grid.Row="0" Padding="3" Grid.Column="3" Background="LightGray">
                  <TextBlock>Len</TextBlock>
                </Border>
                <Border Grid.Row="0" Padding="3" Grid.Column="4" Background="LightGray">
                  <TextBlock>Table</TextBlock>
                </Border>
                <Border Grid.Row="0" Padding="3" Grid.Column="5" Background="LightGray">
                  <TextBlock>Name</TextBlock>
                </Border>
                <xsl:for-each select="./Component">
                  <xsl:variable name="sequence" select="count(preceding-sibling::Component) + 1"/>
                  <Border BorderThickness="1"  Padding="3" Grid.Row="{$sequence}" Grid.Column="0" BorderBrush="Black">
                    <TextBlock>
                      <xsl:value-of select="$sequence"/>
                    </TextBlock>
                  </Border>
                  <Border BorderThickness="1"  Padding="3" Grid.Row="{$sequence}" Grid.Column="1" BorderBrush="Black">
                    <TextBlock>
                      <xsl:value-of select="@Datatype"/>
                    </TextBlock>
                  </Border>
                  <Border BorderThickness="1"  Padding="3" Grid.Row="{$sequence}" Grid.Column="2" BorderBrush="Black">
                    <TextBlock>
                        <xsl:value-of select="@Usage"/>
                    </TextBlock>
                  </Border>
                  <Border BorderThickness="1"  Padding="3" Grid.Row="{$sequence}" Grid.Column="3" BorderBrush="Black">
                    <TextBlock>
                      <xsl:value-of select="@Length"/>
                    </TextBlock>
                  </Border>
                  <Border BorderThickness="1"  Padding="3" Grid.Row="{$sequence}" Grid.Column="4" BorderBrush="Black">
                    <TextBlock>
                      <xsl:value-of select="@Table"/>
                    </TextBlock>
                  </Border>
                  <Border BorderThickness="1"  Padding="3" Grid.Row="{$sequence}" Grid.Column="5" BorderBrush="Black">
                    <TextBlock>
                      <xsl:value-of select="@Name"/>
                    </TextBlock>
                  </Border>
                </xsl:for-each>

              </Grid>
            </InlineUIContainer><LineBreak/>
          <Bold>Usage:</Bold><LineBreak/>
          
          <Span>R - Required</Span>
          <LineBreak/>
          
          <Span>X - Restricted</Span>
          <LineBreak/>
          
          <Span>O - Optional</Span>
          <LineBreak/>
          
          <Span>C - Conditional</Span>
          <LineBreak/>
          
          <Span>CE - Conditional/Empty</Span>
          <LineBreak/>
          
          <Span>RE - Required/Empty</Span>
          </Paragraph>
        </xsl:if>
      </Section>
    </Article>
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="Component[not(@Usage = 'X')]">
    <xsl:variable name="msg_id" select="concat(concat(//HL7v2xStaticDef/@MsgType,'^'), //HL7v2xStaticDef/@EventType)"/>
    <xsl:variable name="segment_id" select="../../@Name"/>
    <xsl:variable name="fieldNo" select="count(../preceding-sibling::Field) + 1"/>
    <xsl:variable name="componentNo" select="count(preceding-sibling::Component) + 1"/>
    <Article contentType="application/xaml+xml" >
      <xsl:attribute name="kbid">
        <xsl:value-of select="concat(concat($msg_id,':'), $segment_id)"/>-<xsl:value-of select="$fieldNo"/>-<xsl:value-of select="$componentNo"/>
      </xsl:attribute>
      <Section FontFamily="Calibri" FontSize="12"  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
        <xsl:attribute name="xml:space">
          <xsl:value-of select="'preserve'"/>
        </xsl:attribute>
        <Paragraph FontFamily="Arial Black" FontSize="16">
          <xsl:value-of select="@Name"/> (Component)
        </Paragraph>
        <Paragraph>
          <Bold>Path:</Bold>
          <Run>
            <xsl:value-of select="$segment_id"/>-<xsl:value-of select="$fieldNo"/>-<xsl:value-of select="$componentNo"/>
          </Run>
        </Paragraph>
        <Paragraph>
          <Bold>Use:</Bold>
            <xsl:call-template name="usageDesc">
              <xsl:with-param name="code" select="@Usage"/>
            </xsl:call-template>
        </Paragraph>
        <Paragraph>
          <Bold>Type:</Bold>
          <Run>
            <xsl:value-of select="@Datatype"/>
          </Run>
        </Paragraph>
        <Paragraph>
          <Bold>Table:</Bold>
          <Run>
              <xsl:value-of select="@Table"/>
          </Run>
        </Paragraph>
        <xsl:if test="./ImpNote">
          <Paragraph>
            <Bold>Implementation note(s):</Bold>
          <Run>
            <xsl:value-of select="./ImpNote/text()"/>
          </Run>
          </Paragraph>
        </xsl:if>
        <xsl:if test="./SubComponent">

          <Paragraph FontFamily="Arial Black" FontSize="14">Sub Components</Paragraph>

          <Paragraph>
            <InlineUIContainer>
              <Grid>
                <Grid.ColumnDefinitions>
                  <ColumnDefinition Width="Auto"/>
                  <ColumnDefinition Width="Auto"/>
                  <ColumnDefinition Width="Auto"/>
                  <ColumnDefinition Width="Auto"/>
                  <ColumnDefinition Width="Auto"/>
                  <ColumnDefinition Width="Auto"/>
                </Grid.ColumnDefinitions>
                <Grid.RowDefinitions>
                  <RowDefinition Height="Auto"/>
                  <xsl:for-each select="./SubComponent">
                    <RowDefinition Height="Auto"/>
                  </xsl:for-each>
                </Grid.RowDefinitions>

                <!-- Header -->
                <Border Grid.Row="0" Padding="3" Grid.Column="0" Background="LightGray">
                  <TextBlock>Seq</TextBlock>
                </Border>
                <Border Grid.Row="0" Padding="3" Grid.Column="1" Background="LightGray">
                  <TextBlock>DT</TextBlock>
                </Border>
                <Border Grid.Row="0" Padding="3" Grid.Column="2" Background="LightGray">
                  <TextBlock>Opt</TextBlock>
                </Border>
                <Border Grid.Row="0" Padding="3" Grid.Column="3" Background="LightGray">
                  <TextBlock>Len</TextBlock>
                </Border>
                <Border Grid.Row="0" Padding="3" Grid.Column="4" Background="LightGray">
                  <TextBlock>Table</TextBlock>
                </Border>
                <Border Grid.Row="0" Padding="3" Grid.Column="5" Background="LightGray">
                  <TextBlock>Name</TextBlock>
                </Border>
                <xsl:for-each select="./SubComponent">
                  <xsl:variable name="sequence" select="count(preceding-sibling::SubComponent) + 1"/>
                  <Border BorderThickness="1"  Padding="3" Grid.Row="{$sequence}" Grid.Column="0" BorderBrush="Black">
                    <TextBlock>
                      <xsl:value-of select="$sequence"/>
                    </TextBlock>
                  </Border>
                  <Border BorderThickness="1"  Padding="3" Grid.Row="{$sequence}" Grid.Column="1" BorderBrush="Black">
                    <TextBlock>
                      <xsl:value-of select="@Datatype"/>
                    </TextBlock>
                  </Border>
                  <Border BorderThickness="1"  Padding="3" Grid.Row="{$sequence}" Grid.Column="2" BorderBrush="Black">
                    <TextBlock>
                        <xsl:value-of select="@Usage"/>
                    </TextBlock>
                  </Border>
                  <Border BorderThickness="1"  Padding="3" Grid.Row="{$sequence}" Grid.Column="3" BorderBrush="Black">
                    <TextBlock>
                      <xsl:value-of select="@Length"/>
                    </TextBlock>
                  </Border>
                  <Border BorderThickness="1"  Padding="3" Grid.Row="{$sequence}" Grid.Column="4" BorderBrush="Black">
                    <TextBlock>
                      <xsl:value-of select="@Table"/>
                    </TextBlock>
                  </Border>
                  <Border BorderThickness="1"  Padding="3" Grid.Row="{$sequence}" Grid.Column="5" BorderBrush="Black">
                    <TextBlock>
                      <xsl:value-of select="@Name"/>
                    </TextBlock>
                  </Border>
                </xsl:for-each>
              </Grid>
            </InlineUIContainer><LineBreak/>
          <Bold>Usage:</Bold><LineBreak/>
          
          <Span>R - Required</Span>
          <LineBreak/>
          
          <Span>X - Restricted</Span>
          <LineBreak/>
          
          <Span>O - Optional</Span>
          <LineBreak/>
          
          <Span>C - Conditional</Span>
          <LineBreak/>
          
          <Span>CE - Conditional/Empty</Span>
          <LineBreak/>
          
          <Span>RE - Required/Empty</Span>
          </Paragraph>
        </xsl:if>

      </Section>
    </Article>
    <xsl:apply-templates/>
  </xsl:template>

  <xsl:template match="SubComponent[not(@Usage = 'X')]">
    <xsl:variable name="msg_id" select="concat(concat(//HL7v2xStaticDef/@MsgType,'^'), //HL7v2xStaticDef/@EventType)"/>
    <xsl:variable name="segment_id" select="../../../@Name"/>
    <xsl:variable name="fieldNo" select="count(../../preceding-sibling::Field) + 1"/>
    <xsl:variable name="componentNo" select="count(../preceding-sibling::Component) + 1"/>
    <xsl:variable name="subComponentNo" select="count(preceding-sibling::SubComponent) + 1"/>
    <Article contentType="application/xaml+xml">
      <xsl:attribute name="kbid">
        <xsl:value-of select="concat(concat($msg_id,':'), $segment_id)"/>-<xsl:value-of select="$fieldNo"/>-<xsl:value-of select="$componentNo"/>-<xsl:value-of select="$subComponentNo"/>
      </xsl:attribute>
      <Section FontFamily="Calibri" FontSize="12"  xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
        <xsl:attribute name="xml:space">
          <xsl:value-of select="'preserve'"/>
        </xsl:attribute>
        <Paragraph FontFamily="Arial Black" FontSize="16">
          <xsl:value-of select="@Name"/> (SubComponent)
        </Paragraph>
        <Paragraph>
          <Bold>Path:</Bold>
          <Run>
            <xsl:value-of select="$segment_id"/>-<xsl:value-of select="$fieldNo"/>-<xsl:value-of select="$componentNo"/>-<xsl:value-of select="$subComponentNo"/>
          </Run>
        </Paragraph>
        <Paragraph>
          <Bold>Use:</Bold>
            <xsl:call-template name="usageDesc">
              <xsl:with-param name="code" select="@Usage"/>
            </xsl:call-template>
        </Paragraph>
        <Paragraph>
          <Bold>Type:</Bold>
          <Run>
            <xsl:value-of select="@Datatype"/>
          </Run>
        </Paragraph>
        <Paragraph>
          <Bold>Table:</Bold>
          <Run>
              <xsl:value-of select="@Table"/>
          </Run>
        </Paragraph>
      </Section>
    </Article>
  </xsl:template>

  <xsl:template match="SegGroup">
    <xsl:apply-templates/>
  </xsl:template>
  <xsl:template match="* | @*" priority="-1"></xsl:template>
</xsl:stylesheet>
