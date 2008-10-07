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
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Practices.RecipeFramework;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.Modeling.Utilities;
using Microsoft.Practices.Common;
using Microsoft.Practices.Modeling.Dsl.Integration.Helpers;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Modeling.Diagrams;
using System.Security.Permissions;
using Microsoft.Practices.RecipeFramework.Library;

namespace Microsoft.Practices.Modeling.Dsl.Integration.References
{
    [Serializable]
    public class ShapeReference : UnboundRecipeReference, IAttributesConfigurable
    {
		private const string ShapeNameAttribute = "ShapeName";
		private string shapeName;
		private ShapeElement selectedShape;

		public ShapeReference(string recipe) : base(recipe) { }

        /// <summary>
        /// Checks if the target selection contains a Shape with the provided shapeName.
        /// </summary>
		// FxCop: This code is calling into external libraries and we cannot know what exceptions 
		//		  might be thrown.
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public override bool IsEnabledFor(object target)
        {
            try
            {
                selectedShape = DomainModelHelper.GetSelectedShape(this.Site);
                if(selectedShape != null)
                {
                    return selectedShape.GetDomainClass().Name.Equals(shapeName);
                }
            }
            catch
            { }

            return false;
        }

        public override string AppliesTo
        {
            get { return Properties.Resources.DSLShape; }
        }

		protected ShapeElement SelectedShape
		{
			get { return selectedShape; }
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
        
		/// <summary>
        /// Required constructor for deserialization.
        /// </summary>
		protected ShapeReference(SerializationInfo info, StreamingContext context)
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