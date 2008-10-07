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

//-----------------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
//      Information Contained Herein is Proprietary and Confidential.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions.Properties;
using Microsoft.Practices.RecipeFramework.Library;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions
{
    /// <summary>
    /// Facade for registry access.
    /// </summary>
    /// <remarks>
    /// Decouple from Win32 registry implementation. Only supports a small subset of
    /// registry functionality.
    /// </remarks>
    public class RegistryAccessor : IAccessRegistry
    {
        private string name;
        RegistryKey key;

        public RegistryAccessor()
        {
        }

        #region IAccessRegistry Members

		public bool Open(RegistryKey keyRoot, string keyPath, bool writable)
        {
			Guard.ArgumentNotNull(keyRoot, "keyRoot");
			Guard.ArgumentNotNull(keyPath, "keyPath");

            this.Close();
			name = keyPath;

			key = keyRoot.OpenSubKey(name, writable);
            return key != null;
        }

        public string GetValue(string name, string defaultValue)
        {
			Guard.ArgumentNotNull(name, "name");
			if (key == null)
				throw new InvalidOperationException(Resources.RegistryKeyAccessorClosedKey);
			
			return key.GetValue(name, defaultValue).ToString();
        }

        public void Close()
        {
            if (key != null)
                key.Close();
            key = null;
        }

		#endregion

		#region IDisposable Members

		private bool disposed;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					Close();
				}
			}

			disposed = true;
		}

        #endregion
    }
}
