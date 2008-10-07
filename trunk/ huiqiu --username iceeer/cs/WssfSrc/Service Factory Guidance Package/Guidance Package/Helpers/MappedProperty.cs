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

namespace Microsoft.Practices.ServiceFactory.Helpers
{
    /// <summary>
    /// Helper class for the mapped properties.
    /// </summary>
    [Serializable]
    public class MappedProperty
    {
        PublicPropertyListItem firstProperty;
        PublicPropertyListItem secondProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MappedProperty"/> class.
        /// </summary>
        /// <param name="firstProperty">The first property.</param>
        /// <param name="secondProperty">The second property.</param>
        public MappedProperty(PublicPropertyListItem firstProperty, PublicPropertyListItem secondProperty)
        {
            this.firstProperty = firstProperty;
            this.secondProperty = secondProperty;
        }

        /// <summary>
        /// Gets the first property list item.
        /// </summary>
        /// <value>The first property list item.</value>
        public PublicPropertyListItem FirstPropertyListItem { get { return firstProperty; } }
        /// <summary>
        /// Gets the second property list item.
        /// </summary>
        /// <value>The second property list item.</value>
        public PublicPropertyListItem SecondPropertyListItem { get { return secondProperty; } }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, 
                Properties.Resources.MappedPropertyToString, 
                firstProperty.ToString(), 
                GetMappingDirectionString(), 
                secondProperty.ToString());
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            MappedProperty other = obj as MappedProperty;
            if (other == null) return false;

            return firstProperty.Equals(other.firstProperty) && secondProperty.Equals(other.secondProperty);
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

        private string GetMappingDirectionString()
        {
            if (firstProperty.CanMapTo(secondProperty) && secondProperty.CanMapTo(firstProperty))
                return "<->";

            if (firstProperty.CanMapTo(secondProperty))
                return "->";

            if (secondProperty.CanMapTo(firstProperty))
                return "<-";

            return "";
        }
    }
}
