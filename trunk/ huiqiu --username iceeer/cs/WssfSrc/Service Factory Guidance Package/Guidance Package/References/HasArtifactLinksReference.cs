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
using Microsoft.Practices.RecipeFramework.Extensions.References.VisualStudio;
using System.Runtime.Serialization;
using Microsoft.Practices.RecipeFramework;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Practices.ServiceFactory;
using Microsoft.Practices.Modeling.Dsl.Integration.Helpers;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling;
using Microsoft.Practices.Modeling.CodeGeneration.Artifacts;
using ExtFwk = Microsoft.Practices.Modeling.ExtensionProvider.Extension;
using Microsoft.Practices.Modeling.ExtensionProvider.Helpers;
using Microsoft.Practices.ServiceFactory.Actions;
using Microsoft.Practices.RecipeFramework.Extensions.CommonHelpers;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Modeling.CodeGeneration.Logging;
using Microsoft.Practices.Modeling.CodeGeneration;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.ProjectMapping;

namespace Microsoft.Practices.ServiceFactory.References
{
	[Serializable]
	public class HasArtifactLinksReference : UnboundRecipeReference
	{
		public HasArtifactLinksReference(string recipe)
			: base(recipe)
		{
		}

		public override string AppliesTo
		{
			get { return Properties.Resources.ModelProjectReference; }
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		public override bool IsEnabledFor(object target)
		{
			try
			{
				ICodeGenerationService codeGenerationService = GetService<ICodeGenerationService>();
				if (codeGenerationService == null)
				{
					return false;
				}
				IArtifactLinkContainer links = ModelCollector.GetArtifacts(this.Site);
				if (links != null &&
					links.ArtifactLinks != null &&
					links.ArtifactLinks.Count > 0)
				{
					return codeGenerationService.AreValid(links.ArtifactLinks);
				}
			}
			catch (InvalidOperationException ioException)
			{
				Logger.Write(ioException.Message);
			}
			catch (ProjectMappingTableNotFoundException)
			{
				Logger.Write(Properties.Resources.ProjectMappingTableException);
			}
			catch (Exception exception)
			{
				Logger.Write(exception.ToString());
			}
			return false;
		}

		#region ISerializable Members

		protected HasArtifactLinksReference(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion ISerializable Members
	}
}