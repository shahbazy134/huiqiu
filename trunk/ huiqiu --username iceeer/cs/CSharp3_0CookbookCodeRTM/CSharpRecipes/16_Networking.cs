using System;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.IO;
using System.Security;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net.Mime;
using System.Net.Mail;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.ComponentModel;
using System.Reflection;

namespace CSharpRecipes
{
    public class Networking
    {
        #region "16.1 Writing a TCP Server"
        public static void WritingTCPServer()
        {
            // See the ConsoleTCPServer project
        }

        #endregion

        #region "16.2 Writing a TCP Client"
        public static void WritingTCPClient()
        {
            // See the ConsoleTCPClient project
        }
        #endregion

        #region "16.3 Simulating Form Execution"
        public static void SimulatingFormExecution()
        {
            // In order to use this, you need to run the CSCBWeb project first.
            Uri uri = new Uri("http://localhost:7472/CSCBWeb/WebForm1.aspx");
            WebClient client = new WebClient();

            // Create a series of name/value pairs to send
            // Add necessary parameter/value pairs to the name/value container.
            NameValueCollection collection = new NameValueCollection() 
                { {"Item", "WebParts"}, 
                  {"Identity", "foo@bar.com"}, 
                  {"Quantity", "5"} };

            Console.WriteLine("Uploading name/value pairs to URI {0} ...",
                uri.AbsoluteUri);                        

            // Upload the NameValueCollection.
            byte[] responseArray = 
                client.UploadValues(uri.AbsoluteUri,"POST",collection);
            // Decode and display the response.
            Console.WriteLine("\nResponse received was {0}",
                Encoding.ASCII.GetString(responseArray));

        }
        #endregion

        #region "16.4 Downloading Data from a Server"	
        public static void DownloadingDataFromServer()
        {
            Uri uri = new Uri("http://localhost:4088/CSCBWeb/DownloadData16_4.aspx");

            // make a client
            using (WebClient client = new WebClient())
            {
                // get the contents of the file
                Console.WriteLine("Downloading {0} ",uri.AbsoluteUri);
                // download the page and store the bytes
                byte[] bytes;
                try
                {
                    bytes = client.DownloadData(uri);
                }
                catch (WebException we)
                {
                    Console.WriteLine(we.ToString());
                    return;
                }
                // Write the HTML out
                string page = Encoding.ASCII.GetString(bytes);
                Console.WriteLine(page);

                // go get the file
                Console.WriteLine("Retrieving file from {0}...\r\n", uri);
                // get file and put it in a temp file
                string tempFile = Path.GetTempFileName();
                try
                {
                    client.DownloadFile(uri, tempFile);
                }
                catch (WebException we)
                {
                    Console.WriteLine(we.ToString());
                    return;
                }
                Console.WriteLine("Downloaded {0} to {1}", uri, tempFile);
            }
        }

        public static void UploadingDataToServer()
        {
            Uri uri = new Uri("http://localhost:4088/CSCBWeb/UploadData16_4.aspx");
            // make a client
            using (WebClient client = new WebClient())
            {
                Console.WriteLine("Uploading to {0} ",uri.AbsoluteUri);
                try
                {
                    client.UploadFile(uri, "SampleClassLibrary.dll");
                    Console.WriteLine("Uploaded successfully to {0} ", 
                                        uri.AbsoluteUri);
                }
                catch (WebException we)
                {
                    Console.WriteLine(we.ToString());
                }
            }
        }
        #endregion

        #region "16.5 Using Named Pipes to Communicate"
        // See the NamedPipeClientConsole and NamedPipeServerConsole projects
        #endregion

        #region  "16.6 Pinging programmatically"
        public static void TestPing()
        {
            System.Net.NetworkInformation.Ping pinger =
                new System.Net.NetworkInformation.Ping();
            PingReply reply = pinger.Send("www.oreilly.com");
            DisplayPingReplyInfo(reply);

            pinger.PingCompleted += pinger_PingCompleted;
            pinger.SendAsync("www.oreilly.com", "oreilly ping");
        }

