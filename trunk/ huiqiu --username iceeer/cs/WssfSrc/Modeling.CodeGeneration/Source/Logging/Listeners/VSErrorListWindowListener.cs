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
using Microsoft.Practices.Modeling.CodeGeneration.Logging.Helpers;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio;

namespace Microsoft.Practices.Modeling.CodeGeneration.Logging.Listeners
{
	public class VSErrorListWindowListener : TraceListener, IClearableListener
	{
		#region Properties
		private ErrorListProvider errorListProvider;

		protected ErrorListProvider ErrorListProvider
		{
			get
			{
				if(errorListProvider == null)
				{
					DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;

					if(dte == null)
					{
						throw new LoggingException(Properties.Resources.NullDTE);
					}

					IServiceProvider serviceProvider =
						new ServiceProvider(dte as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);

					if(serviceProvider == null)
					{
						throw new LoggingException(Properties.Resources.NullServiceProvider);
					}

					errorListProvider = new ErrorListProvider(serviceProvider);
				}

				return errorListProvider;
			}
		}
		#endregion

		public override void Write(string message)
		{
			WriteError(message);
		}

		public override void WriteLine(string message)
		{
			Write(message);
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			if(data is LogEntry)
			{
				WriteError((LogEntry)data);
			}
			else if(data is string)
			{
				Write(data);
			}
			else
			{
				base.TraceData(eventCache, source, eventType, id, data);
			}
		}

		#region IClearableListener Members

		public void Clear()
		{
			this.ErrorListProvider.Tasks.Clear();
		}

		#endregion

		#region Protected Implementation
		protected virtual void WriteError(string message)
		{
			ErrorTask errorTask = new ErrorTask();

			errorTask.CanDelete = false;
			errorTask.ErrorCategory = TaskErrorCategory.Error;
			errorTask.Text = message;

			WriteError(errorTask);
		}

		protected virtual void WriteError(LogEntry logEntry)
		{
			ErrorTask errorTask = new ErrorTask();

			errorTask.CanDelete = false;

			switch(logEntry.Severity)
			{
				case TraceEventType.Critical:
					errorTask.ErrorCategory = TaskErrorCategory.Error;
					break;
				case TraceEventType.Error:
					errorTask.ErrorCategory = TaskErrorCategory.Error;
					break;
				case TraceEventType.Information:
					errorTask.ErrorCategory = TaskErrorCategory.Message;
					break;
				case TraceEventType.Warning:
					errorTask.ErrorCategory = TaskErrorCategory.Warning;
					break;
				default:
					errorTask.ErrorCategory = TaskErrorCategory.Error;
					break;
			}

			errorTask.Text = logEntry.Message;

			WriteError(errorTask);
		}

		protected virtual void WriteError(ErrorTask errorTask)
		{
			this.ErrorListProvider.Tasks.Add(errorTask);
			this.ErrorListProvider.Show();
		}
		#endregion
	}
}