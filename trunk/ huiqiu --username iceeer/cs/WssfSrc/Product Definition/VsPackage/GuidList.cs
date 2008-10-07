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

// GuidList.cs
// MUST match guids.h
using System;

namespace Microsoft.Practices.ServiceFactory.VsPkg
{
	static class GuidList
	{
		// These cannot be static readonly because they are uses as attribute arguments.
		public const string guidWebServicesFactoryPkgString = "3a053d37-49a0-4713-a3c4-6161158fb0c4";

		public static readonly Guid guidWebServicesFactoryPkg = new Guid(guidWebServicesFactoryPkgString);
	};
}