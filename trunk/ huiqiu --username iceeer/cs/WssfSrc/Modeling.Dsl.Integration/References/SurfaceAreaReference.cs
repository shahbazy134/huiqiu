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
using Microsoft.Practices.RecipeFramework;
using Microsoft.VisualStudio.Modeling.Utilities;
using Microsoft.VisualStudio.Modeling;
using System.Runtime.Serialization;
using Microsoft.Practices.Modeling.Dsl.Integration.Helpers;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Modeling.Diagrams;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.Modeling.Dsl.Integration.References
{
    [Serializable]
    public class SurfaceAreaReference : UnboundRecipeReference
    {
        public SurfaceAreaReference(string recipe) : base(recipe) { }

        protected SurfaceAreaReference(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        /// Checks if the target selection is the DSL Surface area.
        /// </summary>
		// FxCop: This code is calling into external libraries and we cannot know what exceptions 
		//		  might be thrown.
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public override bool IsEnabledFor(object target)
        {
			if (target != null) // only evaluate for some target, and ignore any other call
			{
				try
				{
                    return DomainModelHelper.IsDiagramSelected(this.Site);
				}
				catch{ } // ignore COM exceptions
			}

            return false;
        }

        public override string AppliesTo
        {
            get { return Properties.Resources.DSLSurfaceArea; }
        }
    }
}