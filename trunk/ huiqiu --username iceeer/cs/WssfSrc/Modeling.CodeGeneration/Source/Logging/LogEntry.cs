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
using System.Xml.Serialization;
using System.Diagnostics;
using Microsoft.Practices.Modeling.CodeGeneration.Logging.Formatters;
using System.Globalization;
using Microsoft.Practices.RecipeFramework.Library;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Practices.Modeling.CodeGeneration.Logging
{
	/// <summary>
	/// The current logging infraestructure might be replaced by EntLib v3
	/// </summary>
	[XmlRoot("LogEntry")]
	[Serializable]
	public class LogEntry
	{
		private static readonly TextFormatter toStringFormatter = new TextFormatter();

		public LogEntry(object logMessage, string logTitle, TraceEventType logSeverity, int eventIdentifier)
		{
			Guard.ArgumentNotNull(logMessage, "logMessage");

			this.message = logMessage.ToString();
			this.title = logTitle;
			this.severity = logSeverity;
			this.eventId = eventIdentifier;

			InitializeLogEntry();
		}

		private string title;

		public string Title
		{
			get { return this.title; }
			set { this.title = value; }
		}

		private string message;

		public string Message
		{
			get { return this.message; }
			set { this.message = value; }
		}

		private TraceEventType severity;

		public TraceEventType Severity
		{
			get { return this.severity; }
			set { this.severity = value; }
		}

		public string LoggedSeverity
		{
			get { return severity.ToString(); }
		}

		private int eventId;

		public int EventId
		{
			get { return this.eventId; }
			set { this.eventId = value; }
		}

		private DateTime timeStamp;

		public DateTime TimeStamp
		{
			get { return this.timeStamp; }
			set { this.timeStamp = value; }
		}

		public string TimeStampString
		{
			get { return TimeStamp.ToString(CultureInfo.CurrentCulture); }
		}

		public override string ToString()
		{
			return toStringFormatter.Format(this);
		}

		private void InitializeLogEntry()
		{
			this.TimeStamp = DateTime.UtcNow;
		}
	}
}