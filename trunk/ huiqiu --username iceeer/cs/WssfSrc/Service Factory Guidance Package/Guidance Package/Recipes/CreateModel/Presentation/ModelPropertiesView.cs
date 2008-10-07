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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.Practices.WizardFramework;
using EnvDTE;
using System.IO;
using Microsoft.Practices.ServiceFactory.Library.Presentation.Base;
using Microsoft.Practices.ServiceFactory.Library.Presentation.Models;
using Microsoft.Practices.ServiceFactory.Library.Validation;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.Modeling.Presentation.Models;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateModel.Presentation
{
	public partial class ModelPropertiesView : ViewBase, IModelPropertiesView
	{
		private ModelPropertiesPresenter presenter;
		private ModelPropertiesModel model;
        private bool namespaceIsValid;
        private bool modelNameIsValid;

		public ModelPropertiesView(WizardForm wizard)
			: base(wizard)
		{
			InitializeComponent();
		}

		
		// Empty setter is needed for GAX to properly connect with this view.
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly")]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
		[RecipeArgument]
		public ModelType ModelType
		{
			set { }
		}

		// Empty setter is needed for GAX to properly connect with this view.
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly")]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
		[RecipeArgument]
		public string ModelName
		{
			set { }
		}

		// Empty setter is needed for GAX to properly connect with this view.
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly")]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
		[RecipeArgument]
		public string NamespacePrefix
		{
			set { }
		}

		private Project modelProject;

		[RecipeArgument]
		public Project ModelProject
		{
			get { return modelProject; }
			set { modelProject = value; }
		}

		#region IModelPropertiesView Members

		public event EventHandler<ValidationEventArgs> ModelNameChanged;
		public event EventHandler ModelTypeChanged;

		public string CurrentModelName
		{
			get { return txtModelName.Text; }
		}

		public ModelType CurrentModelType
		{
			get
			{
				if(lstModelTypes.SelectedItems.Count > 0)
				{
					return (ModelType)lstModelTypes.SelectedItems[0].Tag;
				}

				return ModelType.ServiceModel;
			}
		}

		IProjectModel currentModelProject;

		public IProjectModel CurrentModelProject 
		{
			get
			{
				if(currentModelProject == null)
				{
					currentModelProject = new DteProjectModel(this.ModelProject, this.Site as IServiceProvider);
				}

				return currentModelProject;
			}
		}

		public event EventHandler<ValidationEventArgs> NamespacePrefixChanged;

		public string CurrentNamespacePrefix
		{
			get { return txtNamespacePrefix.Text; }
		}
		#endregion

		#region Event Handlers

		private void ModelPropertiesView_Load(object sender, EventArgs e)
		{
			model = new ModelPropertiesModel(this.DictionaryService, this.CurrentModelProject);
			presenter = new ModelPropertiesPresenter(this, model);
			CreateModelItems();
			InitializeControls();
		}

		private void btnLargeIcons_Click(object sender, EventArgs e)
		{
			lstModelTypes.View = View.LargeIcon;
		}

		private void btnSmallIcons_Click(object sender, EventArgs e)
		{
			lstModelTypes.View = View.SmallIcon;
		}

		private void lstModelTypes_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(ModelTypeChanged != null)
			{
				if(lstModelTypes.SelectedItems.Count > 0)
				{
					ModelType modelType = (ModelType)lstModelTypes.SelectedItems[0].Tag;

					switch(modelType)
					{
						case ModelType.ServiceModel:
							lblDescriptionContent.Text = Properties.Resources.ServiceModelDescription;
							break;
						case ModelType.DataContractModel:
							lblDescriptionContent.Text = Properties.Resources.MessageModelDescription;
							break;
						case ModelType.HostDesignerModel:
							lblDescriptionContent.Text = Properties.Resources.HostModelDescription;
							break;
						default:
							break;
					}

					txtModelName.Text = string.Empty;

					ModelTypeChanged(lstModelTypes, e);
				}
			}
		}

		private void txtModelName_TextChanged(object sender, EventArgs e)
		{
			if(ModelNameChanged != null)
			{
				ValidationEventArgs args = CreateValidationEventArgs((Control)sender, errorProvider);
				ModelNameChanged(txtModelName, args);

                modelNameIsValid = args.ValidationResults.IsValid;
				Wizard.OnValidationStateChanged(this);
			}
		}

		#endregion

		#region Protected Implementation

		protected override void InitializeControls()
		{
			lstModelTypes.Items[0].Selected = true;
			txtModelName.Text = model.ModelName;
		}

        public override bool IsDataValid
        {
            get
            {
                return modelNameIsValid && namespaceIsValid;
            }
        }

		#endregion

		#region Private Implementation

		private void CreateModelItems()
		{
			foreach(ModelType modelType in Enum.GetValues(typeof(ModelType)))
			{
				ListViewItem modelListItem = new ListViewItem();
				modelListItem.ImageKey = modelType.ToString();
				modelListItem.Text = GetStringResource(modelType.ToString() + "ImageText", modelType.ToString()); ;
				modelListItem.Tag = modelType;
				lstModelTypes.Items.Add(modelListItem);
			}
		}

		private static string GetStringResource(string resourceKey, string defaultValue)
		{
			string resourceValue = Properties.Resources.ResourceManager.GetString(resourceKey, Properties.Resources.Culture);
			if (string.IsNullOrEmpty(resourceValue))
			{
				resourceValue = defaultValue;
			}
			return resourceValue;
		} 

		#endregion

		private void txtNamespacePrefix_TextChanged(object sender, EventArgs e)
		{
			if(NamespacePrefixChanged != null)
			{
				ValidationEventArgs args = CreateValidationEventArgs((Control)sender, errorProvider);
				NamespacePrefixChanged(txtNamespacePrefix, args);

				namespaceIsValid = args.ValidationResults.IsValid;
				Wizard.OnValidationStateChanged(this);
			}
		}
	}
}