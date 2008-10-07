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
using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Practices.ServiceFactory.VsPkg.ModelProject
{
	public class BuildActionConverter : EnumConverter
	{
		private const string buildActionResourceNamePrefix = "BuildAction_";

		public BuildActionConverter()
			: base(typeof(BuildAction))
		{

		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string)) return true;

			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string str = value as string;

			if (str != null)
			{
				if (str == Properties.Resources.BuildAction_Model) return BuildAction.Model;
				if (str == Properties.Resources.BuildAction_Content) return BuildAction.Content;
				if (str == Properties.Resources.BuildAction_None) return BuildAction.None;
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				string result = null;

				// In some cases if multiple nodes are selected the windows form engine
				// calls us with a null value if the selected node's property values are not equal
				// Example of windows form engine passing us null: File set to Compile, Another file set to None, bot nodes are selected, and the build action combo is clicked.
				if (value != null)
				{
					result = Properties.Resources.ResourceManager.GetString(buildActionResourceNamePrefix + ((BuildAction)value).ToString());
				}
				else
				{
					result = Properties.Resources.ResourceManager.GetString(buildActionResourceNamePrefix + BuildAction.None.ToString());
				}

				if (result != null) return result;
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context)
		{
			return true;
		}

		public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
		{
			return new StandardValuesCollection(new BuildAction[] { BuildAction.Model, BuildAction.Content, BuildAction.None });
		}
	}

	[PropertyPageTypeConverterAttribute(typeof(BuildActionConverter))]
	public enum BuildAction { None, Model, Content }

}
