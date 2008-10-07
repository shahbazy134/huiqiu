#define TRACE
#define TRACE_INSTANTIATION
#define TRACE_BEHAVIOR


using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Threading;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Reflection;
using System.Linq;


namespace CSharpRecipes
{
	public class Diagnostics
	{
        #region "8.1 Providing Fine Grained Control over Debugging/Tracing Output"
		public static void EnableTracing()
		{
			// This method should be conditionally compiled using the TRACE directive, otherwise
			//   this code will get compiled and run when TRACE is turned off
			BooleanSwitch DBSwitch = new BooleanSwitch("DatabaseSwitch", "Switch for database tracing");
			Console.WriteLine("DBSwitch Enabled = " + DBSwitch.Enabled);
			Console.WriteLine("Description = " + DBSwitch.Description);
			Console.WriteLine("DisplayName = " + DBSwitch.DisplayName);

			BooleanSwitch UISwitch = new BooleanSwitch("UISwitch", "Switch for user interface tracing");
			Console.WriteLine("UISwitch Enabled = " + UISwitch.Enabled);
			Console.WriteLine("Description = " + UISwitch.Description);
			Console.WriteLine("DisplayName = " + UISwitch.DisplayName);

			BooleanSwitch ExceptionSwitch = new BooleanSwitch("ExceptionSwitch", "Switch for tracing thrown exceptions");
			Console.WriteLine("ExceptionSwitch Enabled = " + ExceptionSwitch.Enabled);
			Console.WriteLine("Description = " + ExceptionSwitch.Description);
			Console.WriteLine("DisplayName = " + ExceptionSwitch.DisplayName);
		}
	    #endregion
			
        #region "8.2 Determining If A Process Has Stopped Responding"
        public enum ProcessRespondingState
        {
            Responding,
            NotResponding,
            Unknown
        }

        public static ProcessRespondingState IsProcessResponding(Process process)
        {
	        if (process.MainWindowHandle == IntPtr.Zero)
	        {
		        Trace.WriteLine("{0} does not have a MainWindowHandle",
                    process.ProcessName);
                return ProcessRespondingState.Unknown;
	        }
	        else
	        {
		        // This process has a MainWindowHandle
		        if (!process.Responding)
		        {
			        Trace.WriteLine("{0} is not responding.",process.ProcessName);
			        return ProcessRespondingState.NotResponding;
		        }
		        else
		        {
			        Trace.WriteLine("{0} is responding.",process.ProcessName);
			        return ProcessRespondingState.Responding;
		        }
	        }
        }
        #endregion

