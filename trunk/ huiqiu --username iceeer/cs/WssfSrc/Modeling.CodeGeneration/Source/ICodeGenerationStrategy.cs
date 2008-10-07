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
using Microsoft.Practices.Modeling.CodeGeneration.Artifacts;
using Microsoft.Build.Framework;
using System.Runtime.InteropServices;
using Microsoft.Build.BuildEngine;
using Microsoft.Practices.VisualStudio.Helper;

namespace Microsoft.Practices.Modeling.CodeGeneration
{
	[ComVisible(true)]
	[Guid("556CE6BA-F065-424d-99E2-286AACA7B58A")]
	public interface ICodeGenerationStrategy
	{
		/// <summary>
		/// Generates code according to the specified link information.
		/// </summary>
		/// <param name="link">The link.</param>
		/// <returns>A collection of file names and code generated content.</returns>
		CodeGenerationResults Generate(IArtifactLink link);

		/// <summary>
		/// Gets the errors.
		/// </summary>
		/// <value>The errors.</value>
		IList<Logging.LogEntry> Errors { get; }

		/// <summary>
		/// Gets the project references.
		/// </summary>
		/// <value>The project references.</value>
		IList<Guid> ProjectReferences { get;}

		/// <summary>
		/// Gets the assembly references.
		/// </summary>
		/// <value>The project references.</value>
		IList<string> AssemblyReferences { get;}
	}
}
