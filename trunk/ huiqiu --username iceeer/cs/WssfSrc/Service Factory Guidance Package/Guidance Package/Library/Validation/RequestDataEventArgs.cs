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

namespace Microsoft.Practices.ServiceFactory.Library.Validation
{
    /// <summary>
    /// A small EventArgs class that is used when an event is requesting
    /// some piece of data. Similar to the CancelEventArgs class, but
    /// more general, as it can return any value.
    /// </summary>
    public class RequestDataEventArgs<T> : EventArgs
    {
        private T value;
        private bool valueProvided;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RequestDataEventArgs&lt;T&gt;"/> class.
        /// </summary>
        public RequestDataEventArgs()
            : this(default(T))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RequestDataEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public RequestDataEventArgs(T value)
        {
            this.value = value;
            valueProvided = false;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get { return value; }
            set
            {
                this.value = value;
                valueProvided = true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [value provided].
        /// </summary>
        /// <value><c>true</c> if [value provided]; otherwise, <c>false</c>.</value>
        public bool ValueProvided
        {
            get { return valueProvided; }
        }
    }
}