        #region "8.3 Using One or More Event Logs in Your Application"
        public static void TestEventLogClass()
		{
			// Causes and exception
			//AppEvents AppEventLog1 = new AppEvents("AppLog", "AppLocal");
			//AppEvents GlobalEventLog1 = new AppEvents("Application", "AppLocal");
			//GlobalEventLog1.WriteToLog("",EventLogEntryType.Information, CategoryType.AppStartUp, EventIDType.ExceptionThrown);
            
            AppEvents AE = null;
            try
            {
                AE = new AppEvents("", "MYSOURCE");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            AE.WriteToLog("MESSAGE", EventLogEntryType.Information, CategoryType.AppStartUp, EventIDType.ExceptionThrown, new byte[10] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 });
            AE.WriteToLog("MESSAGE", EventLogEntryType.Information, CategoryType.ReadFromDB, EventIDType.Read);

            System.Threading.Thread.Sleep(250);

            foreach (EventLogEntry Entry in AE.GetEntries())
            {
                Console.WriteLine("\r\nMessage:        " + Entry.Message);
                Console.WriteLine("Category:       " + Entry.Category);
                Console.WriteLine("CategoryNumber: " + Entry.CategoryNumber);
                Console.WriteLine("EntryType:      " + Entry.EntryType.ToString());
                Console.WriteLine("InstanceId:     " + Entry.InstanceId);
                Console.WriteLine("Index:          " + Entry.Index);
                Console.WriteLine("MachineName:    " + Entry.MachineName);
                Console.WriteLine("Source:         " + Entry.Source);
                Console.WriteLine("TimeGenerated:  " + Entry.TimeGenerated);
                Console.WriteLine("TimeWritten:    " + Entry.TimeWritten);
                Console.WriteLine("UserName:       " + Entry.UserName);

                foreach (byte data in Entry.Data)
                    Console.WriteLine("\tData: " + data);
            }

            AE.ClearLog();
            AE.CloseLog();
            AE.DeleteLog();
			
			
			// CreateMultipleLogs
            AppEvents AppEventLog = new AppEvents("AppLog", "AppLocal");
            AppEvents GlobalEventLog = new AppEvents("System", "AppGlobal");

            ListDictionary LogList = new ListDictionary();
            LogList.Add(AppEventLog.Name, AppEventLog);
            LogList.Add(GlobalEventLog.Name, GlobalEventLog);
	
            ((AppEvents)LogList[AppEventLog.Name]).WriteToLog("App startup",
                EventLogEntryType.Information, CategoryType.AppStartUp, 
                EventIDType.ExceptionThrown);

            ((AppEvents)LogList[GlobalEventLog.Name]).WriteToLog("App startup security check",
                EventLogEntryType.Information, CategoryType.AppStartUp, 
                EventIDType.BufferOverflowCondition);

            foreach (DictionaryEntry Log in LogList)
            {
	            ((AppEvents)Log.Value).WriteToLog("App startup", 
                    EventLogEntryType.FailureAudit, 
		            CategoryType.AppStartUp, EventIDType.SecurityFailure);
            }

            foreach (DictionaryEntry Log in LogList)
            {
	            ((AppEvents)Log.Value).DeleteLog();
            }
            LogList.Clear();

		}

        public enum EventIDType
        {
	        NA = 0,
	        Read = 1,
	        Write = 2,
	        ExceptionThrown = 3,
	        BufferOverflowCondition = 4,
	        SecurityFailure = 5,
	        SecurityPotentiallyCompromised = 6 
        }

        public enum CategoryType : short
        {
	        None = 0,
	        WriteToDB = 1,
	        ReadFromDB = 2,
	        WriteToFile = 3,
	        ReadFromFile = 4,
	        AppStartUp = 5,
	        AppShutDown = 6,
	        UserInput = 7	
        }

        public class AppEvents
        {
	        // Constructors
	        public AppEvents(string logName) : 
		        this(logName, Process.GetCurrentProcess().ProcessName, ".") {}

	        public AppEvents(string logName, string source) : this(logName, source, ".") {}

	        public AppEvents(string logName, string source, string machineName)
	        {
		        this.logName = logName;
		        this.source = source;
		        this.machineName = machineName;

                if (!EventLog.SourceExists(source, machineName)) 
                {
                    EventSourceCreationData sourceData = 
                        new EventSourceCreationData(source, logName);
                    sourceData.MachineName = machineName;

	                EventLog.CreateEventSource(sourceData);
                }

		        log = new EventLog(logName, machineName, source);
		        log.EnableRaisingEvents = true;
	        }


	        // Fields
	        private EventLog log = null;
	        private string source = "";
	        private string logName = "";
	        private string machineName = ".";


	        // Properties
	        public string Name
	        {
		        get{return (logName);}
	        }

	        public string SourceName
	        {
		        get{return (source);}
	        }

	        public string Machine
	        {
		        get{return (machineName);}
	        }


	        // Methods
	        public void WriteToLog(string message, EventLogEntryType type, 
		        CategoryType category, EventIDType eventID)
	        {
		        if (log == null)
		        {	
			        throw (new ArgumentNullException("log", 
				        "This Event Log has not been opened or has been closed."));
		        }

		        log.WriteEntry(message, type, (int)eventID, (short)category);
	        }

