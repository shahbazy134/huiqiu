<?xml version="1.0" encoding="ISO-8859-1" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" >
  <xsl:output method="html" encoding="ISO-8859-1" indent="yes" omit-xml-declaration="yes"/>
  <xsl:template match="/LogFile">
    <html>
      <head>

        <script type="text/javascript">
        </script>

        <style type="text/css">

          body { background-color: white; }

          .Heading { font-weight: bold; font-size: 20px; font-family: Verdana, Arial, Helvetica, sans-serif; }
          .ExpandCollapseStyle { font-weight: bold; font-size: 9px; font-family: Verdana, Arial, Helvetica, sans-serif; }
          .Footer { font-weight: normal; font-size: 9px; font-family: Verdana, Arial, Helvetica, sans-serif; }
          .LogTableHeader {background-color: #C0C0C0;}
          .ErrorRow { background-color: #FF6060;}
          .WarningRow { background-color: #FFFF99;}
          .NormalRow { background-color: #E9FFD0;}

        </style>

        <meta http-equiv="Content-Language" content="en-us"/>
        <meta http-equiv="Content-Type" content="text/html; charset=windows-1252"/>

        <title>
          Test Automation FX Log for Project <xsl:value-of select="/LogFile/ProjectName"/>
        </title>

      </head>
      <body>
        <p>
          <table style="width: 100%">
            <tr>
              <td style="width: 104px">
                <img alt="" height="95" src="Images/tafx.png" width="117" />
              </td>
              <td>
                <p class="Heading">
                  Test Automation FX Log for Project <xsl:value-of select="/LogFile/ProjectName"/>
                </p>
              </td>
            </tr>
          </table>
        </p>

        <table border="1"
               cellpadding="2"
               cellspacing="0"
               style="border-collapse: collapse; font-size: 9pt"
               bordercolor="black">

          <tbody>
            <tr>
              <td align="right">Filename :</td>
              <td>
                <xsl:value-of select="/LogFile/Filename"/>
              </td>
            </tr>
            <tr>
              <td align="right">Project Name :</td>
              <td>
                <xsl:value-of select="/LogFile/ProjectName"/>
              </td>
            </tr>
            <tr>
              <td align="right">TAFX Version :</td>
              <td>
                <xsl:value-of select="/LogFile/TAFXVersion"/>
              </td>
            </tr>
            <tr>
              <td align="right">Date :</td>
              <td>
                <xsl:value-of select="/LogFile/Date"/>
              </td>
            </tr>
            <tr>
              <td align="right">Time :</td>
              <td>
                <xsl:value-of select="/LogFile/Time"/>
              </td>
            </tr>
          </tbody>
        </table>

        <br/>
        <br/>
        <table border="1" cellpadding="2" cellspacing="0" style="border-collapse: collapse; font-size: 9pt"
               bordercolor="black" width="100%">
          <tr class="LogTableHeader">
            <th>Level</th>
            <th>Message</th>
          </tr>
          <xsl:for-each select="/LogFile/LogEntries/LogEntry">
            <xsl:if test="true()">
              <tr >
                <xsl:attribute name="class">
                  <xsl:choose>
                    <xsl:when test="@Level = 'Error'">ErrorRow</xsl:when>
                    <xsl:when test="@Level = 'Fatal'">ErrorRow</xsl:when>

                    <xsl:when test="@Level = 'Warning'">WarningRow</xsl:when>
                    <xsl:otherwise>NormalRow</xsl:otherwise>
                  </xsl:choose>
                </xsl:attribute>
                <td>
                  <xsl:value-of select="@Level"/>
                </td>
                <td>
                  <xsl:value-of select="@Message"/>
                </td>
              </tr>
            </xsl:if>
          </xsl:for-each>
        </table>

        <br/>

        <hr/>
        <center class="Footer">
          <xsl:text>Copyright &#169; 2008 Cenito Software</xsl:text>
          <br/>
          <a href="http://www.testautomationfx.com">http://www.testautomationfx.com</a>
        </center>
      </body>
    </html>

  </xsl:template>
</xsl:stylesheet>
