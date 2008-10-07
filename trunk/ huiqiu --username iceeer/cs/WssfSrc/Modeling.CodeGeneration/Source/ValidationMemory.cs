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
using Microsoft.VisualStudio.Modeling;

namespace Microsoft.Practices.Modeling.CodeGeneration
{
	/// <summary>
	/// Simple class that remembers which ModelElements have been visited
	/// </summary>
	public class ValidationMemory
	{
		private Dictionary<Guid, bool> validatedElements = new Dictionary<Guid, bool>();

		public void Reset()
		{
			validatedElements.Clear();
		}

		public void Add(Guid elementId)
		{
			if (!validatedElements.ContainsKey(elementId))
				validatedElements.Add(elementId, true);
		}

		public bool IsValidated(Guid elementId)
		{
			return validatedElements.ContainsKey(elementId);
		}
	}
}