	        public void WriteToLog(string message, EventLogEntryType type, 
		        CategoryType category, EventIDType eventID, byte[] rawData)
	        {
		        if (log == null)
		        {
			        throw (new ArgumentNullException("log", 
				        "This Event Log has not been opened or has been closed."));
		        }

		        log.WriteEntry(message, type, (int)eventID, (short)category, rawData);
	        }

	        public EventLogEntryCollection GetEntries()
	        {
		        if (log == null)
		        {
			        throw (new ArgumentNullException("log", 
				        "This Event Log has not been opened or has been closed."));
		        }

		        return (log.Entries);
	        }

	        public void ClearLog()
	        {
		        if (log == null)
		        {
			        throw (new ArgumentNullException("log", 
				        "This Event Log has not been opened or has been closed."));
		        }

		        log.Clear();
	        }

	        public void CloseLog()
	        {
		        if (log == null)
		        {
			        throw (new ArgumentNullException("log", 
				        "This Event Log has not been opened or has been closed."));
		        }

		        log.Close();
		        log = null;
	        }

	        public void DeleteLog()
	        {
                if (EventLog.SourceExists(source, machineName)) 
                {
	                EventLog.DeleteEventSource(source, machineName);
                }

                if (logName != "Application" && 
	                logName != "Security" &&
	                logName != "System")
                {
	                if (EventLog.Exists(logName, machineName)) 
	                {
		                EventLog.Delete(logName, machineName);
	                }
                }

                if (log != null)
                {
	                log.Close();
	                log = null;
                }
	        }
        }
		#endregion
	
        #region "8.4 Searching Event Log Entries"
        public static void FindAnEntryInEventLog()
        {
	        EventLog Log = new EventLog("System");

	        EventLogEntry[] Entries = EventLogSearch.FindEntryType(Log.Entries, 
		        EventLogEntryType.Error);

	        foreach (EventLogEntry Entry in Entries)
	        {
		        Console.WriteLine("Message:        " + Entry.Message);
                Console.WriteLine("InstanceId:        " + Entry.InstanceId);
		        Console.WriteLine("Category:       " + Entry.Category);
		        Console.WriteLine("EntryType:      " + Entry.EntryType.ToString());
		        Console.WriteLine("Source:         " + Entry.Source);
	        }
        }

        public static void FindAnEntryInEventLog2()
        {
	        EventLog Log = new EventLog("System");

	        EventLogEntry[] Entries = EventLogSearch.FindTimeGeneratedAtOrAfter(Log.Entries, 
		        DateTime.Parse("8/3/2003"));
	        Entries = EventLogSearch.FindEntryType(Entries, EventLogEntryType.Error);
	        Entries = EventLogSearch.FindInstanceId(Entries, 7000);

	        foreach (EventLogEntry Entry in Entries)
	        {
		        Console.WriteLine("Message:        " + Entry.Message);
                Console.WriteLine("InstanceId:        " + Entry.InstanceId);
		        Console.WriteLine("Category:       " + Entry.Category);
		        Console.WriteLine("EntryType:      " + Entry.EntryType.ToString());
		        Console.WriteLine("Source:         " + Entry.Source);
	        }
        }
		
		
        public sealed class EventLogSearch
        {
	        // Available (Static) Search Methods:			
	        //    FindTimeGeneratedAtOrBefore
	        //    FindTimeGeneratedAtOrAfter
	        //    FindUserName
	        //    FindMachineName
	        //    FindCategory
	        //    FindSource
	        //    FindEntryType
	        //    FindMessage
	        //    FindEventID

	        private EventLogSearch() {}     // Prevent this class from being instantiated.

	        public static EventLogEntry[] FindTimeGeneratedAtOrBefore(
		        IEnumerable logEntries, DateTime timeGeneratedQuery)
	        {
		        ArrayList entries = new ArrayList();

		        foreach (EventLogEntry logEntry in logEntries)
		        {
			        if (logEntry.TimeGenerated <= timeGeneratedQuery)
			        {
				        entries.Add(logEntry);
			        }
		        }

		        EventLogEntry[] entriesArray = new EventLogEntry[entries.Count];
		        entries.CopyTo(entriesArray);
		        return (entriesArray);
	        }

