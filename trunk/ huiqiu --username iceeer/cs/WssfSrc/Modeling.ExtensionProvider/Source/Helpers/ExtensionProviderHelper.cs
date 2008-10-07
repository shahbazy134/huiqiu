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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Practices.RecipeFramework.Library;
using Microsoft.VisualStudio.Modeling;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using System.Globalization;
using Microsoft.Practices.Modeling.ExtensionProvider.Services;
using Microsoft.Practices.Modeling.ExtensionProvider.Metadata;

namespace Microsoft.Practices.Modeling.ExtensionProvider.Helpers
{
	public sealed class ExtensionProviderHelper
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ExtensionProviderHelper"/> class.
		/// </summary>
		/// <param name="extensibleObject">The extensible object.</param>
		public ExtensionProviderHelper(IExtensibleObject extensibleObject)
		{
			this.extensibleObject = extensibleObject;
		}
		#endregion

		#region Properties
		IExtensibleObject extensibleObject;

		/// <summary>
		/// Gets or sets the extensible object.
		/// </summary>
		/// <value>The extensible object.</value>
		public IExtensibleObject ExtensibleObject
		{
			get { return extensibleObject; }
			set { extensibleObject = value; }
		}
		#endregion

		#region Public Implementation
		/// <summary>
		/// Creates the object extender container.
		/// </summary>
		public void CreateObjectExtenderContainer()
		{
			this.extensibleObject.ObjectExtenderContainer = new ObjectExtenderContainer();
		}

		/// <summary>
		/// Sets the object extender.
		/// </summary>
		/// <param name="extender">The object extender.</param>
		public void SetObjectExtender(object extender)
		{
			this.extensibleObject.ObjectExtender = extender;
		}

		/// <summary>
		/// Gets the object extender.
		/// </summary>
		/// <param name="extensionProvider">The extension provider.</param>
		/// <returns></returns>
		public object GetObjectExtender(Extension.IExtensionProvider extensionProvider)
		{
			Guard.ArgumentNotNull(extensionProvider, "extensionProvider");

			Type objectExtenderType = GetObjectExtenderType(extensionProvider, this.extensibleObject.GetType());

			if (this.extensibleObject.ObjectExtenderContainer == null)
			{
				return null;
			}

			foreach(object objectExtender in this.extensibleObject.ObjectExtenderContainer.ObjectExtenders)
			{
				if(objectExtender.GetType() == objectExtenderType)
				{
					return objectExtender;
				}
			}

			return null;
		}

		/// <summary>
		/// Creates the object extender.
		/// </summary>
		/// <param name="extensionProvider">The extension provider.</param>
		/// <returns></returns>
		public object CreateObjectExtender(Extension.IExtensionProvider extensionProvider, ModelElement modelElement)
		{
			Guard.ArgumentNotNull(extensionProvider, "extensionProvider");

			Type objectExtenderType = GetObjectExtenderType(extensionProvider, this.extensibleObject.GetType());

			if (objectExtenderType == null)
			{
				return null;
			}

			if (extensibleObject.ObjectExtenderContainer == null)
			{
				CreateObjectExtenderContainer();
			}

			object objectExtender = GetObjectExtender(extensionProvider);

			if (objectExtender == null)
			{
				objectExtender = Activator.CreateInstance(objectExtenderType);
				if (objectExtender is IObjectExtenderInternal)
				{
					((IObjectExtenderInternal)objectExtender).ModelElement = modelElement;
				}
				this.extensibleObject.ObjectExtenderContainer.ObjectExtenders.Add(objectExtender);
			}

			return objectExtender;
		}

		/// <summary>
		/// Gets the type of the domain model associated with the IExtensionProvider.
		/// </summary>
		/// <param name="extensionProvider">The extension provider.</param>
		/// <returns></returns>
		public static Type GetDomainModelType(IExtensionProvider extensionProvider)
		{
			Guard.ArgumentNotNull(extensionProvider, "extensionProvider");

			ExtensionProviderAttribute att = ReflectionHelper.GetAttribute<ExtensionProviderAttribute>(extensionProvider.GetType());

			if(att != null)
			{
				return att.ModelToExtend;
			}

			return null;
		}

		/// <summary>
		/// Gets the type of the object extender associated to an object.
		/// </summary>
		/// <param name="extensionProvider">The extension provider.</param>
		/// <param name="objectToExtend">The object to extend.</param>
		/// <returns></returns>
		public static Type GetObjectExtenderType(Extension.IExtensionProvider extensionProvider, Type objectToExtend)
		{
			Guard.ArgumentNotNull(extensionProvider, "extensionProvider");

			foreach(Type type in extensionProvider.ObjectExtenders)
			{
				ObjectExtenderAttribute att = ReflectionHelper.GetAttribute<ObjectExtenderAttribute>(type);

				if(att != null)
				{
					if(att.ObjectToExtend == objectToExtend
						|| att.ObjectToExtend.IsAssignableFrom(objectToExtend))
					{
						return type;
					}
				}
			}

            // If no extender registered for the specified object then return null
            return null;
		}

		public static void AttachObjectExtender(IExtensibleObject extensibleObject, Extension.IExtensionProvider extensionProvider)
		{
			Guard.ArgumentNotNull(extensibleObject, "extensibleObject");
			Guard.ArgumentNotNull(extensionProvider, "extensionProvider");

			ExtensionProviderHelper extensionProviderHelper = new ExtensionProviderHelper(extensibleObject);

			object extender = extensionProviderHelper.CreateObjectExtender(extensionProvider, extensibleObject as ModelElement);
			extensionProviderHelper.SetObjectExtender(extender);			
		}

		#endregion
	}
}