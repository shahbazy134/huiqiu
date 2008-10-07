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

using Microsoft.Practices.Modeling.CodeGeneration.Artifacts;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Metadata;
using Microsoft.Practices.ServiceFactory.DataContracts;
using Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx.Design.Editors;
using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Microsoft.Practices.ServiceFactory.Extenders.DataContract.Asmx
{
	[Serializable]
	[ObjectExtender(typeof(DataContractCollectionBase))]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
	[CLSCompliant(false)]
	public class AsmxDataContractCollection : ObjectExtender<DataContractCollectionBase>, IXmlSerializable
	{
		#region Constructors

		public AsmxDataContractCollection()
		{
		}

		#endregion

		#region fields
		private Type collectionType = null;

		#endregion

		#region Properties

		[Category(DataContractAsmxExtensionProvider.ExtensionProviderPropertyCategory),
        Description("This value determines what kind of collection is generated in code."),
         DisplayName("Collection Type"),
         ReadOnly(false),
		 BrowsableAttribute(true)]
		[System.ComponentModel.Editor(typeof(AsmxCollectionTypesEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[System.ComponentModel.TypeConverterAttribute(typeof(System.ComponentModel.TypeConverter))]
		public Type CollectionType
		{
			get { return collectionType; }
			set { collectionType = value; }
		}

		[Category(DataContractAsmxExtensionProvider.ExtensionProviderPropertyCategory),
		 Description("Specifies how the code will be generated."),
         DisplayName("Artifact Link"),
         ReadOnly(true),
		 BrowsableAttribute(true)]
		[XmlIgnore]
		public AsmxDataContractCollectionLink ArtifactLink
		{
			get
			{
				if (this.ModelElement != null &&
					this.ModelElement.DataContractModel != null)
				{
					return ArtifactLinkFactory.CreateInstance<AsmxDataContractCollectionLink>(
							this.ModelElement,
							this.ModelElement.DataContractModel.ProjectMappingTable);
				}
				return null;
			}
		}

		#endregion

		#region IXmlSerializable Members

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(System.Xml.XmlReader reader)
		{
			string typeName = reader.ReadElementString();
			if (!string.IsNullOrEmpty(typeName))
			{
				this.CollectionType = Type.GetType(typeName);
			}
		}

		public void WriteXml(System.Xml.XmlWriter writer)
		{
			if (this.CollectionType != null)
			{
				writer.WriteElementString("CollectionType", this.CollectionType.AssemblyQualifiedName);
			}
		}

		#endregion
	}
}
