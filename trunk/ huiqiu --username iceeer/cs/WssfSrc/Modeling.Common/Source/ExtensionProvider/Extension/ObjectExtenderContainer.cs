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
using System.ComponentModel;
using System.Collections;
using System.Xml.Serialization;

namespace Microsoft.Practices.Modeling.ExtensionProvider.Extension
{
	/// <summary>
	/// Class that represent a collection of ObjectExtenders
	/// </summary>
	[CLSCompliant(true)]
	[Serializable]
	//This must a fully qualified type name because at the end this is being call by TypeDescriptor.GetConverter()
	[TypeConverter("Microsoft.Practices.Modeling.ExtensionProvider.Design.Converters.ObjectExtenderContainerConverter, Microsoft.Practices.Modeling.ExtensionProvider, Version=3.1.0.0, Culture=neutral, PublicKeyToken=##CustomPublicKeyToken##")]
	public sealed class ObjectExtenderContainer
	{
		private ArrayList objectExtenders;

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectExtenderContainer"/> class.
		/// </summary>
		public ObjectExtenderContainer()
		{
			objectExtenders = new ArrayList();
		}

		/// <summary>
		/// Gets the object extenders on the container.
		/// </summary>
		/// <value>The object extenders on the container.</value>
		[Browsable(false)]
		[XmlElement("ObjectExtenders")]
		public ArrayList ObjectExtenders
		{
			get
			{
				return objectExtenders;
			}
		}
	}
}