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
using Microsoft.FxCop.Sdk;
using System.Reflection;

namespace Microsoft.Practices.FxCop.Rules.WcfSemantic
{
    /// <summary>
    /// Base introspection rule class.
    /// </summary>
    public abstract class SemanticIntrospectionRule : BaseIntrospectionRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:SemanticIntrospectionRule"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected SemanticIntrospectionRule(string name)
			: base(name, "Microsoft.Practices.FxCop.Rules.WcfSemantic.WcfSemanticRules", typeof(SemanticIntrospectionRule).Assembly)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SemanticIntrospectionRule"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="resourceAssembly">The resource assembly.</param>
        protected SemanticIntrospectionRule(string name, string resourceName, Assembly resourceAssembly)
            : base(name, resourceName, resourceAssembly)
        {
        }

        /// <summary>
        /// Gets the target visibility.
        /// </summary>
        /// <value>The target visibility.</value>
        public override TargetVisibilities TargetVisibility
        {
            get { return (TargetVisibilities.All); }
        }
     }
}
