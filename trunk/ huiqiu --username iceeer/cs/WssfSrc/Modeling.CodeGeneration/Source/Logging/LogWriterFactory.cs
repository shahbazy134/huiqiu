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
using Microsoft.Practices.Modeling.CodeGeneration.Logging.Listeners;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.Modeling.CodeGeneration.Logging
{
	/// <summary>
	/// The current logging infraestructure might be replaced by EntLib v3
	/// </summary>
	public class LogWriterFactory
	{
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public LogWriter Create()
		{
			List<TraceListener> listeners = new List<TraceListener>();

			listeners.Add(new VSOutputWindowListener());
			listeners.Add(new VSErrorListWindowListener());

			return new LogWriter(listeners);
		}
	}
}