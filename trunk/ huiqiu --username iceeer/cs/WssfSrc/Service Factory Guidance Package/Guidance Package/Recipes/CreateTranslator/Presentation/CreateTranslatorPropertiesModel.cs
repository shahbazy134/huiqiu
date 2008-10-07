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
using System.ComponentModel.Design;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateTranslator.Presentation
{
    public class CreateTranslatorPropertiesModel
    {
        private const string MappingClassNameKey = "MappingClassName";
        private const string MappingClassNamespaceKey = "ServiceImplementationNamespace";
        private const string FirstTypeKey = "FirstType";
        private const string SecondTypeKey = "SecondType";
        private IDictionaryService dictionary;

        public string MappingClassName
        {
            get { return (string)dictionary.GetValue(MappingClassNameKey); }
            set { dictionary.SetValue(MappingClassNameKey, value); }
        }

        public string MappingNamespace
        {
            get { return (string)dictionary.GetValue(MappingClassNamespaceKey); }
            set { dictionary.SetValue(MappingClassNamespaceKey, value); }
        }

        public Type FirstType
        {
            get { return dictionary.GetValue(FirstTypeKey) as Type; }
            set { dictionary.SetValue(FirstTypeKey, value); }
        }

        public Type SecondType
        {
            get { return dictionary.GetValue(SecondTypeKey) as Type; }
            set { dictionary.SetValue(SecondTypeKey, value); }
        }

        public bool IsValid
        {
            get
            {
                return (!string.IsNullOrEmpty(MappingClassName) &&
                        !string.IsNullOrEmpty(MappingNamespace) &&
                        FirstType != null &&
                        SecondType != null);
            }
        }

        public CreateTranslatorPropertiesModel(IDictionaryService dictionary)
        {
            this.dictionary = dictionary;
        }
    }
}