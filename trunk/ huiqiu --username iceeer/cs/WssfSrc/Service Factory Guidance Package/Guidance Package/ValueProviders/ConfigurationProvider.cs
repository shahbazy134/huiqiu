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
using System.ComponentModel.Design;
using System.Configuration;
using System.IO;
using EnvDTE;
using Microsoft.Practices.Common;
using Microsoft.Practices.ComponentModel;
using Microsoft.Practices.RecipeFramework;
using Microsoft.Practices.RecipeFramework.Library;
using Microsoft.Practices.RecipeFramework.Library.Configuration.Actions;
using System.Xml;

namespace Microsoft.Practices.ServiceFactory.ValueProviders
{
	/// <summary>
	/// Returns a class name
	/// </summary>
	public class ConfigurationProvider : SelectedProjectProviderBase
	{
		/// <summary>
		/// Contains code that will be called when recipe execution begins. This is the first method in the lifecycle.
		/// </summary>
		/// <param name="currentValue">An <see cref="T:System.Object"/> that contains the current value of the argument.</param>
		/// <param name="newValue">When this method returns, contains an <see cref="T:System.Object"/> that contains
		/// the new value of the argument, if the returned value
		/// of the method is <see langword="true"/>. Otherwise, it is ignored.</param>
		/// <returns>
		/// 	<see langword="true"/> if the argument value should be replaced with
		/// the value in <paramref name="newValue"/>; otherwise, <see langword="false"/>.
		/// </returns>
		/// <remarks>By default, always returns <see langword="false"/>, unless overriden by a derived class.</remarks>
		public override bool OnBeginRecipe(object currentValue, out object newValue)
		{
			newValue = null;
			if (this.SelectedProject == null)
			{
				return false;
			}

			string appConfig = GetAppConfig(this.SelectedProject);
			if (string.IsNullOrEmpty(appConfig))
			{
				appConfig = CreateConfig(this.SelectedProject);
			}

			ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
			fileMap.ExeConfigFilename = appConfig;
			newValue = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

			return true;
		}

		private string GetAppConfig(Project project)
		{
			string configName = GetConfigName(project);
			foreach (ProjectItem item in project.ProjectItems)
			{
				FileInfo info = new FileInfo(item.get_FileNames(0));
				if (info.Name.Equals(configName, StringComparison.OrdinalIgnoreCase))
				{
					return info.FullName;
				}
			}
			return string.Empty;
		}

		private string CreateConfig(Project project)
		{
			string filePath = Path.Combine(Path.GetDirectoryName(project.FullName), GetConfigName(project));
			CreateEmptyConfigurationFile(filePath);
			project.ProjectItems.AddFromFile(filePath);
			return filePath;
		}

		private string GetConfigName(Project project)
		{
			return (DteHelper.IsWebProject(project) ? "web.config" : "app.config");
		}

		private static void CreateEmptyConfigurationFile(string filePath)
		{
			using (XmlTextWriter writer = new XmlTextWriter(filePath, new UTF8Encoding(true, true)))
			{
				writer.Formatting = Formatting.Indented;
				writer.WriteStartDocument();
				writer.WriteStartElement("configuration");
				writer.WriteEndElement();
				writer.WriteEndDocument();
				writer.Flush();
			}
		}
	}
}