        private static void DisplayPingReplyInfo(PingReply reply)
        {
            Console.WriteLine("Results from pinging " + reply.Address);
            Console.WriteLine("\tFragmentation allowed?: {0}", !reply.Options.DontFragment);
            Console.WriteLine("\tTime to live: {0}", reply.Options.Ttl);
            Console.WriteLine("\tRoundtrip took: {0}", reply.RoundtripTime);
            Console.WriteLine("\tStatus: {0}", reply.Status.ToString());
        }

        private static void pinger_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            PingReply reply = e.Reply;
            DisplayPingReplyInfo(reply);

            if(e.Cancelled)
            {
                Console.WriteLine("Ping for " + e.UserState.ToString() + " was cancelled");
            }
            else if (e.Error != null)
            {
                Console.WriteLine("Exception thrown during ping: {0}", e.Error.ToString());
            }
        }

		#endregion

        #region  "16.7 Send SMTP mail using the SMTP service"
        public static void TestSendMail()
        {
            try
            {
                // send a message with attachments
                string from = "hilyard@comcast.net";
                string to = "hilyard@comcast.net";
                MailMessage attachmentMessage = new MailMessage(from, to);
                attachmentMessage.Subject = "Hi there!";
                attachmentMessage.Body = "Check out this cool code!";
                // many systems filter out HTML mail that is relayed
                attachmentMessage.IsBodyHtml = false;
                // set up the attachment
                string pathToCode = @"..\..\16_Networking.cs";
                Attachment attachment =
                    new Attachment(pathToCode,
                        MediaTypeNames.Application.Octet);
                attachmentMessage.Attachments.Add(attachment);

                // bounce this off the local SMTP service.  The local SMTP service needs to 
                // have relaying set up to go through a real email server...
                // This could also set up to go against an SMTP server available to 
                // you on the network.
                SmtpClient client = new SmtpClient("localhost");
                client.Send(attachmentMessage);
                // or just send text
                MailMessage textMessage = new MailMessage("hilyard@comcast.net",
                                    "hilyard@comcast.net",
                                    "Me again",
                                    "You need therapy, talking to yourself is one thing but writing code to send email is a whole other thing...");
                client.Send(textMessage);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                Console.WriteLine(e);
            }
        }
		#endregion

        #region  "16.8 Use sockets to scan the ports on a machine"
        public static void TestSockets()
        {
            // do a specific range
            Console.WriteLine("Checking ports 1-30 on localhost...");
            CheapoPortScanner cps = new CheapoPortScanner("127.0.0.1",1,30);
            cps.OpenPortFound += new CheapoPortScanner.OpenPortFoundEventHandler(cps_OpenPortFound);
            cps.Scan();
            Console.WriteLine("Found {0} ports open and {1} ports closed",
                    cps.OpenPorts.Count, cps.ClosedPorts.Count);

            // do the local machine, whole port range 1-65535
            //cps = new CheapoPortScanner();
            //cps.Scan();
            //cps.ReportToConsole();
        }

        static void cps_OpenPortFound(object sender, CheapoPortScanner.OpenPortEventArgs args)
        {
            Console.WriteLine("OpenPortFound reported port {0} was open",args.PortNum);
        }

        class CheapoPortScanner
        {
            #region Private consts and members
            const int PORT_MIN_VALUE = 1;
            const int PORT_MAX_VALUE = 65535;

            private int _minPort = PORT_MIN_VALUE;
            private int _maxPort = PORT_MAX_VALUE;
            private List<int> _openPorts;
            private List<int> _closedPorts;
            private string _host = "127.0.0.1"; // localhost
            #endregion

            #region Event 
            public class OpenPortEventArgs : EventArgs
            {
                int _portNum;
                public OpenPortEventArgs(int portNum) : base()
                {
                    _portNum = portNum;
                }

                public int PortNum
                {
                    get { return _portNum; }
                }
            }

            public delegate void OpenPortFoundEventHandler(object sender, OpenPortEventArgs args);
            public event OpenPortFoundEventHandler OpenPortFound;
            #endregion // Event

            #region CTORs & Init code
            public CheapoPortScanner()
            {
                // defaults are already set for ports & localhost
                SetupLists();
            }

