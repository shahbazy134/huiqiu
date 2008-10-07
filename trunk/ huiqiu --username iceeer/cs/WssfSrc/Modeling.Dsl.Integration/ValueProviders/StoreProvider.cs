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
using Microsoft.Practices.RecipeFramework;
using Microsoft.Practices.Modeling.Dsl.Integration.Helpers;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace Microsoft.Practices.Modeling.Dsl.Integration.ValueProviders
{
    public class StoreProvider : DiagramProvider
    {
        public override bool OnBeginRecipe(object currentValue, out object newValue)
        {
            bool valid = base.OnBeginRecipe(currentValue, out newValue);

            if (valid)
            {
                Diagram diagram = (Diagram)newValue;
                if (diagram == null)
                {
                    return false;
                }
                newValue = diagram.Partition.Store;
            }

            return valid;
        }
    }
}
