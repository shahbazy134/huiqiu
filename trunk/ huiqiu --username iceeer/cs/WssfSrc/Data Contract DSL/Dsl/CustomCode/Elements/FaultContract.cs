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

using Microsoft.Practices.Modeling.CodeGeneration;
using Microsoft.Practices.Modeling.CodeGeneration.Artifacts;
using Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.TypeDescription;
using Microsoft.Practices.ServiceFactory.Validation;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Validation;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Practices.ServiceFactory.DataContracts
{
	[ValidationState(ValidationState.Enabled)]
	public partial class FaultContract : IArtifactLinkContainer
	{
		#region Validation support

		[ValidationMethod(ValidationCategories.Menu | ValidationCategories.Open | ValidationCategories.Save)]
		private void OnValidate(ValidationContext context)
		{
			ValidationEngine.Validate(ValidationElementState.FirstElement, context, this, context.Categories.ToString());
		}

		public void ContinueValidation(ValidationContext context)
		{
			ValidationEngine.Validate(ValidationElementState.LinkedElement, context, this, context.Categories.ToString());
		}

		#endregion

		#region IArtifactLinkContainer Members
		
		[Browsable(false)]
		public System.Collections.Generic.ICollection<IArtifactLink> ArtifactLinks
		{
			get
			{
				List<IArtifactLink> links = new List<IArtifactLink>();

				if(ObjectExtender is IArtifactLinkContainer)
				{
					IArtifactLinkContainer container = (IArtifactLinkContainer)ObjectExtender;
					links.AddRange(container.ArtifactLinks);
				}

				return links;
			}
		}

		#endregion
	}
}