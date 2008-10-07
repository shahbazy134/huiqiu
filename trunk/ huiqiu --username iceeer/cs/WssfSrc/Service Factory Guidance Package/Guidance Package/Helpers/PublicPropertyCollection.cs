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
using System.Reflection;

namespace Microsoft.Practices.ServiceFactory.Helpers
{
    /// <summary/>
    [Serializable]
    public class PublicPropertyCollection : List<PublicPropertyListItem>
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="T:PublicPropertyCollection"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public PublicPropertyCollection(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            
            foreach (PropertyInfo publicProperty in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                Add(new PublicPropertyListItem(publicProperty));
            }
        }
    }
}
