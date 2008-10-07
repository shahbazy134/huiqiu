using System;
using System.Reflection;
using System.IO; 
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Collections;
using System.Security.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;

//Done deliberately in 7.13 to show overload of ==
#pragma warning disable 1718 //Comparison made to same variable; did you mean to compare something else? 

namespace CSharpRecipes
{
	public static class ExceptionHandling
    {
        #region "7.0 Introduction"

        public static void CallCOMMethod()
        {
            try
            {
                // Call a method on a COM object.
                //myCOMObj.Method1();
            }
            catch (System.Runtime.InteropServices.ExternalException exte)
            {
                // Handle potential COM exceptions arising from this call here.
            }
            catch (InvalidOperationException ioe)
            {
                // Handle any potential method calls to the COM object which are
                // not valid in its current state.
            }
        }

        public static void RunCommand(string connection, string command)
        {
            SqlConnection sqlConn = null;
            SqlCommand sqlComm = null;

            using(sqlConn = new SqlConnection(connection))
            {
                using(sqlComm = new SqlCommand(command, sqlConn))
                {
                    sqlConn.Open();
                    sqlComm.ExecuteNonQuery();
                }
            }
        }

        #endregion 7.0 Introduction

        #region "7.1 Indicating Where Exceptions Originate"
        public static void TestExceptionCode()
        {
            try
            {
                // *********sample code start
                try
                {
                    Console.WriteLine("In try");
                    int z2 = 9999999;
                    checked { z2 *= 999999999; }
                }
                catch (OverflowException oe)
                {
                    // Record the fact that the overflow exception occurred.
                    EventLog.WriteEntry("MyApplication", oe.Message, EventLogEntryType.Error);
                    throw;
                }
                // *********sample code end
            }
            catch (Exception ex)
            {
                // just here to help the sample code run
            }
        }


		#endregion

		#region "7.2 Assuring Exceptions are Not Lost When Using Finally Blocks"
        private static void PreventLossOfExceptionFormat()
        { 
            try
            {
                //…
            }
            catch(Exception e)
            {
                Console.WriteLine("Error message == " + e.Message);
                throw;
            }
            finally
            {
                try 	        
                { 	            
                    //… 	        
                }
                catch(Exception e) 	        
                { 	            
                    Console.WriteLine("An unexpected error occurred " + 
                        "in the finally block. Error message: " + e.Message); 	        
                }
            }
        }


        private static void ThrowReplacementException()
        {
            try
            {
                Console.WriteLine("In inner try");
                int z2 = 9999999;
                checked { z2 *= 999999999; }
            }
            catch (OverflowException ofe)
            {
                Console.WriteLine("An Overflow occurred. Error message: " +
                                   ofe.Message);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("Another type of error occurred. " +
                                  "Error message: " + e.Message);
                throw;
            }
            finally
            {
                Console.WriteLine("In inner finally");
                throw (new Exception("Oops"));
            }
        }

