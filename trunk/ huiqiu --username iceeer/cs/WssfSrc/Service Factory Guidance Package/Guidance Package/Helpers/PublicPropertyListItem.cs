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
using System.Globalization;
using System.Reflection;

namespace Microsoft.Practices.ServiceFactory.Helpers
{
    /// <summary/>
    [Serializable]
    public class PublicPropertyListItem
    {
        PropertyInfo property;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PublicPropertyListItem"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public PublicPropertyListItem(PropertyInfo property)
        {
            this.property = property;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, 
                Properties.Resources.PublicPropertyListItemToString, 
                property.Name, 
                property.PropertyType.Name);
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>The property.</value>
        public PropertyInfo Property { get { return property; } }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            PublicPropertyListItem other = obj as PublicPropertyListItem;
            if (other == null) return false;

            return property.Equals(other.property);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="M:System.Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Determines whether this instance [can map to] the specified your list item.
        /// </summary>
        /// <param name="yourListItem">Your list item.</param>
        /// <returns>
        /// 	<c>true</c> if this instance [can map to] the specified your list item; otherwise, <c>false</c>.
        /// </returns>
        public bool CanMapTo(PublicPropertyListItem yourListItem)
        {
            if (yourListItem == null)
                throw new ArgumentNullException("yourListItem");
            
            bool canBeWritten = yourListItem.Property.CanWrite && property.CanRead;
            bool canBeAssigned = yourListItem.Property.PropertyType.IsAssignableFrom(Property.PropertyType);

            return canBeWritten && canBeAssigned;
        }
    }
}
