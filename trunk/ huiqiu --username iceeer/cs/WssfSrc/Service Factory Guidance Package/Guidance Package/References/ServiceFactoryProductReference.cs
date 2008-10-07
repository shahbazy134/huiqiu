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
using Microsoft.Practices.RecipeFramework.Library;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ServiceFactory.References
{
	[Serializable]
	public class ServiceFactoryProductReference : UnboundRecipeReference
	{
		public ServiceFactoryProductReference(string recipe)
			: base(recipe)
		{
		}

		public override string AppliesTo
		{
			get { return Properties.Resources.ModelProjectReference; }
		}

		public override bool IsEnabledFor(object target)
		{
			Guard.ArgumentNotNull(target, "target");
			if (target is EnvDTE.Project)
			{
				return (((EnvDTE.Project)target).Object is IModelProject);
			}
			return false;
		}

		#region ISerializable Members
		protected ServiceFactoryProductReference(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
		#endregion ISerializable Members
	}
}