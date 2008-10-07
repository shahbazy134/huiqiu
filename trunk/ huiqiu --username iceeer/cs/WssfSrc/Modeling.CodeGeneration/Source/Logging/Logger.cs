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
using System.Diagnostics;
using Microsoft.Practices.RecipeFramework.Library;

namespace Microsoft.Practices.Modeling.CodeGeneration.Logging
{
	/// <summary>
	/// The current logging infraestructure might be replaced by EntLib v3
	/// </summary>
	public static class Logger
	{
		private const string DefaultTitle = "";
		private const TraceEventType DefaultSeverity = TraceEventType.Error;
		private const int DefaultEventId = 1;
		private static object sync = new object();
		private static volatile LogWriter writer;
		private static LogWriterFactory factory = new LogWriterFactory();

		public static void Write(object message)
		{
			Write(message, DefaultTitle, DefaultSeverity, DefaultEventId);
		}

		public static void Write(object message, string title)
		{
			Write(message, title, DefaultSeverity, DefaultEventId);
		}

		public static void Write(object message, string title, TraceEventType severity, int eventId)
		{
			Guard.ArgumentNotNull(message, "message");

			LogEntry entry = new LogEntry(message, title, severity, eventId);

			Write(entry);
		}

		public static void Write(LogEntry entry)
		{
			Guard.ArgumentNotNull(entry, "entry");

			Writer.Write(entry);
		}

		public static void Reset()
		{
			lock(sync)
			{
				LogWriter oldWriter = writer;

				writer = null;

				if(oldWriter != null)
				{
					oldWriter.Dispose();
				}
			}
		}

		public static void Clear()
		{
			Writer.Clear();
		}

		private static LogWriter Writer
		{
			get
			{
				if(writer == null)
				{
					lock(sync)
					{
						if(writer == null)
						{
							writer = factory.Create();
						}
					}
				}

				return writer;
			}
		}
	}
}