	        public static EventLogEntry[] FindTimeGeneratedAtOrAfter(
		        IEnumerable logEntries, DateTime timeGeneratedQuery)
	        {
		        ArrayList entries = new ArrayList();

		        foreach (EventLogEntry logEntry in logEntries)
		        {
			        if (logEntry.TimeGenerated >= timeGeneratedQuery)
			        {
				        entries.Add(logEntry);
			        }
		        }

		        EventLogEntry[] entriesArray = new EventLogEntry[entries.Count];
		        entries.CopyTo(entriesArray);
		        return (entriesArray);
	        }

	        public static EventLogEntry[] FindUserName(IEnumerable logEntries, 
		        string userNameQuery)
	        {
		        ArrayList entries = new ArrayList();

		        foreach (EventLogEntry logEntry in logEntries)
		        {
			        if (logEntry.UserName == userNameQuery)
			        {
				        entries.Add(logEntry);
			        }
		        }

		        EventLogEntry[] entriesArray = new EventLogEntry[entries.Count];
		        entries.CopyTo(entriesArray);
		        return (entriesArray);
	        }

	        public static EventLogEntry[] FindMachineName(IEnumerable logEntries, 
		        string machineNameQuery)
	        {
		        ArrayList entries = new ArrayList();

		        foreach (EventLogEntry logEntry in logEntries)
		        {
			        if (logEntry.MachineName == machineNameQuery)
			        {
				        entries.Add(logEntry);
			        }
		        }

		        EventLogEntry[] entriesArray = new EventLogEntry[entries.Count];
		        entries.CopyTo(entriesArray);
		        return (entriesArray);
	        }

	        public static EventLogEntry[] FindCategory(IEnumerable logEntries, 
		        string categoryQuery)
	        {
		        ArrayList entries = new ArrayList();

		        foreach (EventLogEntry logEntry in logEntries)
		        {
			        if (logEntry.Category == categoryQuery)
			        {
				        entries.Add(logEntry);
			        }
		        }

		        EventLogEntry[] entriesArray = new EventLogEntry[entries.Count];
		        entries.CopyTo(entriesArray);
		        return (entriesArray);
	        }

	        public static EventLogEntry[] FindCategory(IEnumerable logEntries, 
		        short categoryNumQuery)
	        {
		        ArrayList entries = new ArrayList();

		        foreach (EventLogEntry logEntry in logEntries)
		        {
			        if (logEntry.CategoryNumber == categoryNumQuery)
			        {
				        entries.Add(logEntry);
			        }
		        }

		        EventLogEntry[] entriesArray = new EventLogEntry[entries.Count];
		        entries.CopyTo(entriesArray);
		        return (entriesArray);
	        }

	        public static EventLogEntry[] FindSource(IEnumerable logEntries, 
		        string sourceQuery)
	        {
		        ArrayList entries = new ArrayList();

		        foreach (EventLogEntry logEntry in logEntries)
		        {
			        if (logEntry.Source == sourceQuery)
			        {
				        entries.Add(logEntry);
			        }
		        }

		        EventLogEntry[] entriesArray = new EventLogEntry[entries.Count];
		        entries.CopyTo(entriesArray);
		        return (entriesArray);
	        }

	        public static EventLogEntry[] FindEntryType(IEnumerable logEntries, 
		        EventLogEntryType entryTypeQuery)
	        {
                ArrayList entries = new ArrayList();

                foreach (EventLogEntry logEntry in logEntries)
                {
                    if (logEntry.EntryType == entryTypeQuery)
                    {
                        entries.Add(logEntry);
                    }
                }

                EventLogEntry[] entriesArray = new EventLogEntry[entries.Count];
                entries.CopyTo(entriesArray);
                return (entriesArray);
	        }

