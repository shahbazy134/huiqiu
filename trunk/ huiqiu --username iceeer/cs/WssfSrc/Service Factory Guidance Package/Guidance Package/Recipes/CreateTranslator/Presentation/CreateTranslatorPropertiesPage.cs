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

using EnvDTE;
using Microsoft.Practices.Modeling.Presentation.Models;
using Microsoft.Practices.ServiceFactory.Library.Presentation.Models;
using Microsoft.Practices.ServiceFactory.Library.Validation;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.Editors.TypeBrowser;
using Microsoft.Practices.WizardFramework;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using Microsoft.Practices.Common;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.ServiceFactory.Library.Presentation.Base;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateTranslator.Presentation
{
	public partial class CreateTranslatorPropertiesPage : ViewBase, ICreateTranslatorPropertiesView
	{
		private Project project;
		private IProjectModel projectModel;

		public CreateTranslatorPropertiesPage()
		{
			InitializeComponent();
		}

		public CreateTranslatorPropertiesPage(WizardForm parent)
			: base(parent)
		{
			InitializeComponent();

			if(this.DesignMode)
				return;

			this.mappingClassNameTextBox.TextChanged += OnMappingClassNameTextBoxTextChanged;
			this.mappingClassNamespaceTextBox.TextChanged += OnMappingClassNamespaceTextBoxTextChanged;
			this.firstClassValueEditor.ValueChanged += OnFirstClassValueEditorValueChanged;
			this.secondClassValueEditor.ValueChanged += OnSecondClassValueEditorValueChanged;

			firstClassValueEditor.BeginInit();
			firstClassValueEditor.ValueType = typeof(Type);
			firstClassValueEditor.EditorType = typeof(FilteredTypeBrowser);
			firstClassValueEditor.EndInit();

			// The Editor *instance* is available only after the initialization of the value editor.
			SetFilterType(firstClassValueEditor.EditorInstance as IAttributesConfigurable);

			secondClassValueEditor.BeginInit();
			secondClassValueEditor.ValueType = typeof(Type);
			secondClassValueEditor.EditorType = typeof(FilteredTypeBrowser);
			secondClassValueEditor.EndInit();
			SetFilterType(secondClassValueEditor.EditorInstance as IAttributesConfigurable);
		}

		public override ISite Site
		{
			get
			{
				return base.Site;
			}
			set
			{
				base.Site = value;
				if(value != null)
				{
					Site.Container.Add(firstClassValueEditor);
					Site.Container.Add(secondClassValueEditor);
				}
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			InitializePresenterAndModel();
			Wizard.OnValidationStateChanged(this);
		}

		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.Practices.ServiceFactory.Recipes.CreateTranslator.Presentation.CreateTranslatorPropertiesPresenter")]
		protected void InitializePresenterAndModel()
		{
			IDictionaryService dictionary = (IDictionaryService)GetService(typeof(IDictionaryService));
			CreateTranslatorPropertiesModel model = new CreateTranslatorPropertiesModel(dictionary);

			projectModel = new DteProjectModel(project, Site);

			new CreateTranslatorPropertiesPresenter(projectModel, this, model);
		}

		public override bool IsDataValid
		{
			get
			{
				RequestDataEventArgs<bool> args = new RequestDataEventArgs<bool>();
				OnRequestIsDataValid(this, args);
				if(args.ValueProvided)
				{
					return args.Value;
				}
				return base.IsDataValid;
			}
		}

		#region Controls Event Handlers

		void OnSecondClassValueEditorValueChanged(object sender, EventArgs e)
		{
			OnSecondTypeChanged(sender, CreateValidationEventArgs(this.secondClassValueEditor, this.errorProvider));
		}

		void OnFirstClassValueEditorValueChanged(object sender, EventArgs e)
		{
			OnFirstTypeChanged(sender, CreateValidationEventArgs(this.firstClassValueEditor, this.errorProvider));
		}

		void OnMappingClassNamespaceTextBoxTextChanged(object sender, EventArgs e)
		{
			OnMappingClassNamespaceChanged(sender, CreateValidationEventArgs(this.mappingClassNamespaceTextBox, this.errorProvider));
		}

		void OnMappingClassNameTextBoxTextChanged(object sender, EventArgs e)
		{
			OnMappingClassNameChanged(sender, CreateValidationEventArgs(this.mappingClassNameTextBox, this.errorProvider));
		}

		#endregion

		#region Gax Input Arguments

		[RecipeArgument]
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly")]
		public Project ServiceImplementationProject
		{
			set { project = value; }
		}

		[RecipeArgument]
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly")]
		public string ServiceImplementationNamespace
		{
			set
			{
				if(value != null)
				{
					mappingClassNamespaceTextBox.Text = value;
				}
			}
		}
		#endregion

		#region IGenerateServiceContractTranslatorPropertiesView Members

		public string MappingClassNamespace
		{
			get { return mappingClassNamespaceTextBox.Text; }
			set { mappingClassNamespaceTextBox.Text = value; }
		}

		public Type FirstType
		{
			get { return firstClassValueEditor.Value as Type; }
			set { firstClassValueEditor.Value = value; }
		}

		public Type SecondType
		{
			get { return secondClassValueEditor.Value as Type; }
			set { secondClassValueEditor.Value = value; }
		}

		[RecipeArgument]
		public string MappingClassName
		{
			get { return mappingClassNameTextBox.Text; }
			set
			{
				if(value != null)
				{
					mappingClassNameTextBox.Text = value;
				}
			}
		}

		public event EventHandler<ValidationEventArgs> MappingClassNameChanged;
		public event EventHandler<ValidationEventArgs> MappingClassNamespaceChanged;
		public event EventHandler<ValidationEventArgs> FirstTypeChanged;
		public event EventHandler<ValidationEventArgs> SecondTypeChanged;
		public event EventHandler<RequestDataEventArgs<bool>> RequestIsDataValid;

		#endregion

		#region View Event Handlers

		[SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
		protected void OnMappingClassNameChanged(object sender, ValidationEventArgs args)
		{
			if(MappingClassNameChanged != null)
			{
				MappingClassNameChanged(sender, args);
			}
		}

		[SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
		protected void OnMappingClassNamespaceChanged(object sender, ValidationEventArgs args)
		{
			if(MappingClassNamespaceChanged != null)
			{
				MappingClassNamespaceChanged(sender, args);
			}
		}


		[SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
		protected void OnFirstTypeChanged(object sender, ValidationEventArgs args)
		{
			if(FirstTypeChanged != null)
			{
				FirstTypeChanged(sender, args);
			}
		}


		[SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
		protected void OnSecondTypeChanged(object sender, ValidationEventArgs args)
		{
			if(SecondTypeChanged != null)
			{
				SecondTypeChanged(sender, args);
			}
		}

		[SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
		protected virtual void OnRequestIsDataValid(object sender, RequestDataEventArgs<bool> e)
		{
			if(RequestIsDataValid != null)
			{
				RequestIsDataValid(sender, e);
			}
		}
		#endregion

		#region Private Implementation
		private static void SetFilterType(IAttributesConfigurable configurableEditor)
		{
			if(configurableEditor != null)
			{
				StringDictionary dictionary = new StringDictionary();
				Type filterType = typeof(PublicTypeWithDefaultConstructorFilter);
				dictionary.Add(FilteredTypeBrowser.FilterAttribute, filterType.AssemblyQualifiedName);
				configurableEditor.Configure(dictionary);
			}
		}
		#endregion
	}
}