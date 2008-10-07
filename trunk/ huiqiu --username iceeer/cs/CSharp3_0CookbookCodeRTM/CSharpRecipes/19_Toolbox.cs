using System;
using System.Runtime.ConstrainedExecution;
using System.Reflection;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Runtime.Remoting;
using Microsoft.Win32;
using System.Messaging;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace CSharpRecipes
{
	public static class Toolbox
	{
		#region "19.1 Preventing bad application shutdown"
        public static void PreventBadShutdown()
        {
            RegisterForSystemEvents();

            // change power mode
            // change session stuff

            UnregisterFromSystemEvents();
        }

        public static void RegisterForSystemEvents()
        {
            // always get the final notification when the event thread is shutting down 
            // so we can unregister
            SystemEvents.EventsThreadShutdown += 
                new EventHandler(OnEventsThreadShutdown);
            SystemEvents.PowerModeChanged +=
                new PowerModeChangedEventHandler(OnPowerModeChanged);
            SystemEvents.SessionSwitch +=
                new SessionSwitchEventHandler(OnSessionSwitch);
            SystemEvents.SessionEnding +=
                new SessionEndingEventHandler(OnSessionEnding);
            SystemEvents.SessionEnded +=
                new SessionEndedEventHandler(OnSessionEnded);
        }

        private static void UnregisterFromSystemEvents()
        {
            SystemEvents.EventsThreadShutdown -= 
                new EventHandler(OnEventsThreadShutdown);
            SystemEvents.PowerModeChanged -=
                new PowerModeChangedEventHandler(OnPowerModeChanged);
            SystemEvents.SessionSwitch -=
                new SessionSwitchEventHandler(OnSessionSwitch);
            SystemEvents.SessionEnding -=
                new SessionEndingEventHandler(OnSessionEnding);
            SystemEvents.SessionEnded -=
                new SessionEndedEventHandler(OnSessionEnded);
        }

        private static void OnEventsThreadShutdown(object sender, EventArgs e)
        {
            Debug.WriteLine("System event thread is shutting down, no more notifications.");

            // Unregister all our events as the notification thread is going away
            UnregisterFromSystemEvents();
        }

        private static void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            // power mode is changing
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    Debug.WriteLine("PowerMode: OS is resuming from suspended state");
                    break;
                case PowerModes.StatusChange:
                    Debug.WriteLine("PowerMode: There was a change relating to the power" + 
                        " supply (weak battery, unplug, etc..)");
                    break;
                case PowerModes.Suspend:
                    Debug.WriteLine("PowerMode: OS is about to be suspended");
                    break;
            }
        }

        private static void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            // check reason
            switch (e.Reason)
            {
                case SessionSwitchReason.ConsoleConnect:
                    Debug.WriteLine("Session connected from the console");
                    break;
                case SessionSwitchReason.ConsoleDisconnect:
                    Debug.WriteLine("Session disconnected from the console");
                    break;
                case SessionSwitchReason.RemoteConnect:
                    Debug.WriteLine("Remote session connected");
                    break;
                case SessionSwitchReason.RemoteDisconnect:
                    Debug.WriteLine("Remote session disconnected");
                    break;
                case SessionSwitchReason.SessionLock:
                    Debug.WriteLine("Session has been locked");
                    break;
                case SessionSwitchReason.SessionLogoff:
                    Debug.WriteLine("User was logged off from a session");
                    break;
                case SessionSwitchReason.SessionLogon:
                    Debug.WriteLine("User has logged on to a session");
                    break;
                case SessionSwitchReason.SessionRemoteControl:
                    Debug.WriteLine("Session changed to or from remote status");
                    break;
                case SessionSwitchReason.SessionUnlock:
                    Debug.WriteLine("Session has been unlocked");
                    break;
            }
        }

        private static void OnSessionEnding(object sender, SessionEndingEventArgs e)
        {
            // true to cancel the user request to end the session, false otherwise
            e.Cancel = false;
            // check reason
            switch(e.Reason)
            {
                case SessionEndReasons.Logoff:
                    Debug.WriteLine("Session ending as the user is logging off");
                    break;
                case SessionEndReasons.SystemShutdown:
                    Debug.WriteLine("Session ending as the OS is shutting down");
                    break;
            }
        }

        private static void OnSessionEnded(object sender, SessionEndedEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionEndReasons.Logoff:
                    Debug.WriteLine("Session ended as the user is logging off");
                    break;
                case SessionEndReasons.SystemShutdown:
                    Debug.WriteLine("Session ended as the OS is shutting down");
                    break;
            }
        }

        #endregion

        #region "19.2 At your service"
        public static void TestServiceManipulation()
        {
            ServiceController scStateService = new ServiceController("COM+ Event System");
            foreach (ServiceController sc in scStateService.DependentServices)
            {
                Console.WriteLine(scStateService.DisplayName + " is depended on by: " + sc.DisplayName);
            }
            Console.WriteLine("Service Type: " + scStateService.ServiceType.ToString());
            Console.WriteLine("Service Name: " + scStateService.ServiceName);
            Console.WriteLine("Display Name: " + scStateService.DisplayName);
            foreach (ServiceController sc in scStateService.ServicesDependedOn)
            {
                Console.WriteLine(scStateService.DisplayName + " depends on: " + sc.DisplayName);
            }
            Console.WriteLine("Status: " + scStateService.Status);
            // save original state
            ServiceControllerStatus originalState = scStateService.Status;
            TimeSpan serviceTimeout = TimeSpan.FromSeconds(60);
            // if it is stopped, start it
            if (scStateService.Status == ServiceControllerStatus.Stopped)
            {
                scStateService.Start();
                // wait up to 60 seconds for start
                scStateService.WaitForStatus(ServiceControllerStatus.Running, serviceTimeout);
            }
            Console.WriteLine("Status: " + scStateService.Status);
            // if it is paused, continue
            if (scStateService.Status == ServiceControllerStatus.Paused)
            {
                if(scStateService.CanPauseAndContinue)
                {
                    scStateService.Continue();
                    // wait up to 60 seconds for start
                    scStateService.WaitForStatus(ServiceControllerStatus.Running, serviceTimeout);
                }
            }
            Console.WriteLine("Status: " + scStateService.Status);

            // should be running at this point 

            // can we stop it?
            if (scStateService.CanStop)
            {
                scStateService.Stop();
                // wait up to 60 seconds for stop
                scStateService.WaitForStatus(ServiceControllerStatus.Stopped, serviceTimeout);
            }
            Console.WriteLine("Status: " + scStateService.Status);

            // set it back to the original state
            switch (originalState)
            {
                case ServiceControllerStatus.Stopped:
                    if (scStateService.CanStop)
                    {
                        scStateService.Stop();
                    }
                    break;
                case ServiceControllerStatus.Running:
                    scStateService.Start();
                    // wait up to 60 seconds for start
                    scStateService.WaitForStatus(ServiceControllerStatus.Running, serviceTimeout);
                    break;
                case ServiceControllerStatus.Paused:
                    // if it was paused and is stopped, need to restart so we can pause
                    if (scStateService.Status == ServiceControllerStatus.Stopped)
                    {
                        scStateService.Start();
                        // wait up to 60 seconds for start
                        scStateService.WaitForStatus(ServiceControllerStatus.Running, serviceTimeout);
                    }
                    // now pause
                    if (scStateService.CanPauseAndContinue)
                    {
                        scStateService.Pause();
                        // wait up to 60 seconds for paused
                        scStateService.WaitForStatus(ServiceControllerStatus.Paused, serviceTimeout);
                    }
                    break;
            }
            scStateService.Refresh();
            Console.WriteLine("Status: " + scStateService.Status.ToString());

            // close it
            scStateService.Close();
        }
        #endregion

        #region "19.3 List what processes an assembly is loaded in"
        public static IEnumerable<Process> GetProcessesAssemblyIsLoadedIn(string assemblyFileName)
        {
            var processes = from process in Process.GetProcesses()
                            where process.ProcessName != "System" &&
                                  process.ProcessName != "Idle"
                            from ProcessModule processModule in process.Modules
                            where processModule.ModuleName.Equals(assemblyFileName, StringComparison.OrdinalIgnoreCase)
                            select process;
            return processes;
        }
        #endregion

        #region "19.4 Using Message Queues on a local workstation"
        public static void TestMessageQueue()
        {
            // NOTE: Message Queue services must be set up for this to work
            // This can be added in Add/Remove Windows Components
            try
            {
                // this is the good syntax for workstation queues
                //MQWorker mqw = new MQWorker(@".\private$\MQWorkerQ");
                using (MQWorker mqw = new MQWorker(@".\MQWorkerQ"))
                {
                    string xml = "<MyXml><InnerXml location=\"inside\"/></MyXml>";
                    Console.WriteLine("Sending message to message queue: " + xml);
                    mqw.SendMessage("Label for message", xml);
                    string retXml = mqw.ReadMessage();
                    Console.WriteLine("Read message from message queue: " + retXml);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        #region MQWorker class
        class MQWorker : IDisposable
        {
            private bool _disposed;
            private string _mqPathName;
            MessageQueue _queue;

            public MQWorker(string queuePathName)
            {
                if (string.IsNullOrEmpty(queuePathName))
                    throw new ArgumentNullException("queuePathName");

                _mqPathName = queuePathName;

                SetUpQueue();
            }

            private void SetUpQueue()
            {
                // See if the queue exists, create it if not
                if (!MessageQueue.Exists(_mqPathName))
                {
                    try
                    {
                        _queue = MessageQueue.Create(_mqPathName);
                    }
                    catch (MessageQueueException mqex)
                    {
                        // see if we are running on a workgroup computer
                        if (mqex.MessageQueueErrorCode == MessageQueueErrorCode.UnsupportedOperation)
                        {
                            string origPath = _mqPathName;
                            // must be a private queue in workstation mode
                            int index = _mqPathName.ToLowerInvariant().
                                            IndexOf("private$",StringComparison.OrdinalIgnoreCase);
                            if (index == -1)
                            {
                                // get the first \
                                index = _mqPathName.IndexOf(@"\",StringComparison.OrdinalIgnoreCase);
                                // insert private$\ after server entry
                                _mqPathName = _mqPathName.Insert(index + 1, @"private$\");
                                // try try again
                                try
                                {
                                    if (!MessageQueue.Exists(_mqPathName))
                                        _queue = MessageQueue.Create(_mqPathName);
                                    else
                                        _queue = new MessageQueue(_mqPathName);
                                }
                                catch (Exception)
                                {
                                    // set original as inner exception
                                    throw new Exception("Failed to create message queue with " + origPath +
                                        " or " + _mqPathName, mqex);
                                }
                            }
                        }
                    }
                }
                else
                {
                    _queue = new MessageQueue(_mqPathName);
                }
            }

            public void SendMessage(string label, string body)
            {
                if (_queue != null)
                {
                    Message msg = new Message();
                    // label our message
                    msg.Label = label;
                    // override the default XML formatting with binary
                    // as it is faster (at the expense of legibility while debugging)
                    msg.Formatter = new BinaryMessageFormatter();
                    // make this message persist (causes message to be written
                    // to disk)
                    msg.Recoverable = true;
                    msg.Body = body;
                    _queue.Send(msg);
                }
            }

            public string ReadMessage()
            {
                Message msg = null;
                msg = _queue.Receive();
                msg.Formatter = new BinaryMessageFormatter();
                return (string)msg.Body;
            }


            #region IDisposable Members

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (!this._disposed)
                {
                    if (disposing)
                        _queue.Dispose();

                    _disposed = true;
                }
            }
            #endregion
        }
        #endregion

        #endregion

        #region "19.5 Finding the path to the current framework version"
        public static string GetCurrentFrameworkPath()
        {
            return System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
        }
        #endregion

        #region "19.6 Determining the versions of an assembly that are registered in the Global Assembly Cache"
        public static void PrintGacRegisteredVersions(string assemblyFileName)
        {
            Console.WriteLine("Searching for GAC Entries for {0}\r\n", assemblyFileName);
            // get the file name without the extension as that is the subdirectory
            // name in the GAC where it would be registered
            string assemblyFileNameNoExt = Path.GetFileNameWithoutExtension(assemblyFileName);

            // need to look for both the native images as well as "regular" dlls and exes
            string searchDLL = assemblyFileNameNoExt + ".dll";
            string searchEXE = assemblyFileNameNoExt + ".exe";
            string searchNIDLL = assemblyFileNameNoExt + ".ni.dll";
            string searchNIEXE = assemblyFileNameNoExt + ".ni.exe";
            
            // get the path to the GAC 
            string sysDir = Environment.GetFolderPath(Environment.SpecialFolder.System);
            string gacPath = Path.GetFullPath(sysDir + @"\..") + @"\ASSEMBLY\";

            // Query the GAC
            var files = from file in Directory.GetFiles(gacPath, "*", SearchOption.AllDirectories)
                        let fileInfo = new FileInfo(file)
                        where fileInfo.Name == searchDLL ||
                              fileInfo.Name == searchEXE ||
                              fileInfo.Name == searchNIDLL ||
                              fileInfo.Name == searchNIEXE
                        select fileInfo.FullName;

            foreach (string file in files)
            {
                // grab the version info and print
                FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(file);
                if (file.IndexOf("NativeImage",StringComparison.OrdinalIgnoreCase) != -1)
                {
                    Console.WriteLine("Found {0} in the GAC under {1} as a native image",
                        assemblyFileNameNoExt, Path.GetDirectoryName(file));
                }
                else
                {
                    Console.WriteLine("Found {0} in the GAC under {1} with version information:\r\n{2}",
                        assemblyFileNameNoExt, Path.GetDirectoryName(file), fileVersion.ToString());
                }
            }
        }
		#endregion

        #region "19.7 Capturing Output from the Standard Output Stream"
        public static void RedirectOutput()
        {
            try 
            {
                Console.WriteLine("Stealing standard output!");
                using (StreamWriter writer = new StreamWriter(@"c:\log.txt"))
                {
                    // steal stdout for our own purposes...
                    Console.SetOut(writer);

                    Console.WriteLine("Writing to the console... NOT!");

                    for (int i = 0; i < 10; i++)
                        Console.WriteLine(i);
                }
            }
            catch(IOException e) 
            {
                Debug.WriteLine(e.ToString());
                return ;            
            }

            // Recover the standard output stream so that a 
            // completion message can be displayed.
            using (StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput()))
            {
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
                Console.WriteLine("Back to standard output!");
            }
        }
		#endregion

        #region "19.8 Run code in it's own AppDomain"
        public class RunMe : MarshalByRefObject
        {
            public RunMe()
            {
                PrintCurrentAppDomainName();
            }

            public void PrintCurrentAppDomainName()
            {
                string name = AppDomain.CurrentDomain.FriendlyName;
                Console.WriteLine("Hello from {0}!", name);
            }
        }
        public static void RunCodeInNewAppDomain()
        {
            AppDomain myOwnAppDomain = AppDomain.CreateDomain("MyOwnAppDomain");
            // print out our current AppDomain name
            RunMe rm = new RunMe();
            rm.PrintCurrentAppDomainName();

            // Create our RunMe class in the new appdomain
            Type adType = typeof(RunMe);
            ObjectHandle objHdl =
                myOwnAppDomain.CreateInstance(adType.Module.Assembly.FullName, adType.FullName);
            // unwrap the reference
            RunMe adRunMe = (RunMe)objHdl.Unwrap();

            // make a call on the toolbox
            adRunMe.PrintCurrentAppDomainName();

            // now unload the appdomain
            AppDomain.Unload(myOwnAppDomain);
        }

		#endregion

		#region "19.9 Determining the Operating System and Service Pack Version of the current OS"
        public static string GetOSAndServicePack()
        {
            // Get the current OS info
            OperatingSystem os = Environment.OSVersion;
            string osText = string.Empty;
            // if version is 5, then it is Win2K, XP, or 2003
            switch (os.Version.Major)
            {
                case 5:
                    switch (os.Version.Minor)
                    {
                        case 0: osText = "Windows 2000";
                            break;
                        case 1: osText = "Windows XP";
                            break;
                        case 2: osText = "Windows Server 2003";
                            break;
                        default: osText = os.ToString();
                            break;
                    }
                    break;
                case 6:
                    switch (os.Version.Minor)
                    {
                        case 0: osText = "Windows Vista";
                            break;
                        case 1: osText = "Windows Server 2008";
                            break;
                        default: osText = os.ToString();
                            break;
                    }
                    break;
            }
            if (!string.IsNullOrEmpty(osText))
            {
                // get the text for the service pack
                string spText = os.ServicePack;
                // build the whole string
                return string.Format("{0} {1}", osText, spText);
            }
            // Unknown OS so return version
            return os.VersionString;
        }
		#endregion

    }
}
