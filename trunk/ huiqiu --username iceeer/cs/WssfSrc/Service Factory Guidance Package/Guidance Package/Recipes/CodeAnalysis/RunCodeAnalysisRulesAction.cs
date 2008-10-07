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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Globalization;
using System.IO;
using EnvDTE;
using Web = VsWebSite;
using Vs = VSLangProj;
using Microsoft.Practices.RecipeFramework;
using Microsoft.Practices.RecipeFramework.Extensions.CommonHelpers;
using Microsoft.Practices.ComponentModel;
using Microsoft.Practices.RecipeFramework.Services;
using Microsoft.Practices.RecipeFramework.Library;
using System.Reflection;
using Microsoft.Practices.ServiceFactory.Actions;
using Microsoft.Practices.Modeling.CodeGeneration.Logging;

namespace Microsoft.Practices.ServiceFactory.Recipes.CodeAnalysis
{
    /// <summary>
    /// Run the Code Analysis rules in the selected project and all its references.
    /// </summary>
    [ServiceDependency(typeof(IConfigurationService))]
	public class RunCodeAnalysisRulesAction : ActionBase
    {
        private Project project;
        private EnvDTE.BuildEvents buildEvents;
        private const string RuleCheckIdPrefix = "WSSF";
        private string webTempFile;
        private StringDictionary runCodeAnalysisValues;

        /// <summary>
        /// Gets or sets the configuration file path.
        /// </summary>
        /// <value>The configuration file path.</value>
        [Input(Required = true)]
        public Project Project
        {
            get { return project; }
            set { project = value; }
        }

		protected override void OnExecute()
		{
			string rulesPath = GetRulesAssemblyLocation();
			runCodeAnalysisValues = new StringDictionary();

			// set the rules on each reference
			SetCodeAnalysisOnReferences(rulesPath);

			if (DteHelper.IsWebProject(this.project))
			{
				PrepareConfigFile();
				SetCodeAnalysisOnWebProject(this.project, rulesPath);
				SetUpBuildEventHandling();
				this.project.DTE.ExecuteCommand("Build.RunCodeAnalysisonWebSite", "");
			}
			else
			{
				SetCodeAnalysisOnProject(this.project, rulesPath);
				SetUpBuildEventHandling();
				this.project.DTE.ExecuteCommand("ClassViewContextMenus.ClassViewProject.RunCodeAnalysisonSelection", ""); // C++ project may need "RunCodeAnalysisonOnlyProject" command.
			}
		}

		protected override void OnUndo()
		{
			string rulesPath = string.Empty;

			//Clean all properties
			SetCodeAnalysisOnReferences(rulesPath);

			if (DteHelper.IsWebProject(this.project))
			{
				SetCodeAnalysisOnWebProject(this.project, rulesPath);
			}
			else
			{
				SetCodeAnalysisOnProject(this.project, rulesPath);
			}

			if (!string.IsNullOrEmpty(this.webTempFile) &&
				File.Exists(this.webTempFile))
			{
				File.Delete(this.webTempFile);
			}
		}

		#region Private methods

		private void SetUpBuildEventHandling()
        {
            // we need to know when the process ends so we may 
            // rollback all property changes. 
            // We will monitor the build event so we can rollback all changes after the buid process ends
            // Notice that the code analysis is done through the build process.
            buildEvents = (EnvDTE.BuildEvents)this.project.DTE.Events.BuildEvents;
            buildEvents.OnBuildDone += OnBuildDone;
        }

        private void OnBuildDone(EnvDTE.vsBuildScope Scope, EnvDTE.vsBuildAction Action)
        {
            // detach from the event
            buildEvents.OnBuildDone -= OnBuildDone;

            // rollback the properties changes
            this.Undo();

            EnvDTE80.ErrorList errorList = ((EnvDTE80.DTE2)this.project.DTE).ToolWindows.ErrorList;
            // ShowResult if there is any WCF rule violation
            int ruleWarnings = RuleWarningsCount(errorList);
            if (ruleWarnings == 0)
            {
				Logger.Write(
					Properties.Resources.CodeAnalysisSuccess, string.Empty, TraceEventType.Information, 1);
            }
            else
            {
				Logger.Write(
					string.Format(CultureInfo.CurrentCulture, Properties.Resources.CodeAnalysisWarnings, ruleWarnings));
                // We may force the Show Warnings button to be ON so the user will always see any 
                // warning message from the result of the code analysis run.
                errorList.ShowWarnings = true;
            }
            this.project = null;
        }