            public CheapoPortScanner(string host, int minPort, int maxPort)
            {
                if (minPort > maxPort)
                    throw new ArgumentException("Min port cannot be greater than max port");
                if (minPort < PORT_MIN_VALUE || minPort > PORT_MAX_VALUE)
                    throw new ArgumentOutOfRangeException("Min port cannot be less than " +
                                        PORT_MIN_VALUE + " or greater than " + PORT_MAX_VALUE);
                if (maxPort < PORT_MIN_VALUE || maxPort > PORT_MAX_VALUE)
                    throw new ArgumentOutOfRangeException("Max port cannot be less than " +
                                        PORT_MIN_VALUE + " or greater than " + PORT_MAX_VALUE);

                _host = host;
                _minPort = minPort;
                _maxPort = maxPort;
                SetupLists();
            }

            private void SetupLists()
            {
                // set up lists with capacity to hold half of range
                // since we can't know how many ports are going to be open
                // so we compromise and allocate enough for half

                // rangeCount is max - min + 1
                int rangeCount = (_maxPort - _minPort) + 1;
                // if there are an odd number, bump by one to get one extra slot
                if (rangeCount % 2 != 0)
                    rangeCount += 1;
                // reserve half the ports in the range for each
                _openPorts = new List<int>(rangeCount / 2);
                _closedPorts = new List<int>(rangeCount / 2);
            }
            #endregion // CTORs & Init code

            #region Properties
            public ReadOnlyCollection<int> OpenPorts
            {
                get { return new ReadOnlyCollection<int>(_openPorts); } 
            }

            public ReadOnlyCollection<int> ClosedPorts
            {
                get { return new ReadOnlyCollection<int>(_closedPorts); }
            }
            #endregion // Properties

            #region Private Methods
            private void CheckPort(int port)
            {
                if (IsPortOpen(port))
                {
                    // if we got here it is open
                    _openPorts.Add(port);

                    // notify anyone paying attention
                    OpenPortFoundEventHandler openPortFound = OpenPortFound;
                    if (openPortFound != null)
                        openPortFound(this, new OpenPortEventArgs(port));
                }
                else
                {
                    // server doesn't have that port open
                    _closedPorts.Add(port);
                }
            }

            private bool IsPortOpen(int port)
            {
                Socket sock = null;
                try
                {
                    // make a TCP based socket
                    sock = new Socket(AddressFamily.InterNetwork,
                                    SocketType.Stream,
                                    ProtocolType.Tcp);
                    // connect 
                    sock.Connect(_host, port);
                    return true;

                }
                catch (SocketException se)
                {
                    if (se.SocketErrorCode == SocketError.ConnectionRefused)
                    {
                        return false;
                    }
                    else
                    {
                        //An error occurred when attempting to access the socket. 
                        Debug.WriteLine(se.ToString());
                        Console.WriteLine(se.ToString());
                    }
                }
                finally
                {
                    if (sock != null)
                    {
                        if (sock.Connected)
                            sock.Disconnect(false);
                        sock.Close();
                    }
                }
                return false;
            }
            #endregion

            #region Public Methods
            public void Scan()
            {
                for (int port = _minPort; port <= _maxPort; port++)
                {
                    CheckPort(port);
                }
            }

            public void ReportToConsole()
            {
                Console.WriteLine("Port Scan for host at {0}:", _host.ToString());
                Console.WriteLine("\tStarting Port: {0}; Ending Port: {1}", _minPort, _maxPort);
                Console.WriteLine("\tOpen ports:");
                foreach (int port in _openPorts)
                {
                    Console.WriteLine("\t\tPort {0}", port);
                }
                Console.WriteLine("\tClosed ports:");
                foreach (int port in _closedPorts)
                {
                    Console.WriteLine("\t\tPort {0}", port);
                }
            }

            #endregion // Public Methods
        }

		#endregion

