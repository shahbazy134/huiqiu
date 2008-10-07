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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Microsoft.Practices.Common;
using Microsoft.Practices.Modeling.Dsl.Integration.Helpers;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.Practices.RecipeFramework.Library;
using System.Security.Permissions;

namespace Microsoft.Practices.ServiceFactory.References
{
	[Serializable]
	public class ShapeHasArtifactLinksReference : HasArtifactLinksReference, IAttributesConfigurable
	{
		private const string ShapeNameAttribute = "ShapeName";
		private string shapeName;

		public ShapeHasArtifactLinksReference(string recipe)
			: base(recipe)
		{
		}

		public override string AppliesTo
		{
			get { return Properties.Resources.ShapeHasArtifactLinksReference; }
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public override bool IsEnabledFor(object target)
		{
			try
			{
                ShapeElement selectedShape = DomainModelHelper.GetSelectedShape(this.Site);
				if(selectedShape != null)
				{
					if(selectedShape.GetDomainClass().Name.Equals(shapeName))
					{
						return base.IsEnabledFor(target);
					}
				}
			}
			catch
			{ }

			return false;
		}

		#region IAttributesConfigurable Members

		public void Configure(System.Collections.Specialized.StringDictionary attributes)
		{
			Guard.ArgumentNotNull(attributes, "attributes");

			if(attributes.ContainsKey(ShapeNameAttribute))
			{
				shapeName = attributes[ShapeNameAttribute];
			}
		}
		#endregion

		#region ISerializable Members

		protected ShapeHasArtifactLinksReference(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			Guard.ArgumentNotNull(info, "info");
			shapeName = info.GetString(ShapeNameAttribute);
		}

		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		protected override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(ShapeNameAttribute, shapeName);
		}
		#endregion ISerializable Members
	}
}