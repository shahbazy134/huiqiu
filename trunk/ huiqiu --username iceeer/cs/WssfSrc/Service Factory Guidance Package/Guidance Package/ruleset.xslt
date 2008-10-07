<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html"/>
	<xsl:template match="/">
		<html>
			<head>
				<title>DSL Rule Set</title>
			</head>
			<body>
				<xsl:apply-templates />
			</body>
		</html>
	</xsl:template>

	<xsl:template match="//configuration/validation/type">
		<table border="1" width="100%">
			<tr>
				<td bgcolor="silver">
					Element: <xsl:value-of select="@name"/>
				</td>
			</tr>
			<xsl:apply-templates />

		</table>
		<br/>
	</xsl:template>

	<xsl:template match="ruleset">
		<tr>
			<td>
				Ruleset: <xsl:value-of select="@name"/>
				<xsl:apply-templates select="properties" />
			</td>
		</tr>
	</xsl:template>

	<xsl:template match="property">
		<p>
			Property: <xsl:value-of select="@name"/>
			<ul>
				<xsl:apply-templates />
			</ul>
		</p>
	</xsl:template>

	<xsl:template match="validator">
		<li>
			<xsl:choose>
				<xsl:when test="contains(@type, 'ElementCollectionValidator')">
					<xsl:choose>
						<xsl:when test="@collectionElementUniqueIdProperty = ''">
							All elements on this collection must have unique Name properties.
						</xsl:when>
						<xsl:otherwise>
							All elements on this collection must have unique <xsl:value-of select="@collectionElementUniqueIdProperty" /> properties.
						</xsl:otherwise>
					</xsl:choose>
				</xsl:when>
				<xsl:when test ="contains(@type, 'Microsoft.Practices.ServiceFactory.Validation.IdentifierValidator')">
					The property must be a valid C# identifier.
				</xsl:when>
				<xsl:when test ="contains(@type, 'Microsoft.Practices.ServiceFactory.Validation.CrossModelReferenceValidator')">
					The property must refer to a valid model element on another model.
				</xsl:when>
				<xsl:when test ="contains(@type, 'Microsoft.Practices.ServiceFactory.Validation.ElementObjectValidator')">
					Validate the object refered to by this property.
				</xsl:when>
				<xsl:when test ="contains(@type, 'Microsoft.Practices.ServiceFactory.Validation.ElementObjectCollectionValidator')">
					Validate the objects refered to by this collection.
				</xsl:when>
				<xsl:otherwise>
					<font color="red">Unknown validator: </font><xsl:value-of select="@type"/>
				</xsl:otherwise>
			</xsl:choose>
		</li>
	</xsl:template>
</xsl:stylesheet>