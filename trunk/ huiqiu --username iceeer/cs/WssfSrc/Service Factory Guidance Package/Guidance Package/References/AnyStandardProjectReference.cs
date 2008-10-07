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
using Microsoft.Practices.RecipeFramework.Extensions.References.VisualStudio;

namespace Microsoft.Practices.ServiceFactory.References
{
	[Serializable]
	public class AnyStandardProjectReference : AnyProjectReference
	{
		public AnyStandardProjectReference(string recipe)
			: base(recipe)
		{
		}

		public override bool IsEnabledFor(object target)
		{
			Guard.ArgumentNotNull(target, "target");
			return( base.IsEnabledFor(target) && !IsCustomProject(target) );
		}

		private bool IsCustomProject(object target)
		{
			if (target is EnvDTE.Project)
			{
				return (((EnvDTE.Project)target).Object is IModelProject);
			}
			return false;
		}

		#region ISerializable Members

		protected AnyStandardProjectReference(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion ISerializable Members
	}
}