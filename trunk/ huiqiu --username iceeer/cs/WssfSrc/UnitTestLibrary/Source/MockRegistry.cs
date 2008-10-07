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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions;
using Microsoft.Win32;
using System.IO;

namespace Microsoft.Practices.UnitTestLibrary
{
    public class MockRegistry : IAccessRegistry
    {
		private bool isWritable;
        private bool isOpened = false;
		private string keyRoot;
        private Dictionary<string, string> keys = new Dictionary<string, string>();

        public MockRegistry()
        {
        }

        public void MockSetValue(RegistryKey root, string name, string value)
        {
			name = Path.Combine(root.ToString(), name);

            if (keys.ContainsKey(name))
                keys[name] = value;
            else
                keys.Add(name, value);
        }

		#region IAccessRegistry Members

		public bool Open(RegistryKey keyRoot, string keyPath, bool writable)
		{
			this.isWritable = writable;
			this.keyRoot = Path.Combine(keyRoot.ToString(), keyPath);

			isOpened = true;
			return true;
		}

		public string GetValue(string name, string defaultValue)
		{
			if (!isOpened)
				throw new InvalidOperationException();

			string value;
			name = Path.Combine(keyRoot, name);

			if (!keys.TryGetValue(name, out value))
				value = defaultValue;
			return value;
		}

		public void Close()
		{
			isOpened = false;
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			Close();
		}

		#endregion
	}
}