        public static void NotPreventLossOfException()
        {
            try
            {
                Console.WriteLine("In outer try");
                ThrowReplacementException();
            }
            catch (Exception e)
            {
                Console.WriteLine("In outer catch. Caught exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("In outer finally");
            }
        }

        private static void ThrowException()
        {
            try
            {
                Console.WriteLine("In inner try");
                int z2 = 9999999;
                checked { z2 *= 999999999; }
            }
            catch (OverflowException ofe)
            {
                Console.WriteLine("An Overflow occurred. Error message: " +
                                   ofe.Message);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine("Another type of error occurred. " +
                                  "Error message: " + e.Message);
                throw;
            }
            finally
            {
                try
                {
                    Console.WriteLine("In inner finally");
                    throw (new Exception("Oops"));
                }
                catch (Exception e)
                {
                    Console.WriteLine(@"An error occurred in the finally block. " +
                             "Error message: " + e.Message);
                }
            }
        }



        public static void PreventLossOfException()
        {
            try
            {
                Console.WriteLine("In outer try");
                ThrowException();
            }
            catch (Exception e)
            {
                Console.WriteLine("In outer catch. Caught exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("In outer finally");
            }
        }
        #endregion

        #region "7.3 Handling Exceptions Thrown from Methods Invoked Via Reflection"
        public static void ReflectionException()
        {
            Type reflectedClass = typeof(ExceptionHandling);

            try
            {
	            MethodInfo methodToInvoke = reflectedClass.GetMethod("TestInvoke");

	            if (methodToInvoke != null)
	            {
		            methodToInvoke.Invoke(null, null);
	            }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToShortDisplayString());
            }
        }

        public static void TestInvoke()
        {
            throw (new Exception("Thrown from invoked method."));
        }

        public static string ToShortDisplayString(this Exception ex)
        {
            StringBuilder displayText = new StringBuilder();
            WriteExceptionShortDetail(displayText, ex);
            foreach(Exception inner in ex.GetNestedExceptionList()) // from 7.14
            {
                displayText.AppendFormat("**** INNEREXCEPTION START ****{0}", Environment.NewLine);
                WriteExceptionShortDetail(displayText, inner);
                displayText.AppendFormat("**** INNEREXCEPTION END ****{0}{0}", Environment.NewLine);
            }
            return displayText.ToString();
        }

        public static void WriteExceptionShortDetail(StringBuilder builder, Exception ex)
        {
            builder.AppendFormat("Message: {0}{1}", ex.Message, Environment.NewLine);
            builder.AppendFormat("Type: {0}{1}", ex.GetType(), Environment.NewLine);
            builder.AppendFormat("Source: {0}{1}", ex.Source, Environment.NewLine);
            builder.AppendFormat("TargetSite: {0}{1}", ex.TargetSite, Environment.NewLine);
        }

		
        #endregion
        
		#region "7.4 Preventing Unhandled Exceptions"
		public static void LastChanceHandler(object sender, UnhandledExceptionEventArgs args) 
		{
			Exception e = (Exception) args.ExceptionObject;

			Console.WriteLine("Unhandled exception == " + e.ToString());
			if (args.IsTerminating)
			{
				Console.WriteLine("The application is terminating");
			}
			else
			{
				Console.WriteLine("The application is not terminating");
			}
		}

		public static void SetupLastChanceSeh()
		{
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(LastChanceHandler);
    
			System.Threading.Thread.Sleep(2000);
			
			// When running this under the debugger, the debugger will catch the 
			// exception leaving scope and report it even though you have set up the
			// unhandled exception handler, if you got here from running the code
			// in debug mode, continue to see it hit the handler above as the debugger
			// gets first shot at it.
			throw (new Exception("MyUnhandledException"));
		}
        #endregion

        #region "7.5 Getting Exception Information"
        public static string ToFullDisplayString(this Exception ex)
        {
            StringBuilder displayText = new StringBuilder();
            WriteExceptionDetail(displayText, ex);
            foreach (Exception inner in ex.GetNestedExceptionList()) // from 7.14
            {
                displayText.AppendFormat("**** INNEREXCEPTION START ****{0}", Environment.NewLine);
                WriteExceptionDetail(displayText, inner);
                displayText.AppendFormat("**** INNEREXCEPTION END ****{0}{0}", Environment.NewLine);
            }
            return displayText.ToString();
        }

        public static void WriteExceptionDetail(StringBuilder builder, Exception ex)
        {
            builder.AppendFormat("Message: {0}{1}", ex.Message, Environment.NewLine);
            builder.AppendFormat("Type: {0}{1}", ex.GetType(), Environment.NewLine);
            builder.AppendFormat("HelpLink: {0}{1}", ex.HelpLink, Environment.NewLine);
            builder.AppendFormat("Source: {0}{1}", ex.Source, Environment.NewLine);
            builder.AppendFormat("TargetSite: {0}{1}", ex.TargetSite, Environment.NewLine);
            builder.AppendFormat("Data:{0}", Environment.NewLine);
            foreach (DictionaryEntry de in ex.Data)
            {
                builder.AppendFormat("\t{0} : {1}{2}", 
                    de.Key, de.Value, Environment.NewLine);
            }
            builder.AppendFormat("StackTrace: {0}{1}", ex.StackTrace, Environment.NewLine);
        }

		public static void TestToFullDisplayString()
		{
			Exception innerInner = new Exception("The InnerInner Exception.");
            innerInner.Data.Add("Key1 for InnerInner", "Value1 for InnerInner");
            ArgumentException inner = new ArgumentException("The Inner Exception.", innerInner);
            inner.Data.Add("Key1 for Inner", "Value1 for Inner");
            NullReferenceException se = new NullReferenceException("A Test Message.", inner);
			se.HelpLink = "MyComponent.hlp";
			se.Source = "MyComponent";
            se.Data.Add("Key1 for Outer", "Value1 for Outer");
            se.Data.Add("Key2 for Outer", "Value2 for Outer");
            se.Data.Add("Key3 for Outer", "Value3 for Outer");

			try
			{
				throw (se);
			}
			catch(Exception e)
			{
				Console.WriteLine(e.ToFullDisplayString());
				//DisplayInnerException(e);
				//Console.WriteLine(e.GetBaseException().ToString());
	    	}
		}


		public static void DisplayInnerException(Exception exception)
		{
			Console.WriteLine(exception.GetBaseException().ToString());
		}
        #endregion
		
		#region "7.6 Getting to the Root of a Problem Quickly"
		public static void DisplayException2(Exception exception)
		{
			Console.WriteLine(exception.GetBaseException().ToString());
    	}
		#endregion
		
        #region "7.7 Creating a New Exception Type"		
		public static void TestSpecializedException()
		{
			// Generic inner exception used to test the RemoteComponentException's inner exception
			Exception inner = new Exception("The Inner Exception");
    
			// Test each ctor
			Console.WriteLine(Environment.NewLine + Environment.NewLine + "TEST EACH CTOR");
			RemoteComponentException se1 = new RemoteComponentException ();
			RemoteComponentException se2 = new RemoteComponentException ("A Test Message for se2");
			RemoteComponentException se3 = new RemoteComponentException ("A Test Message for se3", inner);
			RemoteComponentException se4 = new RemoteComponentException ("A Test Message for se4", 
				"MyServer");
			RemoteComponentException se5 = new RemoteComponentException ("A Test Message for se5", inner, 
				"MyServer");

			// Test new ServerName property
			Console.WriteLine(Environment.NewLine + "TEST NEW SERVERNAME PROPERTY");
			Console.WriteLine("se1.ServerName == " + se1.ServerName);
			Console.WriteLine("se2.ServerName == " + se2.ServerName);
			Console.WriteLine("se3.ServerName == " + se3.ServerName);
			Console.WriteLine("se4.ServerName == " + se4.ServerName);
			Console.WriteLine("se5.ServerName == " + se5.ServerName);

			// Test overridden Message property
			Console.WriteLine(Environment.NewLine + "TEST -OVERRIDDEN- MESSAGE PROPERTY");
			Console.WriteLine("se1.Message == " + se1.Message);
			Console.WriteLine("se2.Message == " + se2.Message);
			Console.WriteLine("se3.Message == " + se3.Message);
			Console.WriteLine("se4.Message == " + se4.Message);
			Console.WriteLine("se5.Message == " + se5.Message);

			// Test -overridden- ToString method
			Console.WriteLine(Environment.NewLine + "TEST -OVERRIDDEN- TOSTRING METHOD");
			Console.WriteLine("se1.ToString() == " + se1.ToString());
			Console.WriteLine("se2.ToString() == " + se2.ToString());
			Console.WriteLine("se3.ToString() == " + se3.ToString());
			Console.WriteLine("se4.ToString() == " + se4.ToString());
			Console.WriteLine("se5.ToString() == " + se5.ToString());

			// Test ToBaseString method
			Console.WriteLine(Environment.NewLine + "TEST TOBASESTRING METHOD");
			Console.WriteLine("se1.ToBaseString() == " + se1.ToBaseString());
			Console.WriteLine("se2.ToBaseString() == " + se2.ToBaseString());
			Console.WriteLine("se3.ToBaseString() == " + se3.ToBaseString());
			Console.WriteLine("se4.ToBaseString() == " + se4.ToBaseString());
			Console.WriteLine("se5.ToBaseString() == " + se5.ToBaseString());

			// Test -overridden- == operator
			Console.WriteLine(Environment.NewLine + "TEST -OVERRIDDEN- == OPERATOR");
            Console.WriteLine("se1 == se1 == " + (se1 == se1));
			Console.WriteLine("se2 == se1 == " + (se2 == se1));
			Console.WriteLine("se3 == se1 == " + (se3 == se1));
			Console.WriteLine("se4 == se1 == " + (se4 == se1));
			Console.WriteLine("se5 == se1 == " + (se5 == se1));
			Console.WriteLine("se5 == se4 == " + (se5 == se4));

			// Test -overridden- != operator
			Console.WriteLine(Environment.NewLine + "TEST -OVERRIDDEN- != OPERATOR");
			Console.WriteLine("se1 != se1 == " + (se1 != se1));
			Console.WriteLine("se2 != se1 == " + (se2 != se1));
			Console.WriteLine("se3 != se1 == " + (se3 != se1));
			Console.WriteLine("se4 != se1 == " + (se4 != se1));
			Console.WriteLine("se5 != se1 == " + (se5 != se1));
			Console.WriteLine("se5 != se4 == " + (se5 != se4));

			// Test -overridden- GetBaseException method
			Console.WriteLine(Environment.NewLine + "TEST -OVERRIDDEN- GETBASEEXCEPTION METHOD");
			Console.WriteLine("se1.GetBaseException() == " + se1.GetBaseException());
			Console.WriteLine("se2.GetBaseException() == " + se2.GetBaseException());
			Console.WriteLine("se3.GetBaseException() == " + se3.GetBaseException());
			Console.WriteLine("se4.GetBaseException() == " + se4.GetBaseException());
			Console.WriteLine("se5.GetBaseException() == " + se5.GetBaseException());

			// Test -overridden- GetHashCode method
			Console.WriteLine(Environment.NewLine + "TEST -OVERRIDDEN- GETHASHCODE METHOD");
			Console.WriteLine("se1.GetHashCode() == " + se1.GetHashCode());
			Console.WriteLine("se2.GetHashCode() == " + se2.GetHashCode());
			Console.WriteLine("se3.GetHashCode() == " + se3.GetHashCode());
			Console.WriteLine("se4.GetHashCode() == " + se4.GetHashCode());
			Console.WriteLine("se5.GetHashCode() == " + se5.GetHashCode());

			// Test serialization
			Console.WriteLine(Environment.NewLine + "TEST SERIALIZATION/DESERIALIZATION");
			BinaryFormatter binaryWrite = new BinaryFormatter();
			Stream ObjectFile = File.Create("se1.object");
			binaryWrite.Serialize(ObjectFile, se1);
			ObjectFile.Close();
			ObjectFile = File.Create("se2.object");
			binaryWrite.Serialize(ObjectFile, se2);
			ObjectFile.Close();
			ObjectFile = File.Create("se3.object");
			binaryWrite.Serialize(ObjectFile, se3);
			ObjectFile.Close();
			ObjectFile = File.Create("se4.object");
			binaryWrite.Serialize(ObjectFile, se4);
			ObjectFile.Close();
			ObjectFile = File.Create("se5.object");
			binaryWrite.Serialize(ObjectFile, se5);
			ObjectFile.Close();

			BinaryFormatter binaryRead = new BinaryFormatter();
			ObjectFile = File.OpenRead("se1.object");
			object Data = binaryRead.Deserialize(ObjectFile);
			Console.WriteLine("----------" + Environment.NewLine + Data);
			ObjectFile.Close();
			ObjectFile = File.OpenRead("se2.object");
			Data = binaryRead.Deserialize(ObjectFile);
			Console.WriteLine("----------" + Environment.NewLine + Data);
			ObjectFile.Close();
			ObjectFile = File.OpenRead("se3.object");
			Data = binaryRead.Deserialize(ObjectFile);    
			Console.WriteLine("----------" + Environment.NewLine + Data);
			ObjectFile.Close();    
			ObjectFile = File.OpenRead("se4.object");
			Data = binaryRead.Deserialize(ObjectFile);
			Console.WriteLine("----------" + Environment.NewLine + Data);
			ObjectFile.Close();
			ObjectFile = File.OpenRead("se5.object");
			Data = binaryRead.Deserialize(ObjectFile);
			Console.WriteLine("----------" + Environment.NewLine + Data + Environment.NewLine 
				+ "----------");
			ObjectFile.Close();

			Console.WriteLine(Environment.NewLine + "END TEST" + Environment.NewLine);
		}


        [Serializable]
        public class RemoteComponentException : Exception, ISerializable
        {
            #region Constructors
            // Normal exception ctor's
	        public RemoteComponentException() : base()
	        {
	        }

	        public RemoteComponentException(string message) : base(message)
	        {
	        }

	        public RemoteComponentException(string message, Exception innerException) 
		        : base(message, innerException)
	        {
	        }

	        // Exception ctor's that accept the new ServerName parameter
	        public RemoteComponentException(string message, string serverName) : base(message)
	        {
		        this.ServerName = serverName;
	        }

	        public RemoteComponentException(string message, Exception innerException, string serverName) 
		        : base(message, innerException)
	        {
		        this.ServerName = serverName;
            }

            // Serialization ctor
            protected RemoteComponentException(SerializationInfo exceptionInfo,
                StreamingContext exceptionContext)
                : base(exceptionInfo, exceptionContext)
            {
                this.ServerName = exceptionInfo.GetString("ServerName");
            }
            #endregion // Constructors

            #region Properties
            // Read-only property for server name
            public string ServerName { get; private set; }
            
            public override string Message
            {
                get
                {
                    if (string.IsNullOrEmpty(this.ServerName))
                        return (string.Format(Thread.CurrentThread.CurrentCulture,
                            "{0}{1}A server with an unknown name has encountered an error.",
                            base.Message, Environment.NewLine));
                    else
                        return (
                            string.Format(Thread.CurrentThread.CurrentCulture,
                                "{0}{1}The server ({2}) has encountered an error.",
                                base.Message, Environment.NewLine, this.ServerName));
                }
            }
            #endregion // Properties

            #region Overridden methods
            // ToString method
	        public override string ToString()
	        {
                string errorString =
                    string.Format(Thread.CurrentThread.CurrentCulture,
                    "An error has occured in a server component of this client.{0}Server Name: {1}{0}{2}",
                    Environment.NewLine, this.ServerName, this.ToFullDisplayString());
		        return errorString;
            }

            // Used during serialization to capture information about extra fields
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
            public override void GetObjectData(SerializationInfo info,
                StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue("ServerName", this.ServerName);
            }
            #endregion // Overridden methods

            // Call base.ToString method
	        public string ToBaseString() 
	        {
		        return (base.ToString());
	        }
        }
        #endregion

        #region "7.8 Obtaining a Stack Trace"		
		public static string GetStackTraceInfo(string currentStackTrace)
		{
			string firstStackTraceCall = "System.Environment.get_StackTrace()";
			int posOfStackTraceCall = 
                currentStackTrace.IndexOf(firstStackTraceCall,StringComparison.OrdinalIgnoreCase);
			return (currentStackTrace.Substring(posOfStackTraceCall + firstStackTraceCall.Length));
		}
		
		public static int GetStackTraceDepth(string currentStackTrace)
		{
			string firstStackTraceCall = "System.Environment.get_StackTrace()";
            int posOfStackTraceCall = 
                currentStackTrace.IndexOf(firstStackTraceCall, StringComparison.OrdinalIgnoreCase);
			string finalStackTrace = currentStackTrace.Substring(posOfStackTraceCall + firstStackTraceCall.Length);
			
			MatchCollection methodCallMatches = Regex.Matches(finalStackTrace, @"\sat\s.*(\sin\s.*\:line\s\d*)?");
			return (methodCallMatches.Count);
		}
        #endregion
 
		#region "7.9 Breaking on a First Chance Exception"
		// See recipe 7.9 in book for explanation.
		#endregion

        #region "7.10 Handling Exceptions Thrown from an Asynchronous Delegate"
		public static void TestPollingAsyncDelegate()
		{
			AsyncInvoke MI = new AsyncInvoke(TestAsyncInvoke.Method1);
			IAsyncResult AR = MI.BeginInvoke(null, null);
			
			while (!AR.IsCompleted)
			{
				System.Threading.Thread.Sleep(100);
				Console.Write('.');
			}
			Console.WriteLine("\r\n\r\nDONE Polling...");

            try
            {
                int RetVal = MI.EndInvoke(AR);
                Console.WriteLine("RetVal (Polling): " + RetVal);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
		}
		
		
		public delegate int AsyncInvoke();
		public class TestAsyncInvoke
		{
			public static int Method1()
			{
				//throw (new Exception("Method1"));
							Console.WriteLine("Invoked Method1");
							return (1);
			}
		}
		#endregion

		#region "7.11 Give your exceptions the extra info they need (Exception.Data)"
        public static void TestExceptionData()
        {
            try
            {
                try
                {
                    try
                    {
                        try
                        {
                            ArgumentException irritable =
                                new ArgumentException("I'm irritable!");
                            irritable.Data["Cause"]="Computer crashed";
                            irritable.Data["Length"]=10;
                            throw irritable;
                        }
                        catch (Exception e)
                        {
                            // see if I can help...
                            if(e.Data.Contains("Cause"))
                                e.Data["Cause"]="Fixed computer";
                            throw;
                        }
                    }
                    catch (Exception e)
                    {
                        e.Data["Comment"]="Always grumpy you are";
                        throw;
                    }
                }
                catch (Exception e)
                {
                    e.Data["Reassurance"]="Error Handled";
                    throw;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception supporting data:");
                foreach(DictionaryEntry de in e.Data)
                {
                    Console.WriteLine("\t{0} : {1}",de.Key,de.Value);
                }
            }
        }
		#endregion

		#region "7.12 Dealing with Unhandled Exceptions in a WinForms Application"
        // see the UnhandledThreadExceptions project
		#endregion

        #region "7.13 Dealing with Unhandled Exceptions in a Windows Presentation Foundation (WPF) Application"
        // see the UnhandledWPFExceptions project
        #endregion

        #region "7.14 Analyzing exceptions for common errors"
        public static void TestGetNestedSetOfExceptions()
        {
            // simulate an exception chain unwinding
            // hit a bad reference
            NullReferenceException nrex = new NullReferenceException("Touched a bad object");
            nrex.Data.Add("Bad ObjectID", "0x34874573");
            // have an error formatting a message for it
            FormatException fex = new FormatException("Resulted from Null Reference", nrex);
            // mask it as part of a data layer
            ApplicationException appex = new ApplicationException("There was an error in the data layer.",fex);
            // hit another error reformatting the exception information
            FormatException fmtEx = new FormatException("Error formatting error message.", appex);

            // Use LINQ to look for common or well-known problems in the app
            var query = from ex in fmtEx.GetNestedExceptionList()
                        where ex is NullReferenceException ||
                              ex is OutOfMemoryException ||
                              ex is ThreadAbortException
                        select ex;
            // report if any of the common or well known errors were found
            foreach (Exception ex in query)
            {
                Console.WriteLine("Found common exception (" + ex.GetType() +
                    ") with message: " + ex.Message);
            }
        }

        public static IEnumerable<Exception> GetNestedExceptionList(this Exception exception)
        {
            Exception current = exception;
            do
            {
                current = current.InnerException;
                if (current != null)
                    yield return current;
            }
            while (current != null);
        }
        #endregion
    }
}
