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
using EnvDTE80;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.Practices.Modeling.CodeGeneration.Logging.Helpers;
using System.Globalization;

namespace Microsoft.Practices.Modeling.CodeGeneration.Logging.Listeners
{
	public class VSOutputWindowListener : TraceListener, IClearableListener
	{
		private const string PaneName = "Services Software Factory";
		private OutputWindowHelper outputWindowHelper;

		protected OutputWindowHelper OutputWindowHelper
		{
			get
			{
				if(outputWindowHelper == null)
				{
					DTE2 dte = Package.GetGlobalService(typeof(DTE)) as DTE2;

					if(dte == null)
					{
						throw new LoggingException(Properties.Resources.NullDTE);
					}

					outputWindowHelper = new OutputWindowHelper(dte, PaneName);
				}

				return outputWindowHelper;
			}
		}

		public override void Write(string message)
		{
			WriteLine(message);
		}

		public override void WriteLine(string message)
		{
			this.OutputWindowHelper.WriteMessage(
				string.Format(CultureInfo.CurrentCulture,"{0}{1}", message, Environment.NewLine));
		}

		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			if(data is LogEntry || 
			   data is string)
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
			this.OutputWindowHelper.Clear();
		}

		#endregion
	}
}