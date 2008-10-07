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
using System.Text;
using System.ComponentModel;
using System.Xml;
using System.Collections;
using Microsoft.Practices.Modeling.ExtensionProvider.Serialization;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.RecipeFramework.Library;
using Microsoft.Win32;
using System.Globalization;
using Microsoft.Practices.Modeling.ExtensionProvider.Helpers;
using Microsoft.VisualStudio.Modeling;

namespace Microsoft.Practices.Modeling.ExtensionProvider.Design.Converters
{
	/// <summary>
	/// TypeConverter used on the DSL Serialization process for the ObjectExtenderContainer type.
	/// </summary>
	public sealed class ObjectExtenderContainerConverter : TypeConverter
	{
		#region Fields
		IServiceProvider serviceProvider;
		ModelElement modelElement;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectExtenderContainerConverter"/> class.
		/// </summary>
		public ObjectExtenderContainerConverter()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectExtenderContainerConverter"/> class.
		/// </summary>
		/// <param name="serviceProvider">The service provider.</param>

		//HACK: Sometimes the ITypeDescriptorContext is null, because of that on we have this constructor 
		public ObjectExtenderContainerConverter(ModelElement modelElement, IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
			this.modelElement = modelElement;
		}

		#endregion

		#region Public Implementation
		/// <summary>
		/// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
		/// <param name="sourceType">A <see cref="T:System.Type"></see> that represents the type you want to convert from.</param>
		/// <returns>
		/// true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if(sourceType == typeof(string))
			{
				return true;
			}

			return base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		/// Converts the given object to the type of this converter, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
		/// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"></see> to use as the current culture.</param>
		/// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
		/// <returns>
		/// An <see cref="T:System.Object"></see> that represents the converted value.
		/// </returns>
		/// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if(value is string)
			{
				ObjectExtenderContainer container = null;
				try
				{
					//We need to obtain all posible referenced types for the XMLSerializer (XMLInclude)
					List<Type> types = (List<Type>)GetExtraTypesFromProviders(context);

					container = GenericSerializer.Deserialize<ObjectExtenderContainer>(types.ToArray(), value.ToString());
				}
				catch(InvalidOperationException iop)
				{
					Trace.TraceError(iop.Message);
					container = new ObjectExtenderContainer();
				}
				if (container != null)
				{
					foreach (object extender in container.ObjectExtenders)
					{
						if (extender is IObjectExtenderInternal)
						{
							((IObjectExtenderInternal)extender).ModelElement = this.modelElement;
						}
					}
				}
				return container;
			}

			return base.ConvertFrom(context, culture, value);
		}

		/// <summary>
		/// Converts the given value object to the specified type, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
		/// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"></see>. If null is passed, the current culture is assumed.</param>
		/// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
		/// <param name="destinationType">The <see cref="T:System.Type"></see> to convert the value parameter to.</param>
		/// <returns>
		/// An <see cref="T:System.Object"></see> that represents the converted value.
		/// </returns>
		/// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
		/// <exception cref="T:System.ArgumentNullException">The destinationType parameter is null. </exception>
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if(destinationType == typeof(string)
				&& value is ObjectExtenderContainer)
			{
				ObjectExtenderContainer objectExtenderContainer = value as ObjectExtenderContainer;

				//We need to obtain all posible referenced types for the XMLSerializer (XMLInclude)
				List<Type> types = (List<Type>)GetExtraTypesFromContainer(objectExtenderContainer);

				return GenericSerializer.Serialize<ObjectExtenderContainer>(value, types.ToArray());
			}

			return base.ConvertTo(context, culture, value, destinationType);
		} 
		#endregion

		#region Private Implementation
		private static IList<Type> GetExtraTypesFromContainer(ObjectExtenderContainer objectExtenderContainer)
		{
			Guard.ArgumentNotNull(objectExtenderContainer, "objectExtenderContainer");

			IList<Type> types = new List<Type>();

			foreach(object objectExtender in objectExtenderContainer.ObjectExtenders)
			{
				types.Add(objectExtender.GetType());
			}

			return types;
		}

		private IList<Type> GetExtraTypesFromProviders(ITypeDescriptorContext context)
		{
			IList<Type> types;

			//HACK: Sometimes the context is null, because of that we do this check
			if(context == null)
			{
				types = ServiceHelper.GetExtensionProviderService(this.serviceProvider).ObjectExtenderTypes;
			}
			else
			{
				types = ServiceHelper.GetExtensionProviderService(context as IServiceProvider).ObjectExtenderTypes;
			}

			return types;
		} 
		#endregion
	}
}