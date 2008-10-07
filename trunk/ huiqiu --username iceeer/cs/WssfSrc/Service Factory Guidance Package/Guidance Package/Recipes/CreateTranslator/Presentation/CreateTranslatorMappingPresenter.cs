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
using Microsoft.Practices.ServiceFactory.Helpers;
using Microsoft.Practices.RecipeFramework.Library;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateTranslator.Presentation
{
    public class CreateTranslatorMappingPresenter
    {
        ICreateTranslatorMappingView view;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GenerateServiceContractTranslatorMappingPresenter"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        public CreateTranslatorMappingPresenter(ICreateTranslatorMappingView view)
        {
            Guard.ArgumentNotNull(view, "view");
            this.view = view;
        }

        /// <summary>
        /// Initializes the specified first type.
        /// </summary>
        /// <param name="firstType">Type of the first.</param>
        /// <param name="secondType">Type of the second.</param>
        public void Initialize(Type firstType, Type secondType)
        {
            view.AddMappingButtonEnabled(false);
            view.RemoveMappingButtonEnabled(false);

            PublicPropertyCollection firstTypeList = new PublicPropertyCollection(firstType);
            view.AddFirstTypeListTypes(firstTypeList);

            PublicPropertyCollection secondTypeList = new PublicPropertyCollection(secondType);
            view.AddSecondTypeListTypes(secondTypeList);
        }

        /// <summary>
        /// Elements the selected for mapping.
        /// </summary>
        /// <param name="firstSelectedItem">The first selected item.</param>
        /// <param name="secondSelectedItem">The second selected item.</param>
        public void ElementSelectedForMapping(object firstSelectedItem, object secondSelectedItem)
        {
            bool mappingIsDefined = firstSelectedItem != null && secondSelectedItem != null;
            view.AddMappingButtonEnabled(mappingIsDefined);
        }

        /// <summary>
        /// Creates the mapping button clicked.
        /// </summary>
        /// <param name="firstProperty">The first property.</param>
        /// <param name="secondProperty">The second property.</param>
        public void CreateMappingButtonClicked(object firstProperty, object secondProperty)
        {
            PublicPropertyListItem firstItem = (PublicPropertyListItem)firstProperty;
            PublicPropertyListItem secondItem = (PublicPropertyListItem)secondProperty;
            MappedProperty mappedProperty = new MappedProperty(firstItem, secondItem);

            if (!firstItem.CanMapTo(secondItem) && !secondItem.CanMapTo(firstItem))
            {
                view.NotifyUserOfInvalidMapping(mappedProperty);
                return;
            }

            view.RemoveTypeFromFirstList(firstItem);
            view.RemoveTypeFromSecondList(secondItem);
            view.AddMappedProperty(mappedProperty);
        }

        /// <summary>
        /// Removes the mapping button clicked.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public void RemoveMappingButtonClicked(object itemClicked)
        {
            MappedProperty mappedProperty = (MappedProperty)itemClicked;

            view.AddPropertyToFirstList(mappedProperty.FirstPropertyListItem);
            view.AddPropertyToSecondList(mappedProperty.SecondPropertyListItem);
            view.RemoveMappedProperty(mappedProperty);
            view.RemoveMappingButtonEnabled(false);
        }

        /// <summary>
        /// Mappeds the property selected.
        /// </summary>
        /// <param name="property">The property.</param>
		public void MappedPropertySelected()
        {
            view.RemoveMappingButtonEnabled(true);   
        }
    }
}
