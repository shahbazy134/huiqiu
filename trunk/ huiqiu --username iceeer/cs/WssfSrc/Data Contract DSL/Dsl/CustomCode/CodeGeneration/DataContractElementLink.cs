//===============================================================================
// Microsoft patterns & practices
// Web Service Software Factory
//===============================================================================
// Copyright � Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.Modeling.CodeGeneration.ArtifactLink;
using System.ComponentModel;
using Microsoft.Practices.Modeling.CodeGeneration.ArtifactLink.Design;
using Microsoft.Practices.Modeling.CodeGeneration;
using System.Drawing.Design;

namespace Microsoft.Practices.ServiceFactory.DataContracts
{
	[CLSCompliant(false)]
	[TextTemplate("TextTemplates\\WCF\\CS\\DataContract.tt")]
	[AssemblyReference("System.Runtime.Serialization, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	[TypeConverter(typeof(ArtifactLinkConverter<DataContractElementLink>))]
	[CodeGenerationStrategy(typeof(TextTemplateCodeGenerationStrategy))]
	public sealed class DataContractElementLink : ArtifactLink
	{
	}
}