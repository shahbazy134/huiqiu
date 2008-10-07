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

using Microsoft.Practices.WizardFramework;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Practices.ServiceFactory.Helpers;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.ServiceFactory.Library.Presentation.Base;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateTranslator.Presentation
{
	public partial class CreateTranslatorMappingsPage : ViewBase, ICreateTranslatorMappingView
    {
        Type firstType;
        Type secondType;
        MappedPropertyCollection propertyMappings = new MappedPropertyCollection();
        CreateTranslatorMappingPresenter presenter;
		const string PropertyMappingsArgumentName = "PropertyMappings";
		const string FirstTypeArgumentName = "FirstType";
		const string SecondTypeArgumentName = "SecondType";

        /// <summary>
        /// Gets or sets the property mappings.
        /// </summary>
        /// <value>The property mappings.</value>
        [RecipeArgument]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public MappedPropertyCollection PropertyMappings
        {
            get { return propertyMappings; }
            set { propertyMappings = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GenerateServiceContractTranslatorMappingsPage"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public CreateTranslatorMappingsPage(WizardForm parent)
            : base(parent)
        {
            InitializeComponent();
            presenter = new CreateTranslatorMappingPresenter(this);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:GenerateServiceContractTranslatorMappingsPage"/> class.
        /// </summary>
        public CreateTranslatorMappingsPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the site of the control.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.ComponentModel.ISite"></see> associated with the <see cref="T:System.Windows.Forms.Control"></see>, if any.</returns>
        /// <PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        public override ISite Site
        {
            get
            {
                return base.Site;
            }
            set
            {
                base.Site = value;
                if (value != null)
                {
                }
            }
        }

        /// <summary>
        /// Adds the mapping button enabled.
        /// </summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        public void AddMappingButtonEnabled(bool isEnabled)
        {
            this.createMappingButton.Enabled = isEnabled;
        }

        /// <summary>
        /// Removes the mapping button enabled.
        /// </summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        public void RemoveMappingButtonEnabled(bool isEnabled)
        {
            this.removeMappingButton.Enabled = isEnabled;
        }

        /// <summary>
        /// Adds the first type list types.
        /// </summary>
        /// <param name="firstTypeListNames">The first type list names.</param>
		// FxCop: False positive. PublicPropertyCollection is based on Collection<T>.
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public void AddFirstTypeListTypes(PublicPropertyCollection firstTypeListNames)
        {
            this.firstProperties.Items.AddRange(firstTypeListNames.ToArray());
        }

        /// <summary>
        /// Adds the second type list types.
        /// </summary>
        /// <param name="secondTypeListNames">The second type list names.</param>
		// FxCop: False positive. PublicPropertyCollection is based on Collection<T>.
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		public void AddSecondTypeListTypes(PublicPropertyCollection secondTypeListNames)
        {
            this.secondProperties.Items.AddRange(secondTypeListNames.ToArray());
        }

        /// <summary>
        /// Adds the mapped property.
        /// </summary>
        /// <param name="mappedProperty">The mapped property.</param>
		public void AddMappedProperty(MappedProperty mappedProperty)
        {
            this.mappedProperties.Items.Add(mappedProperty);
            propertyMappings.Add(mappedProperty);
			Wizard.OnValidationStateChanged(this);
        }

        /// <summary>
        /// Removes the type from first list.
        /// </summary>
        /// <param name="property">The property.</param>
		[SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#")]
		public void RemoveTypeFromFirstList(PublicPropertyListItem property)
        {
            this.firstProperties.Items.Remove(property);
        }

        /// <summary>
        /// Removes the type from second list.
        /// </summary>
        /// <param name="property">The property.</param>
		[SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#")]
		public void RemoveTypeFromSecondList(PublicPropertyListItem property)
        {
            this.secondProperties.Items.Remove(property);
        }

        /// <summary>
        /// Removes the mapped property.
        /// </summary>
        /// <param name="mappedProperty">The mapped property.</param>
        public void RemoveMappedProperty(MappedProperty mappedProperty)
        {
            this.mappedProperties.Items.Remove(mappedProperty);
            propertyMappings.Remove(mappedProperty);
			Wizard.OnValidationStateChanged(this);
		}

        /// <summary>
        /// Adds the property to first list.
        /// </summary>
        /// <param name="property">The property.</param>
        public void AddPropertyToFirstList(PublicPropertyListItem property) 
        {
            this.firstProperties.Items.Add(property);
        }

        /// <summary>
        /// Adds the property to second list.
        /// </summary>
        /// <param name="property">The property.</param>
        public void AddPropertyToSecondList(PublicPropertyListItem property)
        {
            this.secondProperties.Items.Add(property);
        }
       
        /// <summary>
        /// Notifies the user of invalid mapping.
        /// </summary>
        /// <param name="badMapping">The bad mapping.</param>
        public void NotifyUserOfInvalidMapping(MappedProperty badMapping) 
        {
            MessageBox.Show(String.Format(
                CultureInfo.CurrentCulture,
				Properties.Resources.InvalidMappingErrorMessage,
                badMapping.FirstPropertyListItem.ToString(), badMapping.SecondPropertyListItem.ToString()),
				Properties.Resources.InvalidMappingErrorTitle,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1,
                CustomPageHelper.GetRtlMessageBoxOptionsToShutUpFxCop(this));
        }

        public override void OnActivated()
        {
            base.OnActivated();

            if (firstType != (Type)this[FirstTypeArgumentName] ||
                secondType != (Type)this[SecondTypeArgumentName])
            {
                Clean();
                Initialize();
            }
        }

		public override bool IsDataValid
		{
			get
			{
				return (this.mappedProperties.Items.Count > 0);
			}
		}

        private void Clean()
        {
            firstProperties.Items.Clear();
            secondProperties.Items.Clear();
            mappedProperties.Items.Clear();

            if (propertyMappings != null)
            {
                propertyMappings.Clear();
            }
        }

        private void Initialize()
        {
            IDictionaryService dictionary = (IDictionaryService)GetService(typeof(IDictionaryService));
            dictionary.SetValue(PropertyMappingsArgumentName, new MappedPropertyCollection());

            firstType = (Type)this[FirstTypeArgumentName];
            secondType = (Type)this[SecondTypeArgumentName];

			firstPropertiesLabel.Text = String.Format(CultureInfo.CurrentUICulture, Properties.Resources.PropertiesLabelText, firstType.FullName);
            secondPropertiesLabel.Text = String.Format(CultureInfo.CurrentUICulture, Properties.Resources.PropertiesLabelText, secondType.FullName);

            presenter.Initialize(firstType, secondType);
        }

		private void firstProperties_SelectedIndexChanged(object sender, EventArgs e)
		{
			presenter.ElementSelectedForMapping(firstProperties.SelectedItem, secondProperties.SelectedItem);
		}

		private void secondProperties_SelectedIndexChanged(object sender, EventArgs e)
		{
			presenter.ElementSelectedForMapping(firstProperties.SelectedItem, secondProperties.SelectedItem);
		}

		private void createMappingButton_Click(object sender, EventArgs e)
		{
			presenter.CreateMappingButtonClicked(firstProperties.SelectedItem, secondProperties.SelectedItem);
		}

		private void removeMappingButton_Click(object sender, EventArgs e)
		{
			presenter.RemoveMappingButtonClicked(mappedProperties.SelectedItem);
		}

		private void mappedProperties_SelectedIndexChanged(object sender, EventArgs e)
		{
			presenter.MappedPropertySelected();
		}
    }
}