	        public static EventLogEntry[] FindMessage(IEnumerable logEntries, 
		        string messageQuery)
	        {
		        ArrayList entries = new ArrayList();

		        foreach (EventLogEntry logEntry in logEntries)
		        {
			        if (logEntry.Message == messageQuery)
			        {
				        entries.Add(logEntry);
			        }
		        }

		        EventLogEntry[] entriesArray = new EventLogEntry[entries.Count];
		        entries.CopyTo(entriesArray);
		        return (entriesArray);
	        }

	        public static EventLogEntry[] FindInstanceId(IEnumerable logEntries, 
		        int instanceIDQuery)
	        {
		        ArrayList entries = new ArrayList();

		        foreach (EventLogEntry logEntry in logEntries)
		        {
                    if (logEntry.InstanceId == instanceIDQuery)
			        {
				        entries.Add(logEntry);
			        }
		        }

		        EventLogEntry[] entriesArray = new EventLogEntry[entries.Count];
		        entries.CopyTo(entriesArray);
		        return (entriesArray);
	        }
        }
        #endregion

        #region "8.5 Watching the Event Log for a Specific Entry"
		public static void WatchForAppEvent(EventLog log)
		{
			log.EnableRaisingEvents = true;
			log.EntryWritten += new EntryWrittenEventHandler(OnEntryWritten);
			
			TestEventLogClass();
		}

		public static void OnEntryWritten(object source, EntryWrittenEventArgs entryArg) 
		{
			if (entryArg.Entry.EntryType == EventLogEntryType.Error)
			{
				// Do further actions here as necessary
				Console.WriteLine(entryArg.Entry.Category);
				Console.WriteLine(entryArg.Entry.EntryType.ToString());
				Console.WriteLine(entryArg.Entry.Message);
				Console.WriteLine("Entry written");
			}
		}
        #endregion

        #region "8.6 Implementing a Simple Performance Counter"
		public static void TestCreateSimpleCounter()
		{
			PerformanceCounter AppCounter = CreateSimpleCounter("AppCounter","", PerformanceCounterType.CounterTimer, "AppCategory", "");
			AppCounter.RawValue = 10;
			for (int i = 0; i <= 10; i++)
			{
				CounterSample CounterSampleValue = AppCounter.NextSample();
				Console.WriteLine("\r\n--> Sample RawValue  = " + CounterSampleValue.RawValue);
				
				long Value = AppCounter.IncrementBy(i * 2);
				System.Threading.Thread.Sleep(10 * i);

				CounterSample CounterNextSampleValue = AppCounter.NextSample();
				Console.WriteLine("--> NextValue RawValue = " + CounterNextSampleValue.RawValue);
				
				Console.WriteLine("Time delta = " + (CounterNextSampleValue.TimeStamp - CounterSampleValue.TimeStamp));
				Console.WriteLine("Time 100ns delta = " + (CounterNextSampleValue.TimeStamp100nSec - CounterSampleValue.TimeStamp100nSec));
				Console.WriteLine("CounterTimeStamp delta = " + (CounterNextSampleValue.CounterTimeStamp - CounterSampleValue.CounterTimeStamp));
				
				float Sample1 = CounterSample.Calculate(CounterSampleValue, CounterNextSampleValue);
				Console.WriteLine("--> Calculated Sample1 = " + Sample1);
			}
		}

		public static PerformanceCounter CreateSimpleCounter(string counterName, string counterHelp, 
			PerformanceCounterType counterType, string categoryName, string categoryHelp)
		{
			CounterCreationDataCollection counterCollection = 
				new CounterCreationDataCollection();

			// Create the custom counter object and add it to the collection of counters
			CounterCreationData counter = new CounterCreationData(counterName, counterHelp,
				counterType);
			counterCollection.Add(counter);

			// Create category
			if (PerformanceCounterCategory.Exists(categoryName))
			{
				PerformanceCounterCategory.Delete(categoryName);
			}

            PerformanceCounterCategory appCategory = 
            // Commented out as obsolete
            //PerformanceCounterCategory.Create(categoryName, categoryHelp, counterCollection);
            PerformanceCounterCategory.Create(categoryName, categoryHelp,
                PerformanceCounterCategoryType.SingleInstance, counterCollection);

			// Create the counter and initialize it
			PerformanceCounter appCounter = 
				new PerformanceCounter(categoryName, counterName, false);

			appCounter.RawValue = 0;

			return (appCounter);
		}
        #endregion

