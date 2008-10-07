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

using Microsoft.Practices.Common;
using Microsoft.Practices.ComponentModel;
using Microsoft.Practices.RecipeFramework;
using System;
using System.ComponentModel.Design; 

namespace Microsoft.Practices.ServiceFactory.ValueProviders
{
    /// <summary>
    /// Returns a class name
    /// </summary>
    [ServiceDependency(typeof(IDictionaryService))]
    public class ClassNameProvider : ValueProvider, IAttributesConfigurable
    {
        string fullyQualifiedClassName;

        /// <summary>
        /// Contains code that will be called when recipe execution begins. This is the first method in the lifecycle.
        /// </summary>
        /// <param name="currentValue">An <see cref="T:System.Object"/> that contains the current value of the argument.</param>
        /// <param name="newValue">When this method returns, contains an <see cref="T:System.Object"/> that contains
        /// the new value of the argument, if the returned value
        /// of the method is <see langword="true"/>. Otherwise, it is ignored.</param>
        /// <returns>
        /// 	<see langword="true"/> if the argument value should be replaced with
        /// the value in <paramref name="newValue"/>; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>By default, always returns <see langword="false"/>, unless overriden by a derived class.</remarks>
        public override bool OnBeginRecipe(object currentValue, out object newValue)
        {
            return GetValue(currentValue, out newValue);
        }

        /// <summary>
        /// Contains code that will be called whenever an argument monitored by the value provider
        /// changes, as specified in the configuration file.
        /// </summary>
        /// <param name="changedArgumentName">The name of the argument being monitored that changed.</param>
        /// <param name="changedArgumentValue">An <see cref="T:System.Object"/> that contains the value of the monitored argument.</param>
        /// <param name="currentValue">An <see cref="T:System.Object"/> that contains the current value of the argument.</param>
        /// <param name="newValue">When this method returns, contains an <see cref="T:System.Object"/> that contains
        /// the new value of the argument, if the returned value
        /// of the method is <see langword="true"/>. Otherwise, it is ignored.</param>
        /// <returns>
        /// 	<see langword="true"/> if the argument value should be replaced with
        /// the value in <paramref name="newValue"/>; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>By default, always returns <see langword="false"/>, unless overriden by a derived class.</remarks>
        public override bool OnArgumentChanged(string changedArgumentName, object changedArgumentValue, object currentValue, out object newValue)
        {
            return GetValue(currentValue, out newValue);
        }

        /// <summary>
        /// Contains code that will be called before actions are executed.
        /// </summary>
        /// <param name="currentValue">An <see cref="T:System.Object"/> that contains the current value of the argument.</param>
        /// <param name="newValue">When this method returns, contains an <see cref="T:System.Object"/> that contains
        /// the new value of the argument, if the returned value
        /// of the method is <see langword="true"/>. Otherwise, it is ignored.</param>
        /// <returns>
        /// 	<see langword="true"/> if the argument value should be replaced with
        /// the value in <paramref name="newValue"/>; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>By default, always returns <see langword="false"/>, unless overridden by a derived class.</remarks>
        public override bool OnBeforeActions(object currentValue, out object newValue)
        {
            return GetValue(currentValue, out newValue);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="currentValue">The current value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns></returns>
        private bool GetValue(object currentValue, out object newValue)
        {
            newValue = "(null)";

            IDictionaryService dictservice = (IDictionaryService)ServiceHelper.GetService(this, typeof(IDictionaryService));
            Type type = dictservice.GetValue(fullyQualifiedClassName) as Type;

            if (type == null) return false;

            newValue = ClassNameParser.Parse(type.FullName);

            return !object.Equals(currentValue, newValue);
        }

        /// <summary>
        /// Configures the component using the dictionary of attributes specified
        /// in the configuration file.
        /// </summary>
        /// <param name="attributes">The attributes in the configuration element.</param>
        public void Configure(System.Collections.Specialized.StringDictionary attributes)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");
            
            if (attributes.ContainsKey("FullyQualifiedClassName"))
            {
                fullyQualifiedClassName = attributes["FullyQualifiedClassName"];
            }
        }

    }
}
