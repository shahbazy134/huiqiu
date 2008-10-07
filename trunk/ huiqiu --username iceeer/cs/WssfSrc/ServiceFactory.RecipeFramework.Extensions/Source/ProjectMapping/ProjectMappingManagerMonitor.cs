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
using Microsoft.Practices.RecipeFramework.Library;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.ProjectMapping.Configuration;
using System.Collections.ObjectModel;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.ProjectMapping.Serialization;
using System.IO;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.ProjectMapping.Helpers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Practices.VisualStudio.Helper;
using Helpers = Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.ProjectMapping.Helpers;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.ProjectMapping
{
	public class ProjectMappingManagerMonitor : IDisposable, IVsFileChangeEvents, IVsSolutionEvents
	{
		#region Fields
		private IServiceProvider serviceProvider;
		private IVsSolution solution;
		private bool disposed;
		#endregion

		#region Constructors
		public ProjectMappingManagerMonitor(IServiceProvider serviceProvider, IVsSolution solution)
		{
			this.serviceProvider = serviceProvider;
			this.solution = solution;
			AdviseSolutionEvents();
		}
		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(!this.disposed)
			{
				if(disposing)
				{
					UnAdviseVsFileChangeEvents();
					UnAdviseSolutionEvents();
				}
			}

			disposed = true;
		}

		~ProjectMappingManagerMonitor()
		{
			Dispose(false);
		}

		#endregion

		#region IVsFileChangeEvents Members
		private uint fileChangeEventsCookie;

		private void AdviseVsFileChangeEvents()
		{
			IVsFileChangeEx fileChange = GetService<IVsFileChangeEx, SVsFileChangeEx>();

			Debug.Assert(fileChange != null, "FileChange service is null");

			if(fileChange != null)
			{
				fileChange.AdviseFileChange(
					GetMappingFileName(),
					(uint)_VSFILECHANGEFLAGS.VSFILECHG_Time,
					this,
					out fileChangeEventsCookie);
			}
		}

		private void UnAdviseVsFileChangeEvents()
		{
			IVsFileChangeEx fileChange = GetService<IVsFileChangeEx, SVsFileChangeEx>();

			Debug.Assert(fileChange != null, "FileChange service is null");

			if(fileChange != null && fileChangeEventsCookie != 0)
			{
				fileChange.UnadviseFileChange(fileChangeEventsCookie);
				fileChangeEventsCookie = 0;
			}
		}

		public int DirectoryChanged(string pszDirectory)
		{
			return VSConstants.S_OK;
		}

		public int FilesChanged(uint cChanges, string[] rgpszFile, uint[] rggrfChange)
		{
			ProjectMappingManager.Instance.ReloadMappingFile();
			return VSConstants.S_OK;
		}

		#endregion

		#region IVsSolutionEvents Members
		private uint solutionEventsCookie;

		private void AdviseSolutionEvents()
		{
			solution.AdviseSolutionEvents(this, out solutionEventsCookie);
		}

		private void UnAdviseSolutionEvents()
		{
			solution.UnadviseSolutionEvents(solutionEventsCookie);
			solutionEventsCookie = 0;
		}

		public int OnAfterCloseSolution(object pUnkReserved)
		{
			return VSConstants.S_OK;
		}

		public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
		{
			return VSConstants.S_OK;
		}

		public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
		{
			return VSConstants.S_OK;
		}

		public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
		{
			ProjectMappingManager.Instance.ReloadMappingFile();
			AdviseVsFileChangeEvents();
			return VSConstants.S_OK;
		}

		public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
		{
			return VSConstants.S_OK;
		}

		public int OnBeforeCloseSolution(object pUnkReserved)
		{
			return VSConstants.S_OK;
		}

		public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
		{
			return VSConstants.S_OK;
		}

		public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}

		public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}

		public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}

		#endregion

		#region Private Implementation

		private TService GetService<TService, SService>()
		{
			return (TService)serviceProvider.GetService(typeof(SService));
		}

		private string GetMappingFileName()
		{
			string solutionDirectory;

			using (HierarchyNode node = new HierarchyNode(solution))
			{
				if (node.ProjectDir == null)
				{
					string solutionFile;
					string optsFile;
					solution.GetSolutionInfo(out solutionDirectory, out solutionFile, out optsFile);
				}
				else
				{
					solutionDirectory = node.ProjectDir;
				}
			}
			return Path.Combine(solutionDirectory, Helpers.Constants.MappingFile);
		}

		#endregion
	}
}