        #region "8.7 Enable/Disable Complex Tracing Code"
		public static void TestTraceFactoryClass()
		{
			Trace.Listeners.Clear();
			
			TraceFactory Factory = new TraceFactory();
			Fooz Obj = Factory.CreateObj();
			Console.WriteLine(Obj.ToString());
			Console.WriteLine(Obj.GetHashCode());
			Obj.SomeBehavior();
		}


		public class TraceFactory
		{
			public TraceFactory() {}

			public Fooz CreateObj()
			{
				Fooz Obj = null;

#if (TRACE)
#if (TRACE_INSTANTIATION)
				Obj = new BarTraceInst();
#elif (TRACE_BEHAVIOR)
                Obj = new BarTraceBehavior();
#else
                Obj = new Bar();
#endif
#else
            Obj = new Bar();
#endif

				return (Obj);
			}
		}

		public abstract class Fooz
		{
			public virtual void SomeBehavior()
			{
				//...
			}
		}

		public class Barz : Fooz
		{
			public Barz() {}

			public override void SomeBehavior()
			{
				base.SomeBehavior();
			}
		}

		public class BarTraceInst : Fooz
		{
			public BarTraceInst() 
			{
				Trace.WriteLine("BarTraceInst object instantiated");
			}

			public override void SomeBehavior()
			{
				base.SomeBehavior();
			}
		}

		public class BarTraceBehavior : Fooz
		{
			public BarTraceBehavior() {}

			public override void SomeBehavior()
			{
				Trace.WriteLine("SomeBehavior called");
				base.SomeBehavior();
			}
		}
		#endregion

		#region "8.8 Capture output from a running process"
        public static void TestCaptureOutput()
        {
            // see 12.21 for more info on redirection...
            Process application = new Process();
            // run the command shell
            application.StartInfo.FileName = @"cmd.exe";

            // get a directory listing from the current directory
            application.StartInfo.Arguments = @"/Cdir " + Environment.CurrentDirectory;
            Console.WriteLine("Running cmd.exe with arguments: {0}", application.StartInfo.Arguments);

            // redirect standard output so we can read it
            application.StartInfo.RedirectStandardOutput = true;
            application.StartInfo.UseShellExecute = false;

            // Create a log file to hold the results in the current EXE directory
            using (StreamWriter logger = new StreamWriter("cmdoutput.log"))
            {
                // start it up
                application.Start();

                // get stdout
                StreamReader output = application.StandardOutput;

                // dump the output stream while the app runs
                do
                {
                    using (output)
                    {
                        char[] info = null;
                        while (output.Peek() >= 0)
                        {
                            info = new char[4096];
                            output.Read(info, 0, info.Length);
                            // write to the logger
                            logger.Write(info, 0, info.Length);
                        }
                    }
                }
                while (!application.HasExited);
            }

            // close the process object
            application.Close();

        }
		#endregion

        #region "8.9 Create custom debugging displays for your classes"
        [DebuggerDisplay("Citizen Full Name = {_honorific}{_first}{_middle}{_last}")]
        public class Citizen
        {
            private string _honorific;
            private string _first;
            private string _middle;
            private string _last;

            public Citizen(string honorific, string first, string middle, string last)
            {
                _honorific = honorific;
                _first = first;
                _middle = middle;
                _last = last;
            }
        }
        public static void TestCustomDebuggerDisplay()
        {
            Citizen mrsJones = new Citizen("Mrs.","Alice","G.","Jones");
            Citizen mrJones = new Citizen("Mr.", "Robert", "Frederick", "Jones");
        }
		#endregion

	}
}
