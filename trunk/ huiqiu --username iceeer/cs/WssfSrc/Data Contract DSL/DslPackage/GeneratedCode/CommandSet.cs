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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DslShell = global::Microsoft.VisualStudio.Modeling.Shell;
using VSShell = global::Microsoft.VisualStudio.Shell;
using DslModeling = global::Microsoft.VisualStudio.Modeling;
using DslDiagrams = global::Microsoft.VisualStudio.Modeling.Diagrams;
using DslValidation = global::Microsoft.VisualStudio.Modeling.Validation;

namespace Microsoft.Practices.ServiceFactory.DataContracts
{
	/// <summary>
	/// Double-derived class to allow easier code customization.
	/// </summary>
	internal partial class DataContractDslCommandSet : DataContractDslCommandSetBase
	{
		/// <summary>
		/// Constructs a new DataContractDslCommandSet.
		/// </summary>
		public DataContractDslCommandSet(global::System.IServiceProvider serviceProvider) 
			: base(serviceProvider)
		{
		}
	}

	/// <summary>
	/// Class containing handlers for commands supported by this DSL.
	/// </summary>
	internal abstract class DataContractDslCommandSetBase : DslShell::CommandSet
	{
		/// <summary>
		/// Constructs a new DataContractDslCommandSetBase.
		/// </summary>
		protected DataContractDslCommandSetBase(global::System.IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		/// <summary>
		/// Provide the menu commands that this command set handles
		/// </summary>
		protected override global::System.Collections.Generic.IList<global::System.ComponentModel.Design.MenuCommand> GetMenuCommands()
		{
			// Get the standard commands
			global::System.Collections.Generic.IList<global::System.ComponentModel.Design.MenuCommand> commands = base.GetMenuCommands();

			global::System.ComponentModel.Design.MenuCommand menuCommand;
			// Add command handler for the "view explorer" command in the top-level menu.
			// We use a ContextBoundMenuCommand because the visibility of this command is
			// based on whether or not the command context of our DSL editor is active. 
			menuCommand = new DslShell::CommandContextBoundMenuCommand(this.ServiceProvider,
				new global::System.EventHandler(OnMenuViewModelExplorer),
				Constants.ViewDataContractDslExplorerCommand,
				typeof(DataContractDslEditorFactory).GUID);

			commands.Add(menuCommand);

			// Add handler for "Validate All" menu command (validates the entire model).
			menuCommand = new DslShell::DynamicStatusMenuCommand(new global::System.EventHandler(OnStatusValidateModel), 
				new global::System.EventHandler(OnMenuValidateModel),
				DslShell::CommonModelingCommands.ValidateModel);
			commands.Add(menuCommand);
			// Add handler for "Validate" menu command (validates the current selection).
			menuCommand = new DslShell::DynamicStatusMenuCommand(new global::System.EventHandler(OnStatusValidate), 
				new global::System.EventHandler(OnMenuValidate),
				DslShell::CommonModelingCommands.Validate);
			commands.Add(menuCommand);

			return commands;
		}
		/// <summary>
		/// Command handler that shows the explorer tool window.
		/// </summary>
		internal virtual void OnMenuViewModelExplorer(object sender, global::System.EventArgs e)
		{
			DataContractDslExplorerToolWindow explorer = this.DataContractDslExplorerToolWindow;
			if (explorer != null)
			{
				explorer.Show();
			}
		}

		/// <summary>
		/// Status event handler for validating the model.
		/// </summary>
		internal virtual void OnStatusValidateModel(object sender, global::System.EventArgs e)
		{
			System.ComponentModel.Design.MenuCommand cmd = sender as System.ComponentModel.Design.MenuCommand;
			cmd.Enabled = cmd.Visible = true;
		}

		/// <summary>
		/// Handler for validating the model.
		/// </summary>
		internal virtual void OnMenuValidateModel(object sender, System.EventArgs e)
		{
			if (this.CurrentDataContractDslDocData != null && this.CurrentDataContractDslDocData.Store != null)
			{
				this.CurrentDataContractDslDocData.ValidationController.Validate(this.CurrentDataContractDslDocData.Store, DslValidation::ValidationCategories.Menu);
			}
		}
		
		/// <summary>
		/// Status event handler for validating the current selection.
		/// </summary>
		internal virtual void OnStatusValidate(object sender, System.EventArgs e)
		{
			global::System.ComponentModel.Design.MenuCommand cmd = sender as global::System.ComponentModel.Design.MenuCommand;
			cmd.Visible = cmd.Enabled = false;

			foreach (object selectedObject in this.CurrentSelection)
			{
				DslModeling::ModelElement element = GetValidationTarget(selectedObject);
					
				if (element != null)
				{
					cmd.Visible = cmd.Enabled = true;
					break;
				}
			}
		}

		/// <summary>
		/// Handler for validating the current selection.
		/// </summary>
		internal virtual void OnMenuValidate(object sender, global::System.EventArgs e)
		{
			if (this.CurrentDataContractDslDocData != null && this.CurrentDataContractDslDocData.Store != null)
			{
				System.Collections.Generic.List<DslModeling::ModelElement> elementList = new System.Collections.Generic.List<Microsoft.VisualStudio.Modeling.ModelElement>();
				DslModeling::DepthFirstElementWalker elementWalker = new DslModeling::DepthFirstElementWalker(new ValidateCommandVisitor(elementList), new DslModeling::EmbeddingVisitorFilter(), true);
				foreach (object selectedObject in this.CurrentSelection)
				{
					// Build list of elements embedded beneath the selected root.
					DslModeling::ModelElement element = GetValidationTarget(selectedObject);
					if (element != null && !elementList.Contains(element))
					{
						elementWalker.DoTraverse(element);
					}
				}

				this.CurrentDataContractDslDocData.ValidationController.Validate(elementList, DslValidation::ValidationCategories.Menu);
			}
		}
		
		/// <summary>
		/// Helper method to retrieve the target root element for validation from the selected object.
		/// </summary>
		private static DslModeling::ModelElement GetValidationTarget(object selectedObject)
		{
			DslModeling::ModelElement element = null;
			DslDiagrams::PresentationElement presentation = selectedObject as DslDiagrams::PresentationElement;
			if (presentation != null)
			{
				if (presentation.Subject != null)
				{
					element = presentation.Subject;
				}
			}
			else
			{
				element = selectedObject as DslModeling::ModelElement;
			}
			return element;
		}
		
		#region ValidateCommandVisitor
		/// <summary>
		/// Helper class for building the list of elements to validate when the Validate command is executed.
		/// </summary>
		private sealed class ValidateCommandVisitor : DslModeling::IElementVisitor
		{
			private System.Collections.Generic.IList<DslModeling::ModelElement> validationList;
 
			/// <summary>
			/// Construct a ValidateCommandVisitor that adds elements to be validated to the specified list.
			/// </summary>
			public ValidateCommandVisitor(System.Collections.Generic.IList<DslModeling::ModelElement> elementList)
			{
				this.validationList = elementList;
			}

			/// <summary>
			/// Called when traversal begins. 
			/// </summary>
			public void StartTraverse(Microsoft.VisualStudio.Modeling.ElementWalker walker) { }

			/// <summary>
			/// Called when traversal ends. 
			/// </summary>
			public void EndTraverse(Microsoft.VisualStudio.Modeling.ElementWalker walker) { }
			
			/// <summary>
			/// Called for each element in the traversal.
			/// </summary>
			public bool Visit(Microsoft.VisualStudio.Modeling.ElementWalker walker, Microsoft.VisualStudio.Modeling.ModelElement element)
			{
				this.validationList.Add(element);
				return true;
			}
		}
		#endregion

		/// <summary>
		/// Returns the currently focused document.
		/// </summary>
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		protected DataContractDslDocData CurrentDataContractDslDocData
		{
			get
			{
				return this.MonitorSelection.CurrentDocument as DataContractDslDocData;
			}
		}

		/// <summary>
		/// Returns the currently focused document view.
		/// </summary>
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		protected DataContractDslDocView CurrentDataContractDslDocView
		{
			get
			{
				return this.MonitorSelection.CurrentDocumentView as DataContractDslDocView;
			}
		}

		/// <summary>
		/// Returns the explorer tool window.
		/// </summary>
		protected DataContractDslExplorerToolWindow DataContractDslExplorerToolWindow
		{
			get
			{
				DataContractDslExplorerToolWindow explorerWindow = null;
				DslShell::ModelingPackage package = this.ServiceProvider.GetService(typeof(VSShell::Package)) as DslShell::ModelingPackage;

				if (package != null)
				{
					explorerWindow = package.GetToolWindow(typeof(DataContractDslExplorerToolWindow), true) as DataContractDslExplorerToolWindow;
				}

				return explorerWindow;
			}
		}

		/// <summary>
		/// Returns the currently selected object in the model explorer.
		/// </summary>
		[global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		protected object ExplorerSelection
		{
			get
			{
				object selection = null;
				DataContractDslExplorerToolWindow explorerWindow = this.DataContractDslExplorerToolWindow;
				
				if(explorerWindow != null)
				{
					foreach (object o in explorerWindow.GetSelectedComponents())
					{
						selection = o;
						break;
					}
				}

				return selection;
			}
		}
	}
}
