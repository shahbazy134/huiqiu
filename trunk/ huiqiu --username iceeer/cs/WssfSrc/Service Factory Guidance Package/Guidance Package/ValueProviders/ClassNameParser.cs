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

namespace Microsoft.Practices.ServiceFactory.ValueProviders
{
    /// <summary>
    /// Helper to parse a class name
    /// </summary>
    public static class ClassNameParser
    {
        /// <summary>
        /// Parses the specified complete class name.
        /// </summary>
        /// <param name="completeClassName">Name of the complete class.</param>
        /// <returns></returns>
        public static string Parse(string completeClassName)
        {
            if (String.IsNullOrEmpty(completeClassName)) return "";

            string[] classNameElements = completeClassName.Split('.');
            return classNameElements[classNameElements.Length - 1];
        }
    }
}