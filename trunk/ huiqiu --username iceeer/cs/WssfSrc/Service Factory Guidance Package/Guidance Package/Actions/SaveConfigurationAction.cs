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
using System.IO;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using Microsoft.Practices.RecipeFramework;
using Config = System.Configuration.Configuration;
using EnvDTE;
using System.ComponentModel.Design;
using Microsoft.Practices.ComponentModel;
using Microsoft.Practices.ServiceFactory.Helpers;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Practices.ServiceFactory.Actions
{
	/// <summary>
	/// Saves any changes to a Configuration object
	/// </summary>
	[ServiceDependency(typeof(DTE))]
	public sealed class SaveConfigurationAction : ActionBase
	{
		#region Input Properties

		/// <summary>
		/// The configuration object to Save
		/// </summary>
		[Input(Required = true)]
		public Config Configuration
		{
			get { return configuration; }
			set { configuration = value; }
		} Config configuration;

		#endregion

		#region Output Properties

		#endregion

		#region Action members

		/// <summary>
		/// <see cref="IAction.Execute"/>
		/// </summary>
		public override void Execute()
		{
			DTE vs = GetService<DTE>(true);
			TFSHelper.CheckOutFile(vs, this.configuration.FilePath);
			this.Configuration.Save(ConfigurationSaveMode.Modified);

			base.Trace(Properties.Resources.UpdatedFile, this.configuration.FilePath);
		}

		#endregion
	}
}
