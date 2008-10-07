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
using Microsoft.Practices.ServiceFactory.Library.Validation;

namespace Microsoft.Practices.ServiceFactory.Recipes.CreateTranslator.Presentation
{
    public interface ICreateTranslatorPropertiesView
    {
        string MappingClassNamespace { set; get; }
        Type FirstType { set; get; }
        Type SecondType { set; get; }
        string MappingClassName { set; get; }

        event EventHandler<ValidationEventArgs> MappingClassNameChanged;
        event EventHandler<ValidationEventArgs> MappingClassNamespaceChanged;
        event EventHandler<ValidationEventArgs> FirstTypeChanged;
        event EventHandler<ValidationEventArgs> SecondTypeChanged;
		event EventHandler<RequestDataEventArgs<bool>> RequestIsDataValid;
    }
}