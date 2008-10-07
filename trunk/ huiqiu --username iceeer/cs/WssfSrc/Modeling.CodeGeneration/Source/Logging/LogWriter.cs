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
using System.Threading;
using Microsoft.Practices.RecipeFramework.Library;
using System.Collections;

namespace Microsoft.Practices.Modeling.CodeGeneration.Logging
{
	/// <summary>
	/// The current logging infraestructure might be replaced by EntLib v3
	/// </summary>
	public class LogWriter : IDisposable
	{
		private IList<TraceListener> listeners;
		private bool disposed;

		public LogWriter()
			: this(new List<TraceListener>(new TraceListener[] { new DefaultTraceListener() }))
		{
		}

		public LogWriter(IList<TraceListener> listeners)
		{
			Guard.ArgumentNotNull(listeners, "listeners");

			this.listeners = listeners;
		}

		public IList<TraceListener> Listeners
		{
			get { return listeners; }
		}

		public void Write(LogEntry entry)
		{
			Guard.ArgumentNotNull(entry, "entry");

			TraceEventCache manager = new TraceEventCache();

			foreach(TraceListener listener in this.Listeners)
			{
				try
				{
					if(!listener.IsThreadSafe)
					{
						Monitor.Enter(listener);
					}

					listener.TraceData(manager, string.Empty, entry.Severity, 0, entry);

					listener.Flush();
				}
				finally
				{
					if(!listener.IsThreadSafe)
					{
						Monitor.Exit(listener);
					}
				}
			}
		}

		public void Clear()
		{
			foreach(TraceListener listener in this.Listeners)
			{
				IClearableListener clearableListener = listener as IClearableListener;

				if(clearableListener != null)
				{
					try
					{
						if(!listener.IsThreadSafe)
						{
							Monitor.Enter(listener);
						}

						clearableListener.Clear();
					}
					finally
					{
						if(!listener.IsThreadSafe)
						{
							Monitor.Exit(listener);
						}
					}
				}
			}
		}

		#region IDisposable Members
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(!this.disposed)
			{
				if(disposing)
				{
					foreach(TraceListener listener in this.Listeners)
					{
						listener.Dispose();
					}
				}
			}

			this.disposed = true;
		}

		~LogWriter()
		{
			Dispose(false);
		}
		#endregion
	}
}