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
using System.Reflection;
using System.Globalization;

namespace Microsoft.Practices.ServiceFactory.RecipeFramework.Extensions
{
	/// <summary>
	/// Automatically resolve assemblies across contexts. 
	/// </summary>
	public class AssemblyResolver
	{
		public Assembly Resolve(string assemblyName)
		{
			if (String.IsNullOrEmpty(assemblyName))
				return null;

			AssemblyName requestedAssemblyName = new AssemblyName(assemblyName);

			foreach (Assembly cachedAssembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				AssemblyName cachedAssemblyName = cachedAssembly.GetName();
				if (cachedAssemblyName.Name == requestedAssemblyName.Name)
				{
					if (requestedAssemblyName.Version != null &&
						requestedAssemblyName.Version.CompareTo(cachedAssemblyName.Version) != 0)
					{
						continue;
					}
					byte[] requestedAsmPublicKeyToken = requestedAssemblyName.GetPublicKeyToken();
					if (requestedAsmPublicKeyToken != null)
					{
						byte[] cachedAssemblyPublicKeyToken = cachedAssemblyName.GetPublicKeyToken();

						if (Convert.ToBase64String(requestedAsmPublicKeyToken) != Convert.ToBase64String(cachedAssemblyPublicKeyToken))
						{
							continue;
						}
					}

					CultureInfo requestedAssemblyCulture = requestedAssemblyName.CultureInfo;
					if (requestedAssemblyCulture != null && requestedAssemblyCulture.LCID != CultureInfo.InvariantCulture.LCID)
					{
						if (cachedAssemblyName.CultureInfo.LCID != requestedAssemblyCulture.LCID)
						{
							continue;
						}
					}

					return cachedAssembly;
				}
			}
			return null;
		}
	}
}
