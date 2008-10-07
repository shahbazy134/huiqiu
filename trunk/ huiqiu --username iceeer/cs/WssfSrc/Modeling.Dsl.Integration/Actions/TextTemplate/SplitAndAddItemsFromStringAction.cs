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
using Microsoft.Practices.ComponentModel;
using EnvDTE;
using Microsoft.Practices.RecipeFramework.Library;
using System.Diagnostics;
using System.IO;
using EnvDTE80;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Microsoft.Practices.Modeling.Dsl.Integration.Actions.TextTemplate
{
	public class SplitAndAddItemsFromStringAction : ConfigurableAction
	{
		static readonly Regex splitRegex
			= new Regex(@"^// begin ""(?<filename>[^""]+)"".?$\n(?<content>.*)^// end ""\k<filename>"".?$", RegexOptions.Multiline | RegexOptions.Singleline);

		#region Input Properties

		private string content;

		[Input(Required = true)]
		public string Content
		{
			get { return content; }
			set { content = value; }
		}

		private Project project;

		[Input(Required = true)]
		public Project Project
		{
			get { return project; }
			set { project = value; }
		}

		private bool open = true;

		[Input]
		public bool Open
		{
			get { return open; }
			set { open = value; }
		}

		private bool overwrite = true;

		[Input]
		public bool Overwrite
		{
			get { return overwrite; }
			set { overwrite = value; }
		}


		#endregion Input Properties

		#region Output Properties

		private ICollection<EnvDTE.ProjectItem> projectItems;

		[Output]
		public ICollection<EnvDTE.ProjectItem> ProjectItem
		{
			get { return projectItems; }
			set { projectItems = value; }
		}

		#endregion Output Properties

		public override void Execute()
		{
			DTE vs = GetService<DTE>(true);
			projectItems = new List<EnvDTE.ProjectItem>();

			for (Match match = splitRegex.Match(content); match.Success; match = match.NextMatch())
			{
				string relativeFileName = match.Groups["filename"].Value;
				string fileName = Path.GetFileName(relativeFileName);
				string relativeTargetPath = Path.GetDirectoryName(relativeFileName);

				ProjectItem projectItem = null;

				string tempfile = Path.GetTempFileName();
				using (StreamWriter sw = new StreamWriter(tempfile, false))
				{
					sw.WriteLine(match.Groups["content"].Value);
				}

				if (project.Object is SolutionFolder)
				{
					if (!string.IsNullOrEmpty(relativeTargetPath))
					{
						throw new InvalidOperationException(
							String.Format(CultureInfo.CurrentCulture, Properties.Resources.IntermediateFoldersNotSupportedInSolutionFolders, relativeFileName));
					}

					// Copy the file and add it explicitly from the right location.
					string targetPath = Path.Combine(
						Path.GetDirectoryName(vs.Solution.FileName),
						DteHelper.BuildPath(project));
					if (!Directory.Exists(targetPath))
					{
						Directory.CreateDirectory(targetPath);
					}
					string fileFullPath = Path.Combine(targetPath, match.Groups["filename"].Value);
					if (overwrite || !File.Exists(fileFullPath))
					{
						File.Copy(tempfile, fileFullPath, true);
						projectItem = project.ProjectItems.AddFromFile(fileFullPath);
					}
				}
				else
				{
					string fileFullPath = Path.Combine(
						Path.GetDirectoryName(project.FileName),
						relativeFileName);

					ProjectItems targetProjectItems = GetTargetProjectItems(project, relativeTargetPath, relativeFileName);
					ProjectItem existingItem = DteHelper.FindItemByName(targetProjectItems, fileName, false);

					if (existingItem != null)
					{
						// the file is already in the project
						if (overwrite)
						{
							// the item already exists in the project, overwrite it
							// this is a work around for visual studio: if the file exists and there's an
							// editor over it, the call to AddFromTemplate will succeed but the contents
							// of the file will be the ones in the editor, not the ones in the template
							File.Copy(tempfile, fileFullPath, true);
						}
					}
					else
					{
						// the item doesn't exist in the project (but might be in the file system)
						if (File.Exists(fileFullPath))
						{
							// the file exists, but it's not in the project, remove the file
							File.Delete(fileFullPath);
						}

						// add the item to the project, creating the physical file for it
						projectItem = targetProjectItems.AddFromTemplate(tempfile, fileName);
					}
				}

				if (projectItem != null)
				{
					projectItems.Add(projectItem);
					if (open)
					{
						Window wnd = projectItem.Open(Constants.vsViewKindPrimary);
						wnd.Visible = true;
						wnd.Activate();
					}
				}
				File.Delete(tempfile);
			}
		}

		private static ProjectItems GetTargetProjectItems(Project project, string relativeTargetPath, string fileName)
		{
			if (string.IsNullOrEmpty(relativeTargetPath))
			{
				return project.ProjectItems;
			}
			else
			{
				//ProjectItem targetprojectFolder
				//    = DteHelper.FindInCollection(project.ProjectItems, relativeTargetPath);
				string[] fragments = relativeTargetPath.Split(new char[] {
					Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

				ProjectItem targetProjectFolder = FindOrCreateProjectFolder(project.ProjectItems, fragments, 0);
				
				if (targetProjectFolder == null)
				{
					throw new InvalidOperationException(
						String.Format(CultureInfo.CurrentCulture, Properties.Resources.MissingTargetFolder, relativeTargetPath, project.Name, fileName));
				}
				if (targetProjectFolder.Kind != Constants.vsProjectItemKindPhysicalFolder)
				{
					throw new InvalidOperationException(
						String.Format(CultureInfo.CurrentCulture, Properties.Resources.TargetItemNotAFolder, relativeTargetPath, project.Name));
				}

				return targetProjectFolder.ProjectItems;
			}
		}

		private static ProjectItem FindOrCreateProjectFolder(ProjectItems items, string[] fragments, int index)
		{
			string path = fragments[index];

			foreach (ProjectItem item in items)
			{
				if (item.Name == path)
				{
					if (index == fragments.Length - 1)
					{
						return item;
					}
					else
					{
						return FindOrCreateProjectFolder(item.ProjectItems, fragments, ++index);
					}
				}
			}

			// Didn't find it, create structure.
			ProjectItems currentItems = items;
			ProjectItem currentItem = null;

			int i = index;
			do
			{
				try
				{
					currentItem = currentItems.AddFolder(fragments[i], Constants.vsProjectItemKindPhysicalFolder);
				}
				catch (Exception ex)
				{
					// Retry adding from the physical folder if it exists.
					string[] currentPath = new string[index + 1];
					Array.Copy(fragments, currentPath, index + 1);

					string folderOnDisk = Path.Combine(
							Path.GetDirectoryName(currentItems.ContainingProject.FileName),
							String.Join(Path.DirectorySeparatorChar.ToString(), currentPath));

					if (Directory.Exists(folderOnDisk))
					{
						try
						{
							currentItem = currentItems.AddFromDirectory(folderOnDisk);

						}
						catch (Exception innerEx)
						{
							Trace.TraceError(innerEx.ToString());
							throw;
						}
					}
					else
					{
						Trace.TraceError(ex.ToString());
						throw;
					}
				} 
				
				currentItems = currentItem.ProjectItems;
				i++;
			} while (i < fragments.Length);

			return currentItem;
		}

		/// <summary>
		/// Undoes the creation of the item, then deletes the item
		/// </summary>
		public override void Undo()
		{
			if (projectItems != null)
			{
				foreach (ProjectItem projectItem in projectItems)
				{
					projectItem.Delete();
				}
			}
		}
	}
}
