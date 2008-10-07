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
using System.ServiceModel.Description;
using System.ComponentModel.Design;
using System.CodeDom.Compiler;
using EnvDTE;
using Microsoft.Practices.RecipeFramework.Extensions.CommonHelpers;
using System.IO;
using Microsoft.Practices.RecipeFramework.Library;
using System.CodeDom;
using Microsoft.Practices.ServiceFactory.Description;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateWCFClientProxy.Presentation
{
    public class SecureClientConfigModel
    {
        private IDictionaryService dict;
        public const string ServiceEndpointKeyName = "ServiceEndpoint";
        private const string ConfigurationKeyName = "Configuration";
        private const string ContractGenerationOptionsKeyName = "ContractGenerationOptions";
        private const string ClientProjectKeyName = "ClientProject";
        private const string ClientProjectNamespaceKeyName = "ClientProjectNamespace";
		private const string CodeCompileUnitKeyName = "ClientProxyCompileUnit";

        public SecureClientConfigModel(IDictionaryService dict)
        {
            this.dict = dict;
        }

        public ServiceEndpoint ServiceEndpoint
        {
			get { return dict.GetValue(ServiceEndpointKeyName) as ServiceEndpoint; }
			set { dict.SetValue(ServiceEndpointKeyName, value); }
        }

		public string ClientProjectNamespace
        {
            get { return dict.GetValue(ClientProjectNamespaceKeyName) as string; }
        }

        public System.Configuration.Configuration Configuration
        {
			get { return dict.GetValue(ConfigurationKeyName) as System.Configuration.Configuration; }
			set { dict.SetValue(ConfigurationKeyName, value); }
        }

        public ContractGenerationOptions ContractGenerationOptions
        {
            get { return dict.GetValue(ContractGenerationOptionsKeyName) as ContractGenerationOptions; }
            set { dict.SetValue(ContractGenerationOptionsKeyName, value); }
        }

        public CodeDomProvider CodeDomProvider
        {
            get
            {
                Project hostProject = dict.GetValue(ClientProjectKeyName) as Project;
                if (hostProject != null)
                {
                    return DteHelperEx.GetCodeDomProvider(hostProject);
                }
                return CodeDomProvider.CreateProvider("C#");
            }
        }

        public string BuildPath
        {
            get
            {
                Project hostProject = dict.GetValue(ClientProjectKeyName) as Project;
                if (hostProject != null)
                {
                    if (!DteHelper.IsWebProject(hostProject))
                    {
                        string path = hostProject.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value as string;
                        return Path.Combine(Path.GetDirectoryName(hostProject.FileName), path);
                    }
                }
                return AppDomain.CurrentDomain.RelativeSearchPath;
            }
        }

        public bool IsValid
        {
            get
            {
                return this.ServiceEndpoint != null &&
                       this.ContractGenerationOptions != null &&
					   this.CodeCompileUnit != null;
            }
        }

		public CodeCompileUnit CodeCompileUnit
		{
			get { return dict.GetValue(CodeCompileUnitKeyName) as CodeCompileUnit; }
			set { dict.SetValue(CodeCompileUnitKeyName, value); }
		}
    }
}