        #region "16.9 Read internet configuration settings"
        public static void GetInternetSettings()
        {
            try
            {
                InternetSettingsReader isr = new InternetSettingsReader();
                Console.WriteLine("Current Proxy Address: {0}", isr.ProxyAddress);
                Console.WriteLine("Current Proxy Port: {0}", isr.ProxyPort);
                Console.WriteLine("Current ByPass Local Address setting: {0}",
                                    isr.BypassLocalAddresses);
                Console.WriteLine("Exception addresses for proxy (bypass):");
                if (isr.ProxyExceptions != null)
                {
                    foreach (string addr in isr.ProxyExceptions)
                    {
                        Console.WriteLine("\t{0}", addr);
                    }
                }
                Console.WriteLine("Proxy connection type: {0}", isr.ConnectionType.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region WinInet structures
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct InternetPerConnOptionList
        {
            public int dwSize;					// size of the INTERNET_PER_CONN_OPTION_LIST struct
            public IntPtr szConnection;			// connection name to set/query options
            public int dwOptionCount;			// number of options to set/query
            public int dwOptionError;				// on error, which option failed
            public IntPtr options;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct InternetConnectionOption
        {
            static readonly int Size;
            public PerConnOption m_Option;
            public InternetConnectionOptionValue m_Value;
            static InternetConnectionOption()
            {
                InternetConnectionOption.Size = Marshal.SizeOf(typeof(InternetConnectionOption));
            }

            // Nested Types
            [StructLayout(LayoutKind.Explicit)]
            public struct InternetConnectionOptionValue
            {
                // Fields
                [FieldOffset(0)]
                public System.Runtime.InteropServices.ComTypes.FILETIME m_FileTime;
                [FieldOffset(0)]
                public int m_Int;
                [FieldOffset(0)]
                public IntPtr m_StringPtr;
            }
        }
        #endregion

        #region WinInet enums
        //
        // options manifests for Internet{Query|Set}Option
        //
        //public enum InternetOption
        //{
        //    INTERNET_OPTION_CALLBACK = 1,
        //    INTERNET_OPTION_CONNECT_TIMEOUT = 2,
        //    INTERNET_OPTION_CONNECT_RETRIES = 3,
        //    INTERNET_OPTION_CONNECT_BACKOFF = 4,
        //    INTERNET_OPTION_SEND_TIMEOUT = 5,
        //    INTERNET_OPTION_CONTROL_SEND_TIMEOUT = INTERNET_OPTION_SEND_TIMEOUT,
        //    INTERNET_OPTION_RECEIVE_TIMEOUT = 6,
        //    INTERNET_OPTION_CONTROL_RECEIVE_TIMEOUT = INTERNET_OPTION_RECEIVE_TIMEOUT,
        //    INTERNET_OPTION_DATA_SEND_TIMEOUT = 7,
        //    INTERNET_OPTION_DATA_RECEIVE_TIMEOUT = 8,
        //    INTERNET_OPTION_HANDLE_TYPE = 9,
        //    INTERNET_OPTION_LISTEN_TIMEOUT = 11,
        //    INTERNET_OPTION_READ_BUFFER_SIZE = 12,
        //    INTERNET_OPTION_WRITE_BUFFER_SIZE = 13,

        //    INTERNET_OPTION_ASYNC_ID = 15,
        //    INTERNET_OPTION_ASYNC_PRIORITY = 16,

        //    INTERNET_OPTION_PARENT_HANDLE = 21,
        //    INTERNET_OPTION_KEEP_CONNECTION = 22,
        //    INTERNET_OPTION_REQUEST_FLAGS = 23,
        //    INTERNET_OPTION_EXTENDED_ERROR = 24,

        //    INTERNET_OPTION_OFFLINE_MODE = 26,
        //    INTERNET_OPTION_CACHE_STREAM_HANDLE = 27,
        //    INTERNET_OPTION_USERNAME = 28,
        //    INTERNET_OPTION_PASSWORD = 29,
        //    INTERNET_OPTION_ASYNC = 30,
        //    INTERNET_OPTION_SECURITY_FLAGS = 31,
        //    INTERNET_OPTION_SECURITY_CERTIFICATE_STRUCT = 32,
        //    INTERNET_OPTION_DATAFILE_NAME = 33,
        //    INTERNET_OPTION_URL = 34,
        //    INTERNET_OPTION_SECURITY_CERTIFICATE = 35,
        //    INTERNET_OPTION_SECURITY_KEY_BITNESS = 36,
        //    INTERNET_OPTION_REFRESH = 37,
        //    INTERNET_OPTION_PROXY = 38,
        //    INTERNET_OPTION_SETTINGS_CHANGED = 39,
        //    INTERNET_OPTION_VERSION = 40,
        //    INTERNET_OPTION_USER_AGENT = 41,
        //    INTERNET_OPTION_END_BROWSER_SESSION = 42,
        //    INTERNET_OPTION_PROXY_USERNAME = 43,
        //    INTERNET_OPTION_PROXY_PASSWORD = 44,
        //    INTERNET_OPTION_CONTEXT_VALUE = 45,
        //    INTERNET_OPTION_CONNECT_LIMIT = 46,
        //    INTERNET_OPTION_SECURITY_SELECT_CLIENT_CERT = 47,
        //    INTERNET_OPTION_POLICY = 48,
        //    INTERNET_OPTION_DISCONNECTED_TIMEOUT = 49,
        //    INTERNET_OPTION_CONNECTED_STATE = 50,
        //    INTERNET_OPTION_IDLE_STATE = 51,
        //    INTERNET_OPTION_OFFLINE_SEMANTICS = 52,
        //    INTERNET_OPTION_SECONDARY_CACHE_KEY = 53,
        //    INTERNET_OPTION_CALLBACK_FILTER = 54,
        //    INTERNET_OPTION_CONNECT_TIME = 55,
        //    INTERNET_OPTION_SEND_THROUGHPUT = 56,
        //    INTERNET_OPTION_RECEIVE_THROUGHPUT = 57,
        //    INTERNET_OPTION_REQUEST_PRIORITY = 58,
        //    INTERNET_OPTION_HTTP_VERSION = 59,
        //    INTERNET_OPTION_RESET_URLCACHE_SESSION = 60,
        //    INTERNET_OPTION_ERROR_MASK = 62,
        //    INTERNET_OPTION_FROM_CACHE_TIMEOUT = 63,
        //    INTERNET_OPTION_BYPASS_EDITED_ENTRY = 64,
        //    INTERNET_OPTION_DIAGNOSTIC_SOCKET_INFO = 67,
        //    INTERNET_OPTION_CODEPAGE = 68,
        //    INTERNET_OPTION_CACHE_TIMESTAMPS = 69,
        //    INTERNET_OPTION_DISABLE_AUTODIAL = 70,
        //    INTERNET_OPTION_MAX_CONNS_PER_SERVER = 73,
        //    INTERNET_OPTION_MAX_CONNS_PER_1_0_SERVER = 74,
        //    INTERNET_OPTION_PER_CONNECTION_OPTION = 75
        //    INTERNET_OPTION_DIGEST_AUTH_UNLOAD = 76,
        //    INTERNET_OPTION_IGNORE_OFFLINE = 77,
        //    INTERNET_OPTION_IDENTITY = 78,
        //    INTERNET_OPTION_REMOVE_IDENTITY = 79,
        //    INTERNET_OPTION_ALTER_IDENTITY = 80,
        //    INTERNET_OPTION_SUPPRESS_BEHAVIOR = 81,
        //    INTERNET_OPTION_AUTODIAL_MODE = 82,
        //    INTERNET_OPTION_AUTODIAL_CONNECTION = 83,
        //    INTERNET_OPTION_CLIENT_CERT_CONTEXT = 84,
        //    INTERNET_OPTION_AUTH_FLAGS = 85,
        //    INTERNET_OPTION_COOKIES_3RD_PARTY = 86,
        //    INTERNET_OPTION_DISABLE_PASSPORT_AUTH = 87,
        //    INTERNET_OPTION_SEND_UTF8_SERVERNAME_TO_PROXY = 88,
        //    INTERNET_OPTION_EXEMPT_CONNECTION_LIMIT = 89,
        //    INTERNET_OPTION_ENABLE_PASSPORT_AUTH = 90,

        //    INTERNET_OPTION_HIBERNATE_INACTIVE_WORKER_THREADS = 91,
        //    INTERNET_OPTION_ACTIVATE_WORKER_THREADS = 92,
        //    INTERNET_OPTION_RESTORE_WORKER_THREAD_DEFAULTS = 93,
        //    INTERNET_OPTION_SOCKET_SEND_BUFFER_LENGTH = 94,
        //    INTERNET_OPTION_PROXY_SETTINGS_CHANGED = 95,

        //    INTERNET_OPTION_DATAFILE_EXT = 96,

        //    INTERNET_FIRST_OPTION = INTERNET_OPTION_CALLBACK,
        //    INTERNET_LAST_OPTION = INTERNET_OPTION_DATAFILE_EXT
        //}

        //
        // Options used in INTERNET_PER_CONN_OPTON struct
        //
        public enum PerConnOption
        {
            INTERNET_PER_CONN_FLAGS = 1, // Sets or retrieves the connection type. The Value member will contain one or more of the values from PerConnFlags 
            INTERNET_PER_CONN_PROXY_SERVER = 2, // Sets or retrieves a string containing the proxy servers.  
            INTERNET_PER_CONN_PROXY_BYPASS = 3, // Sets or retrieves a string containing the URLs that do not use the proxy server.  
            INTERNET_PER_CONN_AUTOCONFIG_URL = 4//, // Sets or retrieves a string containing the URL to the automatic configuration script.  
            //INTERNET_PER_CONN_AUTODISCOVERY_FLAGS = 5, // Sets or retrieves the automatic discovery settings. The Value member will contain one or more of the values from PerConnAutoDiscoveryFlags 
            //INTERNET_PER_CONN_AUTOCONFIG_SECONDARY_URL = 6, // Chained autoconfig URL. Used when the primary autoconfig URL points to an INS file that sets a second autoconfig URL for proxy information.  
            //INTERNET_PER_CONN_AUTOCONFIG_RELOAD_DELAY_MINS = 7, // Minutes until automatic refresh of autoconfig URL by autodiscovery.  
            //INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_TIME = 8, // Read only option. Returns the time the last known good autoconfig URL was found using autodiscovery.  
            //INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_URL = 9  // Read only option. Returns the last known good URL found using autodiscovery.  
        }

        //
        // PER_CONN_FLAGS
        //
        [Flags]
        public enum PerConnFlags
        {
            PROXY_TYPE_DIRECT = 0x00000001,  // direct to net
            PROXY_TYPE_PROXY = 0x00000002,  // via named proxy
            PROXY_TYPE_AUTO_PROXY_URL = 0x00000004,  // autoproxy URL
            PROXY_TYPE_AUTO_DETECT = 0x00000008   // use autoproxy detection
        }

        ////
        //// PER_CONN_AUTODISCOVERY_FLAGS
        ////
        //[Flags]
        //public enum PerConnAutoDiscoveryFlags
        //{
        //    AUTO_PROXY_FLAG_USER_SET = 0x00000001,   // user changed this setting
        //    AUTO_PROXY_FLAG_ALWAYS_DETECT = 0x00000002,   // force detection even when its not needed
        //    AUTO_PROXY_FLAG_DETECTION_RUN = 0x00000004,   // detection has been run
        //    AUTO_PROXY_FLAG_MIGRATED = 0x00000008,   // migration has just been done
        //    AUTO_PROXY_FLAG_DONT_CACHE_PROXY_RESULT = 0x00000010,   // don't cache result of host=proxy name
        //    AUTO_PROXY_FLAG_CACHE_INIT_RUN = 0x00000020,   // don't initalize and run unless URL expired
        //    AUTO_PROXY_FLAG_DETECTION_SUSPECT = 0x00000040   // if we're on a LAN & Modem, with only one IP, bad?!?
        //}
        #endregion

        private static class NativeMethods
        {
            #region P/Invoke defs
            [DllImport("WinInet.dll", EntryPoint = "InternetQueryOptionW", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool InternetQueryOption(
                IntPtr hInternet,
                int dwOption,
                ref InternetPerConnOptionList optionsList,
                ref int bufferLength
                );
            #endregion
        }

        public class InternetSettingsReader
        {
            #region Private Members
            string _proxyAddr;
            int _proxyPort = -1;
            bool _bypassLocal;
            string _autoConfigAddr;
            List<string> _proxyExceptions;
            PerConnFlags _flags;
            #endregion

            #region CTOR
            public InternetSettingsReader()
            {
            }
            #endregion

            #region Properties
            public string ProxyAddress
            {
                get
                {
                    InternetConnectionOption ico = 
                        GetInternetConnectionOption(
                            PerConnOption.INTERNET_PER_CONN_PROXY_SERVER);
                    // parse out the addr and port
                    string proxyInfo = Marshal.PtrToStringUni(
                                            ico.m_Value.m_StringPtr);
                    ParseProxyInfo(proxyInfo);
                    return _proxyAddr;
                }
            }
            public int ProxyPort
            {
                get
                {
                    InternetConnectionOption ico = 
                        GetInternetConnectionOption(
                            PerConnOption.INTERNET_PER_CONN_PROXY_SERVER);
                    // parse out the addr and port
                    string proxyInfo = Marshal.PtrToStringUni(
                                            ico.m_Value.m_StringPtr);
                    ParseProxyInfo(proxyInfo);
                    return _proxyPort;
                }
            }
            public bool BypassLocalAddresses
            {
                get 
                {
                    InternetConnectionOption ico =
                        GetInternetConnectionOption(
                            PerConnOption.INTERNET_PER_CONN_PROXY_BYPASS);
                    // bypass is listed as <local> in the exceptions list
                    string exceptions = 
                        Marshal.PtrToStringUni(ico.m_Value.m_StringPtr);

                    if (exceptions.IndexOf("<local>") != -1)
                        _bypassLocal = true;
                    else
                        _bypassLocal = false;
                    return _bypassLocal; 
                }
            }
            public string AutoConfigurationAddress
            {
                get 
                {
                    InternetConnectionOption ico =
                        GetInternetConnectionOption(
                            PerConnOption.INTERNET_PER_CONN_AUTOCONFIG_URL);
                    // get these straight
                    _autoConfigAddr = 
                        Marshal.PtrToStringUni(ico.m_Value.m_StringPtr);
                    if (_autoConfigAddr == null)
                        _autoConfigAddr = "";
                    return _autoConfigAddr; 
                }
            }
            public IList<string> ProxyExceptions
            {
                get 
                {
                    InternetConnectionOption ico =
                        GetInternetConnectionOption(
                            PerConnOption.INTERNET_PER_CONN_PROXY_BYPASS);
                    // exceptions are seperated by semi colon
                    string exceptions = 
                        Marshal.PtrToStringUni(ico.m_Value.m_StringPtr);
                    if (!string.IsNullOrEmpty(exceptions))
                    {
                        _proxyExceptions = new List<string>(exceptions.Split(';'));
                    }
                    return _proxyExceptions; 
                }
            }
            public PerConnFlags ConnectionType
            {
                get 
                {
                    InternetConnectionOption ico =
                        GetInternetConnectionOption(
                            PerConnOption.INTERNET_PER_CONN_FLAGS);
                    _flags = (PerConnFlags)ico.m_Value.m_Int;

                    return _flags; 
                }
            }

            #endregion

            #region Private Methods
            private void ParseProxyInfo(string proxyInfo)
            {
                if(!string.IsNullOrEmpty(proxyInfo))
                {
                    string [] parts = proxyInfo.Split(':');
                    if (parts.Length == 2)
                    {
                        _proxyAddr = parts[0];
                        try
                        {
                            _proxyPort = Convert.ToInt32(parts[1]);
                        }
                        catch (FormatException)
                        {
                            // no port
                            _proxyPort = -1;
                        }
                    }
                    else 
                    {
                        _proxyAddr = parts[0];
                        _proxyPort = -1;
                    }
                }
            }

            private static InternetConnectionOption GetInternetConnectionOption(PerConnOption pco)
            {
                //Allocate the list and option.
                InternetPerConnOptionList perConnOptList = new InternetPerConnOptionList();
                InternetConnectionOption ico = new InternetConnectionOption();
                //pin the option structure
                GCHandle gch = GCHandle.Alloc(ico, GCHandleType.Pinned);
                //initialize the option for the data we want
                ico.m_Option = pco;
                //Initialize the option list for the default connection or LAN.
                int listSize = Marshal.SizeOf(perConnOptList);
                perConnOptList.dwSize = listSize;
                perConnOptList.szConnection = IntPtr.Zero;
                perConnOptList.dwOptionCount = 1;
                perConnOptList.dwOptionError = 0;
                // figure out sizes & offsets
                int icoSize = Marshal.SizeOf(ico);
                // alloc enough memory for the option
                perConnOptList.options =
                    Marshal.AllocCoTaskMem(icoSize);

                // Make pointer from the structure
                IntPtr optionListPtr = perConnOptList.options;
                Marshal.StructureToPtr(ico, optionListPtr, false);

                //Make the query
                if (NativeMethods.InternetQueryOption(
                    IntPtr.Zero,
                    75, //(int)InternetOption.INTERNET_OPTION_PER_CONNECTION_OPTION,
                    ref perConnOptList,
                    ref listSize) == true)
                {
                    //retrieve the value
                    ico =
                        (InternetConnectionOption)Marshal.PtrToStructure(perConnOptList.options,
                                                typeof(InternetConnectionOption));
                }
                // free the COM memory
                Marshal.FreeCoTaskMem(perConnOptList.options);
                //unpin the structs
                gch.Free();

                return ico;
            }
            #endregion
        }

		#endregion

        #region "16.10 Download a file using FTP"
        public static void TestFtpDownload()
        {
            try
            {
                Uri ftpSite =
                    new Uri("ftp://ftp.oreilly.com/pub/examples/csharpckbk/CSharpCookbook.zip");
                FtpWebRequest request =
                    (FtpWebRequest)WebRequest.Create(
                    ftpSite);

                request.Credentials = new NetworkCredential("anonymous", "hilyard@oreilly.com");
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Stream data = response.GetResponseStream();
                    string targetPath = "CSharpCookbook.zip";
                    if (File.Exists(targetPath))
                        File.Delete(targetPath);

                    byte[] byteBuffer = new byte[4096];
                    using (FileStream output = new FileStream(targetPath, FileMode.CreateNew))
                    {
                        int bytesRead = 0;
                        do
                        {
                            bytesRead = data.Read(byteBuffer, 0, byteBuffer.Length);
                            if (bytesRead > 0)
                            {
                                output.Write(byteBuffer, 0, bytesRead);
                            }
                        }
                        while (bytesRead > 0);
                    }
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e);
            }
        }

        public static void ResumeFtpFileDownload(Uri sourceUri, string destinationFile)
        {
            FileInfo file = new FileInfo(destinationFile);
            FileStream localfileStream;
            FtpWebRequest request = WebRequest.Create(sourceUri) as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            if (file.Exists)
            {
                request.ContentOffset = file.Length;
                localfileStream = new FileStream(destinationFile, FileMode.Append, FileAccess.Write);
            }
            else
            {
                localfileStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write);
            }
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            byte[] buffer = new byte[1024];
            int bytesRead = responseStream.Read(buffer, 0, 1024);
            while (bytesRead != 0)
            {
                localfileStream.Write(buffer, 0, bytesRead);
                bytesRead = responseStream.Read(buffer, 0, 1024);
            }
            localfileStream.Close();
            responseStream.Close();
        }

        public static void TestFtpUpload()
        {
            try
            {
                string uploadFile = "SampleClassLibrary.dll";
                Uri ftpSite =
                    new Uri("ftp://localhost/Upload/" + uploadFile);
                FileInfo fileInfo = new FileInfo(uploadFile);
                FtpWebRequest request =
                    (FtpWebRequest)WebRequest.Create(
                    ftpSite);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.UseBinary = true;
                request.ContentLength = fileInfo.Length;
                request.Credentials = new NetworkCredential("anonymous", "hilyard@oreilly.com");
                byte[] byteBuffer = new byte[4096];
                using (Stream requestStream = request.GetRequestStream())
                {
                    using (FileStream fileStream = new FileStream(uploadFile, FileMode.Open))
                    {
                        int bytesRead = 0;
                        do
                        {
                            bytesRead = fileStream.Read(byteBuffer, 0, byteBuffer.Length);
                            if (bytesRead > 0)
                            {
                                requestStream.Write(byteBuffer, 0, bytesRead);
                            }
                        }
                        while (bytesRead > 0);
                    }
                }
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine(response.StatusDescription);
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
    }
}
