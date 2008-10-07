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
using Microsoft.Practices.ServiceFactory.Helpers;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateTranslator.Presentation
{
    public interface ICreateTranslatorMappingView
    {
        /// <summary>
        /// Adds the mapping button enabled.
        /// </summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        void AddMappingButtonEnabled(bool isEnabled);
        /// <summary>
        /// Removes the mapping button enabled.
        /// </summary>
        /// <param name="isEnabled">if set to <c>true</c> [is enabled].</param>
        void RemoveMappingButtonEnabled(bool isEnabled);

        /// <summary>
        /// Adds the first type list types.
        /// </summary>
        /// <param name="firstTypeListNames">The first type list names.</param>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		void AddFirstTypeListTypes(PublicPropertyCollection firstTypeListNames);
        /// <summary>
        /// Adds the second type list types.
        /// </summary>
        /// <param name="secondTypeListNames">The second type list names.</param>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		void AddSecondTypeListTypes(PublicPropertyCollection secondTypeListNames);

        /// <summary>
        /// Adds the mapped property.
        /// </summary>
        /// <param name="mappedProperty">The mapped property.</param>
        void AddMappedProperty(MappedProperty mappedProperty);

        /// <summary>
        /// Adds the property to first list.
        /// </summary>
        /// <param name="property">The property.</param>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords")]
        void AddPropertyToFirstList(PublicPropertyListItem property);
        
        /// <summary>
        /// Adds the property to second list.
        /// </summary>
        /// <param name="property">The property.</param>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords")]
        void AddPropertyToSecondList(PublicPropertyListItem property);

        /// <summary>
        /// Removes the type from first list.
        /// </summary>
        /// <param name="secondProperty">The second property.</param>
        void RemoveTypeFromFirstList(PublicPropertyListItem secondProperty);
        /// <summary>
        /// Removes the type from second list.
        /// </summary>
        /// <param name="secondProperty">The second property.</param>
        void RemoveTypeFromSecondList(PublicPropertyListItem secondProperty);

        /// <summary>
        /// Removes the mapped property.
        /// </summary>
        /// <param name="mappedProperty">The mapped property.</param>
        void RemoveMappedProperty(MappedProperty mappedProperty);

        /// <summary>
        /// Notifies the user of invalid mapping.
        /// </summary>
        /// <param name="badMapping">The bad mapping.</param>
        void NotifyUserOfInvalidMapping(MappedProperty badMapping);
    }
}