        private int RuleWarningsCount(EnvDTE80.ErrorList errorList)
        {
            int warningsCount = 0;
            for (int index = 1; index <= errorList.ErrorItems.Count; index++)
            {
                // check only on warnings
                if (errorList.ErrorItems.Item(index).ErrorLevel ==
                    EnvDTE80.vsBuildErrorLevel.vsBuildErrorLevelLow)
                {
                    if (errorList.ErrorItems.Item(index).Description.StartsWith(
                        RuleCheckIdPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        warningsCount++;
                    }
                }
            }
            return warningsCount;
        }

        private void PrepareConfigFile()
        {
            // add a temporal file on App_Code folder in case there's no code so we force precompilation and 
            // get code analysis for the web.config file
            // Note: this file may be removed after the code analysis process is done.
            Web.VSWebSite webProject = this.project.Object as Web.VSWebSite;
            if (webProject.CodeFolders.Count == 0)
            {
                // add App_Code folder
                webProject.CodeFolders.Add("App_Code\\");
            }
            Web.CodeFolder codeFolder = (Web.CodeFolder)webProject.CodeFolders.Item(1);
            // check if we need to add a temp file
            foreach (ProjectItem item in codeFolder.ProjectItem.ProjectItems)
            {
                string ext = Path.GetExtension(item.Name);
                if (!string.IsNullOrEmpty(ext) &&
                    !ext.Equals(".exclude", StringComparison.OrdinalIgnoreCase))
                {
                    // do not add the temp file since we already have some code file
                    return;
                }
            }

            this.webTempFile = Path.Combine(webProject.Project.FileName,
                codeFolder.FolderPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
                + Path.ChangeExtension(Path.GetRandomFileName(), DteHelperEx.GetDefaultExtension(this.project)));

            if (!File.Exists(this.webTempFile))
            {
                File.WriteAllText(this.webTempFile, string.Empty);
            }
        }

        private void SetCodeAnalysisOnProject(Project project, string rulesPath)
        {
            string runCodeAnalysisValue = Boolean.TrueString;
            if (string.IsNullOrEmpty(rulesPath))
            {
                runCodeAnalysisValue = runCodeAnalysisValues[project.UniqueName];
            }
            else
            {
                runCodeAnalysisValues.Add(project.UniqueName,
                    project.ConfigurationManager.ActiveConfiguration.Properties.Item("RunCodeAnalysis").Value.ToString());
            }
            project.ConfigurationManager.ActiveConfiguration.Properties.Item("RunCodeAnalysis").Value = runCodeAnalysisValue;
            project.ConfigurationManager.ActiveConfiguration.Properties.Item("CodeAnalysisRuleAssemblies").Value = rulesPath;
        }

        private void SetCodeAnalysisOnWebProject(Project project, string rulesPath)
        {
            project.Properties.Item("FxCopRuleAssemblies").Value = rulesPath;
        }

        private void SetCodeAnalysisOnReferences(string rulesPath)
        {
            if (!DteHelper.IsWebProject(this.project))
            {
                Vs.VSProject vsProject = this.project.Object as Vs.VSProject;
                foreach (Vs.Reference reference in vsProject.References)
                {
                    if (reference.SourceProject != null)
                    {
                        SetCodeAnalysisOnProject(reference.SourceProject, rulesPath);
                    }
                }
            }
            else
            {
                Web.VSWebSite webProject = this.project.Object as Web.VSWebSite;
                foreach (Web.AssemblyReference reference in webProject.References)
                {
                    Project project = GetSourceProject(reference);
                    if (project != null)
                    {
                        SetCodeAnalysisOnProject(project, rulesPath);
                    }
                }
            }
        }

        private Project GetSourceProject(Web.AssemblyReference reference)
        {
            Project sourceProject = null;

            if (reference.ReferenceKind != Web.AssemblyReferenceType.AssemblyReferenceConfig)
            {
                sourceProject = DteHelperEx.FindProject(reference.DTE, new Predicate<Project>(delegate(Project match)
                {
                    return (match.Kind == VSLangProj.PrjKind.prjKindCSharpProject ||
                        match.Kind == VSLangProj.PrjKind.prjKindVBProject) &&
                        match.Name.Equals(reference.Name, StringComparison.OrdinalIgnoreCase);
                }));
            }
            return sourceProject;
		}

		// Returns the rules assembly file in FxCop rules folder or the assembly location.
		private string GetRulesAssemblyLocation()
		{
            return Microsoft.Practices.FxCop.Rules.WcfSemantic.Utilities.GetRulesAssemblyLocation();
		}

		#endregion
	}
}
