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
using Microsoft.Practices.Modeling.Dsl.Integration.References;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.Practices.Modeling.Dsl.Integration.Helpers;
using Microsoft.Practices.ServiceFactory.DataContracts;
using System.Runtime.Serialization;

namespace Microsoft.Practices.ServiceFactory.References
{
	[Serializable]
	public class OrderAllDataMembersReference : SurfaceAreaReference
	{
		public OrderAllDataMembersReference(string recipe) : base(recipe) { }

		public override bool IsEnabledFor(object target)
		{
			if (base.IsEnabledFor(target))
			{
				ModelingDocView docView = DesignerHelper.GetModelingDocView(this.Site);
				DataContractModel dcModel = docView.DocData.RootElement as DataContractModel;
				return (dcModel != null && dcModel.ImplementationTechnology != null);
			}
			return false;
		}

		public override string AppliesTo
		{
			get { return Properties.Resources.OrderAllDataMembersReference; }
		}

		#region ISerializable Members

		protected OrderAllDataMembersReference(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion ISerializable Members
	}
}
