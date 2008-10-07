using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Linq;
using System.Security;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net.Cache;
using System.Web;
using System.Reflection;
using System.Web.Compilation;
using System.CodeDom.Compiler;
using System.Configuration;
using System.DirectoryServices;
using System.Security.Permissions;
using System.Collections.Generic;

namespace CSharpRecipes
{
	public class Web
	{
		private static string GetWebAppPath()
		{
			string path = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

			#region Code to make it work in debug...
			// figure out where the web code is, should be in an adjacent directory
			// to the console app code
			string cscbWebPath = path;
			int index = -1;
			// keep backing up directories till we find it
			while (!Directory.Exists(cscbWebPath + @"\CSCBWeb"))
			{
				index = cscbWebPath.LastIndexOf('\\');
				if (index == -1)
				{
					cscbWebPath = "";
					break;
				}
				cscbWebPath = cscbWebPath.Substring(0, index);
			}
			#endregion

			// make sure we have a web path
			if (cscbWebPath.Length > 0)
			{
				// append webdir name
				cscbWebPath += @"\CSCBWeb";
			}
			return cscbWebPath;
		}

		#region "14.1 Converting an IP Address to a Host Name"
		public static void ConvertIPToHostName()
		{
			// use the dns class to resolve from the address to the iphostentry
            try
            {
                IPHostEntry iphost = Dns.GetHostEntry("90.0.0.47");
                string hostName = iphost.HostName;
                Console.WriteLine(hostName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
		}
		#endregion

		#region "14.2 Converting a Host Name to an IP Address"
		public static void ConvertingHostNameToIP()
		{
			// writes "IP Address: 208.201.239.37;IP Address: 208.201.239.36;"
			Console.WriteLine(HostNameToIP("www.oreilly.com"));
        }

		public static string HostNameToIP(string hostName)
		{
			// resolve the hostname into an iphost entry using the dns class
            IPHostEntry iphost = System.Net.Dns.GetHostEntry(hostName);

            // get all of the possible IP addresses for this hostname
			IPAddress[] addresses = iphost.AddressList;

			// make a text representation of the list
			StringBuilder addressList = new StringBuilder();
			// get each ip address
			foreach (IPAddress address in addresses)
			{
				// append it to the list
                addressList.AppendFormat("IP Address: {0};", address.ToString());
			}
			return addressList.ToString();
		}

		#endregion

		#region "14.3 Parsing a URI"
		public static void ParsingUri()
		{
			Web.ParseUri(new Uri("http://user:password@localhost:8080/www.abc.com/home%20page.htm?item=1233#stuff"));
		}
		public static void ParseUri(Uri uri)
		{
			try
			{
		        // System.Net.Uri class constructor has parsed it for us.
                // new Uri("http://user:password@localhost:8080/www.abc.com/home%20page.htm?item=1233#stuff")

                // Look at the information we can get at now...
		        StringBuilder uriParts = new StringBuilder();
		        uriParts.AppendFormat("AbsoluteURI: {0}{1}",
                                    uri.AbsoluteUri,Environment.NewLine);
		        uriParts.AppendFormat("AbsolutePath: {0}{1}",
                                    uri.AbsolutePath,Environment.NewLine);
		        uriParts.AppendFormat("Scheme: {0}{1}",
                                    uri.Scheme,Environment.NewLine);
		        uriParts.AppendFormat("UserInfo: {0}{1}",
                                    uri.UserInfo,Environment.NewLine);
		        uriParts.AppendFormat("Authority: {0}{1}",
                                    uri.Authority,Environment.NewLine);
		        uriParts.AppendFormat("DnsSafeHost: {0}{1}",
                                    uri.DnsSafeHost,Environment.NewLine);
		        uriParts.AppendFormat("Host: {0}{1}",
                                    uri.Host,Environment.NewLine);
		        uriParts.AppendFormat("HostNameType: {0}{1}",
                                    uri.HostNameType.ToString(),Environment.NewLine);
		        uriParts.AppendFormat("Port: {0}{1}",uri.Port,Environment.NewLine);
		        uriParts.AppendFormat("Path: {0}{1}",uri.LocalPath,Environment.NewLine);
		        uriParts.AppendFormat("QueryString: {0}{1}",uri.Query,Environment.NewLine);
		        uriParts.AppendFormat("Path and QueryString: {0}{1}",
                                    uri.PathAndQuery,Environment.NewLine);
		        uriParts.AppendFormat("Fragment: {0}{1}",uri.Fragment,Environment.NewLine);
		        uriParts.AppendFormat("Original String: {0}{1}",
                                    uri.OriginalString,Environment.NewLine);
		        uriParts.AppendFormat("Segments: {0}",Environment.NewLine);
		        for (int i = 0; i < uri.Segments.Length; i++)
			        uriParts.AppendFormat("	Segment {0}:{1}{2}",
                                    i, uri.Segments[i], Environment.NewLine);


		        // GetComponents can be used to get commonly used combinations
		        // of Uri information
		        uriParts.AppendFormat("GetComponents for specialized combinations: {0}",
                        	        Environment.NewLine);
		        uriParts.AppendFormat("Host and Port (unescaped): {0}{1}",
					                uri.GetComponents(UriComponents.HostAndPort,
                                    UriFormat.Unescaped),Environment.NewLine);
		        uriParts.AppendFormat("HttpRequestUrl (unescaped): {0}{1}",
					                uri.GetComponents(UriComponents.HttpRequestUrl, 
						            UriFormat.Unescaped),Environment.NewLine);
		        uriParts.AppendFormat("HttpRequestUrl (escaped): {0}{1}",
					                uri.GetComponents(UriComponents.HttpRequestUrl, 
						            UriFormat.UriEscaped),Environment.NewLine);
		        uriParts.AppendFormat("HttpRequestUrl (safeunescaped): {0}{1}",
					                uri.GetComponents(UriComponents.HttpRequestUrl, 
						            UriFormat.SafeUnescaped),Environment.NewLine);
		        uriParts.AppendFormat("Scheme And Server (unescaped): {0}{1}",
					                uri.GetComponents(UriComponents.SchemeAndServer, 
						            UriFormat.Unescaped),Environment.NewLine);
		        uriParts.AppendFormat("SerializationInfo String (unescaped): {0}{1}",
					                uri.GetComponents(UriComponents.SerializationInfoString, 
						            UriFormat.Unescaped),Environment.NewLine);
		        uriParts.AppendFormat("StrongAuthority (unescaped): {0}{1}",
					                uri.GetComponents(UriComponents.StrongAuthority, 
						            UriFormat.Unescaped),Environment.NewLine);
		        uriParts.AppendFormat("StrongPort (unescaped): {0}{1}",
				                    uri.GetComponents(UriComponents.StrongPort, 
						            UriFormat.Unescaped),Environment.NewLine);

		        // write out our summary
		        Console.WriteLine(uriParts.ToString());
			}
			catch (ArgumentNullException e)
			{
				// uriString is a null reference (Nothing in Visual Basic). 
				Console.WriteLine("URI string object is a null reference: {0}", e);
			}
			catch (UriFormatException e)
			{
				Console.WriteLine("URI formatting error: {0}", e);
			}
		}

		#endregion

		#region "14.4 Handling Web Server Errors"
		public enum ResponseCategories
		{
			Unknown,        // unknown code  ( < 100 or > 599)
			Informational,  // informational codes (100 <= 199)
			Success,        // success codes (200 <= 299)
			Redirected,     // redirection code (300 <= 399)
			ClientError,    // client error code (400 <= 499)
			ServerError     // server error code (500 <= 599)
		}

		public static void HandlingWebServerErrors()
		{
			HttpWebRequest httpRequest = null;
			// get a URI object
			Uri uri = new Uri("http://localhost");
			// create the initial request
			httpRequest = (HttpWebRequest)WebRequest.Create(uri);
			HttpWebResponse httpResponse = null;
			try
			{
				httpResponse = (HttpWebResponse)httpRequest.GetResponse();
			}
			catch (WebException we)
			{
				Console.WriteLine(we.ToString());
				return;
			}
			switch (CategorizeResponse(httpResponse))
			{
				case ResponseCategories.Unknown:
					Console.WriteLine("Unknown");
					break;
				case ResponseCategories.Informational:
					Console.WriteLine("Informational");
					break;
				case ResponseCategories.Success:
					Console.WriteLine("Success");
					break;
				case ResponseCategories.Redirected:
					Console.WriteLine("Redirected");
					break;
				case ResponseCategories.ClientError:
					Console.WriteLine("ClientError");
					break;
				case ResponseCategories.ServerError:
					Console.WriteLine("ServerError");
					break;

			}
		}

		public static ResponseCategories CategorizeResponse(HttpWebResponse httpResponse)
		{
			// Just in case there are more success codes defined in the future
			// by HttpStatusCode, we will check here for the "success" ranges
			// instead of using the HttpStatusCode enum as it overloads some
			// values
			int statusCode = (int)httpResponse.StatusCode;
			if ((statusCode >= 100) && (statusCode <= 199))
			{
				return ResponseCategories.Informational;
			}
			else if ((statusCode >= 200) && (statusCode <= 299))
			{
				return ResponseCategories.Success;
			}
			else if ((statusCode >= 300) && (statusCode <= 399))
			{
				return ResponseCategories.Redirected;
			}
			else if ((statusCode >= 400) && (statusCode <= 499))
			{
				return ResponseCategories.ClientError;
			}
			else if ((statusCode >= 500) && (statusCode <= 599))
			{
				return ResponseCategories.ServerError;
			}
			return ResponseCategories.Unknown;
		}

		#endregion

		#region "14.5 Communicating with a Web Server"
		public static void CommunicatingWithWebServer()
		{
            HttpWebRequest request =
                GenerateHttpWebRequest(new Uri("http://localhost/mysite/index.aspx"));

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                // This next line uses VerifyResponse from Recipe 14.5
                if (CategorizeResponse(response) == ResponseCategories.Success)
                {
                    Console.WriteLine("Request succeeded");
                }
            }
		}

        // GET overload
        public static HttpWebRequest GenerateHttpWebRequest(Uri uri)
        {
            // create the initial request
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(uri);
            // return the request
            return httpRequest;
        }

        // POST overload
        public static HttpWebRequest GenerateHttpWebRequest(Uri uri,
	        string postData,
            string contentType)
        {
	        // create the initial request
            HttpWebRequest httpRequest = GenerateHttpWebRequest(uri);

	        // Get the bytes for the request, should be pre-escaped
	        byte[] bytes = Encoding.UTF8.GetBytes(postData);

	        // Set the content type of the data being posted.
            httpRequest.ContentType = contentType;
		        //"application/x-www-form-urlencoded"; for forms

	        // Set the content length of the string being posted.
	        httpRequest.ContentLength = postData.Length;

	        // Get the request stream and write the post data in
            using (Stream requestStream = httpRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }
	        // return the request
	        return httpRequest;
        }

		#endregion

		#region "14.6 Going Through a Proxy"
		public static void GoingThroughProxy()
		{
			HttpWebRequest request =
				GenerateHttpWebRequest(new Uri("http://internethost/mysite/index.aspx"));

			// add the proxy info
			AddProxyInfoToRequest(request, 
                                new Uri("http://webproxy:80"), 
                                "user", 
                                "pwd", 
                                "domain");

			// or set it up to go through the same proxy for all responses
			Uri proxyURI = new Uri("http://webproxy:80");

			// in 1.1 you used to do this:
			//GlobalProxySelection.Select = new WebProxy(proxyURI);
			// Now in 2.0 you do this:
			WebRequest.DefaultWebProxy = new WebProxy(proxyURI);
            // for the current user Internet Explorer proxy info use this
            WebRequest.DefaultWebProxy = WebRequest.GetSystemWebProxy();
		}

		public static HttpWebRequest AddProxyInfoToRequest(HttpWebRequest httpRequest,
			Uri proxyUri,
			string proxyId,
			string proxyPassword,
			string proxyDomain)
		{
			if (httpRequest != null)
			{
				// create the proxy object
				WebProxy proxyInfo = new WebProxy();
				// add the address of the proxy server to use
				proxyInfo.Address = proxyUri;
				// tell it to bypass the proxy server for local addresses
				proxyInfo.BypassProxyOnLocal = true;
				// add any credential information to present to the proxy server
				proxyInfo.Credentials = new NetworkCredential(proxyId,
                    proxyPassword,
					proxyDomain);
				// assign the proxy information to the request
				httpRequest.Proxy = proxyInfo;
			}
			// return the request
			return httpRequest;
		}

		#endregion

		#region "14.7 Obtaining the HTML from a URL"
		public static void ObtainingHtmlFromUrl()
		{
            try
            {
                string html = GetHtmlFromUrl(new Uri("http://www.oreilly.com"));
            }
            catch (WebException e)
            {
                Console.WriteLine(e);
            }
		}

        public static string GetHtmlFromUrl(Uri url)
        {
	        string html = string.Empty;
	        HttpWebRequest request = GenerateHttpWebRequest(url);
	        using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
		        if (CategorizeResponse(response) == ResponseCategories.Success)
		        {
			        // get the response stream.
			        Stream responseStream = response.GetResponseStream();
                    // use a stream reader that understands UTF8
                    using(StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                    {
	                    html = reader.ReadToEnd();
                    }
		        }
	        }
	        return html;
        }

		#endregion

		#region "14.8 Using the new web browser control"
		// See the WebBrowser project 
		#endregion 

		#region "14.9 Tying your database code to the cache"
		// See the CSCBWeb project for this code 
		#endregion

		#region "14.10 Pre-building an ASP.NET web site programmatically"
		public class MyClientBuildManagerCallback : ClientBuildManagerCallback
		{
			public MyClientBuildManagerCallback()
				: base()
			{
			}

            [PermissionSet(SecurityAction.Demand, Unrestricted = true)]
			public override void ReportCompilerError(CompilerError error)
			{
				string msg = "Report Compiler Error: " + error.ToString();
				Debug.WriteLine(msg);
				Console.WriteLine(msg);
			}

            [PermissionSet(SecurityAction.Demand, Unrestricted = true)]
			public override void ReportParseError(ParserError error)
			{
				string msg = "Report Parse Error: " + error.ToString();
				Debug.WriteLine(msg);
				Console.WriteLine(msg);
			}

            [PermissionSet(SecurityAction.Demand, Unrestricted = true)]
            public override void ReportProgress(string message)
			{
				string msg = "Report Progress: " + message;
				Debug.WriteLine(msg);
				Console.WriteLine(msg);
			}
		}

		public static void TestBuildAspNetPages()
		{
			try
			{
				// get the path to the web app shipping with this code...
				string cscbWebPath = GetWebAppPath();

				// make sure we have a web path
				if(cscbWebPath.Length>0)
				{
					string appVirtualDir = @"CSCBWeb";
					string appPhysicalSourceDir = cscbWebPath;
					// make the target an adjacent directory as it cannot be in the same tree
					// or the buildmanager screams...
					string appPhysicalTargetDir = Path.GetDirectoryName(cscbWebPath) + @"\BuildCSCB"; 

					// check flags again in Beta2, more options...
					//AllowPartiallyTrustedCallers   
					//Clean   
					//CodeAnalysis   
					//Default   
					//DelaySign   
					//FixedNames   
					//ForceDebug   
					//OverwriteTarget   
					//Updatable 

//                    Report Progress: Building directory '/CSCBWeb/App_Data'.
//Report Progress: Building directory '/CSCBWeb/Role_Database'.
//Report Progress: Building directory '/CSCBWeb'.
//Report Compiler Error: c:\PRJ32\Book_2_0\C#Cookbook2\Code\CSCBWeb\Default.aspx.cs(14,7) : warning CS0105: The using directive for 'System.Configuration' appeared previously in this namespace


//                    Report Progress: Building directory '/CSCBWeb/App_Data'.
//Report Progress: Building directory '/CSCBWeb/Role_Database'.
//Report Progress: Building directory '/CSCBWeb'.

					PrecompilationFlags flags = PrecompilationFlags.ForceDebug |
												PrecompilationFlags.OverwriteTarget;

                    ClientBuildManagerParameter cbmp = new ClientBuildManagerParameter();
                    cbmp.PrecompilationFlags = flags;
					ClientBuildManager cbm =
							new ClientBuildManager(appVirtualDir, 
													appPhysicalSourceDir, 
													appPhysicalTargetDir,
                                                    cbmp);
					MyClientBuildManagerCallback myCallback = new MyClientBuildManagerCallback();
					cbm.PrecompileApplication(myCallback);
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
			}
		}
		#endregion

		#region "14.11 Escaping and Unescaping data for the web"
		public static void TestEscapeUnescape()
		{
            string data = "<H1>My html</H1>";
            Console.WriteLine("Original Data: {0}",data);
            Console.WriteLine();
            // public static string EscapeDataString(string stringToEscape);
            string escapedData = Uri.EscapeDataString(data);
            Console.WriteLine("escaped Data: {0}",escapedData);
            Console.WriteLine();
            // public static string UnescapeDataString(	string stringToUnescape);
            string unescapedData = Uri.UnescapeDataString(escapedData);
            Console.WriteLine("unescaped Data: {0}", unescapedData);
            Console.WriteLine();
            string uriString = "http://user:password@localhost:8080/www.abc.com/" + 
	            "home page.htm?item=1233;html=<h1>Heading</h1>#stuff";
            Console.WriteLine("Original Uri string: {0}",uriString);
            Console.WriteLine();

            // public static string EscapeUriString(string stringToEscape);
            string escapedUriString = Uri.EscapeUriString(uriString);
            Console.WriteLine("Escaped Uri string: {0}",escapedUriString);
            Console.WriteLine();

            // Why not just use EscapeDataString to escape a Uri?  It's not picky enough...
            string escapedUriData = Uri.EscapeDataString(uriString);
            Console.WriteLine("Escaped Uri data: {0}",escapedUriData);
            Console.WriteLine();

            Console.WriteLine(Uri.UnescapeDataString(escapedUriString));
		}
		#endregion

		#region "14.12 Bulletproof creating a Uri"
        public class UriBuilderFix : UriBuilder
        {
	        public UriBuilderFix() : base()
	        {
	        }

	        public new string Query
	        {
		        get
		        {
			        return base.Query;
		        }
 		        set
		        {
			        if (!string.IsNullOrEmpty(value))
			        {
				        if (value[0] == '?')
					        // trim off the leading ? as the underlying 
					        // UriBuilder class will add one to the 
					        // querystring.  Also prepend ; for additional items
					        base.Query = value.Substring(1);
				        else
					        base.Query = value;
			        }
			        else
				        base.Query = string.Empty;
		        }
	        }
        }
		
		public static void TestUriBuilder()
		{
			//string uriString = "http://user:password@localhost:8080/www.abc.com/" + 
			//	"home page.htm?item=1233;html=<h1>Heading</h1>#stuff";
			//[scheme]://[user]:[password]@[host/authority]:[port]/[path];[params]?
			//[query string]#[fragment]

			try
			{
                UriBuilderFix ubf = new UriBuilderFix();
                ubf.Scheme = "http";
                ubf.UserName = "user";
                ubf.Password = "password";
                ubf.Host = "localhost";
                ubf.Port = 8080;
                ubf.Path = "www.abc.com/home page.htm";

                //The Query property contains any query information included in the URI. 
                //Query information is separated from the path information by a question mark (?) and continues 
                //to the end of the URI. The query information returned includes the leading question mark.
                //The query information is escaped according to RFC 2396.
                //Setting the Query property to null or to System.String.Empty clears the property.
                //Note:   Do not append a string directly to this property. 
                //Instead retrieve the property value as a string, remove the leading question mark, 
                //append the new query string, and set the property with the combined string.

                // Don't Do This, too many ? marks, one for every append...
                ubf.Query = "item=1233";
                ubf.Query += ";html=<h1>heading</h1>";

                ubf.Fragment = "stuff";

                Console.WriteLine("Absolute Composed Uri: " + ubf.Uri.AbsoluteUri); 
                Console.WriteLine("Composed Uri: " + ubf.ToString());

                UriBuilder ub = new UriBuilder();
                ub.Query = "item=1233";
                ub.Query += ";html=<h1>heading</h1>";
				
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception thrown building Uri: " + e.ToString());
			}
		}
		#endregion

		#region "14.13 Inspect and change your web application configuration (System.Web.Configuration)"
		// see the CSCBWeb project for code
		#endregion

		#region "14.14 Using cached results when working with HTTP for faster performance"
		public static void TestCache()
		{
			// 14.6 has generate get or post request
			string html = string.Empty;
			// set up the request
			HttpWebRequest request = 
                GenerateHttpWebRequest(new Uri("http://www.oreilly.com"));
			
			// make a cache policy to use cached results if available
			// the default is to Bypass the cache in machine.config.
			RequestCachePolicy rcpCheckCache = 
				new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
			
			// assign the new policy to the request
			request.CachePolicy = rcpCheckCache;

			// execute the request
			HttpWebResponse response = null;
			try
			{
				response = (HttpWebResponse)request.GetResponse();
			    // check if we hit the cache
			    if(response.IsFromCache==false)
			    {
				    Console.WriteLine("Didn't hit the cache");
			    }

			    // get the response stream.
			    Stream responseStream = response.GetResponseStream();
			    // use a stream reader that understands UTF8
			    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);

			    try
			    {
				    html = reader.ReadToEnd();
			    }
			    finally
			    {
				    // close the reader
				    reader.Close();
			    }
			    Console.WriteLine("Html retrieved: " + html);
			}
			catch (WebException we)
			{
				Console.WriteLine(we.ToString());
			}
		}
		#endregion 

        #region "14.15 Checking out a web server's custom error pages
        public static void GetCustomErrorPageLocations()
        {
            // MetaEdit can be gotten here: http://support.microsoft.com/kb/q232068/
            // Find metabase properties here
            // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/iissdk/html/4930f687-3a39-4f7d-9bc7-56dbb013d2b5.asp

            // Sometimes the COM world is a rough place...
            UInt32 E_RPC_SERVER_UNAVAILABLE = 0x800706BA;

            try
            {
                // This is a case sensitive entry in the metabase
                // You'd think it was misspelled but you would be mistaken...
                const string WebServerSchema = "IIsWebServer";

                // set up to talk to the local IIS server
                string server = "localhost";

                // Create a dictionary entry for the IIS server with a fake
                // user and password.  Credentials would have to be provided
                // if you are running as a regular user
                using (DirectoryEntry w3svc = 
                    new DirectoryEntry(
                        string.Format("IIS://{0}/w3svc", server), 
                            "Domain/UserCode", "Password"))
                {
                    // can't talk to the metabase for some reason, bail
                    if (w3svc != null)
                    {
                        foreach (DirectoryEntry site in w3svc.Children)
                        {
                            if (site != null)
                            {
                                using (site)
                                {
                                    // check all web servers on this box
                                    if (site.SchemaClassName == WebServerSchema)
                                    {
                                        // get the metabase entry for this server
                                        string metabaseDir = 
                                            string.Format("/w3svc/{0}/ROOT", site.Name);

                                        if (site.Children != null)
                                        {
                                            // find the ROOT directory for each server
                                            foreach (DirectoryEntry root in site.Children)
                                            {
                                                using (root)
                                                {
                                                    // did we find the root dir for this site?
                                                    if (root != null && 
                                                        root.Name.Equals("ROOT", 
                                                            StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        // get the HttpErrors
                                                        if (root.Properties.Contains("HttpErrors") == true)
                                                        {
                                                            // write them out
                                                            PropertyValueCollection httpErrors = root.Properties["HttpErrors"];
                                                            if (httpErrors != null)
                                                            {
                                                                for (int i = 0; i < httpErrors.Count; i++)
                                                                {
                                                                    //400,*,FILE,C:\WINDOWS\help\iisHelp\common\400.htm
                                                                    string[] errorParts = httpErrors[i].ToString().Split(',');
                                                                    Console.WriteLine("Error Mapping Entry:");
                                                                    Console.WriteLine("\tHTTP error code: {0}", errorParts[0]);
                                                                    Console.WriteLine("\tHTTP sub-error code: {0}", errorParts[1]);
                                                                    Console.WriteLine("\tMessage Type: {0}", errorParts[2]);
                                                                    Console.WriteLine("\tPath to error HTML file: {0}", errorParts[3]);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (COMException e)
            {
                // apparently it won't talk to us right now
                // this could be set up in a loop to retry....
                if (e.ErrorCode != (Int32)E_RPC_SERVER_UNAVAILABLE)
                {
                    throw;
                }
            }
        }

        public static void GetCustomErrorPageLocationsLinq()
        {
            // MetaEdit can be gotten here: http://support.microsoft.com/kb/q232068/
            // Find metabase properties here
            // http://msdn.microsoft.com/library/default.asp?url=/library/en-us/iissdk/html/4930f687-3a39-4f7d-9bc7-56dbb013d2b5.asp

            // Sometimes the COM world is a rough place...
            UInt32 E_RPC_SERVER_UNAVAILABLE = 0x800706BA;

            try
            {
                // This is a case sensitive entry in the metabase
                // You'd think it was misspelled but you would be mistaken...
                const string WebServerSchema = "IIsWebServer";

                // set up to talk to the local IIS server
                string server = "localhost";

                // Create a dictionary entry for the IIS server with a fake
                // user and password.  Credentials would have to be provided
                // if you are running as a regular user
                using (DirectoryEntry w3svc =
                    new DirectoryEntry(
                        string.Format("IIS://{0}/w3svc", server),
                            "Domain/UserCode", "Password"))
                {
                    // can't talk to the metabase for some reason, bail
                    if (w3svc != null)
                    {
                        // Break up the query using Explicit dot notation into getting the site, then the http error property values
                        //var sites = w3svc.Children.OfType<DirectoryEntry>()
                        //            .Where(child => child.SchemaClassName == WebServerSchema)
                        //            .SelectMany(child => child.Children.OfType<DirectoryEntry>());
                        //var httpErrors = sites
                        //               .Where(site => site.Name == "ROOT")
                        //               .SelectMany<DirectoryEntry,string>(site => site.Properties["HttpErrors"].OfType<string>());

                        // Combine the query using Explicit dot notation
                        //var httpErrors = w3svc.Children.OfType<DirectoryEntry>()
                        //                    .Where(site => site.SchemaClassName == WebServerSchema)
                        //                    .SelectMany(siteDir => siteDir.Children.OfType<DirectoryEntry>())
                        //                    .Where(siteDir => siteDir.Name == "ROOT")
                        //                    .SelectMany<DirectoryEntry, string>(siteDir => siteDir.Properties["HttpErrors"].OfType<string>());

                        // Use a regular query expression to
                        // select the http errors for all web sites on the machine
                        var httpErrors = from site in
                                             w3svc.Children.OfType<DirectoryEntry>()
                                         where site.SchemaClassName == WebServerSchema
                                         from siteDir in
                                             site.Children.OfType<DirectoryEntry>()
                                         where siteDir.Name == "ROOT"
                                         from httpError in siteDir.Properties["HttpErrors"].OfType<string>()
                                         select httpError;

                        // use eager evaluation to convert this to the array
                        // so that we don't requery on each iteration.  We would miss
                        // updates to the metabase that occur during execution but
                        // that is a small price to pay vs. the requery cost.
                        // This will force the evaluation of the query now once.
                        string[] errors = httpErrors.ToArray();
                        foreach (var httpError in errors)
                        {
                            //400,*,FILE,C:\WINDOWS\help\iisHelp\common\400.htm
                            string[] errorParts = httpError.ToString().Split(',');
                            Console.WriteLine("Error Mapping Entry:");
                            Console.WriteLine("\tHTTP error code: {0}", errorParts[0]);
                            Console.WriteLine("\tHTTP sub-error code: {0}", errorParts[1]);
                            Console.WriteLine("\tMessage Type: {0}", errorParts[2]);
                            Console.WriteLine("\tPath to error HTML file: {0}", errorParts[3]);
                        }
                    }
                }
            }
            catch (COMException e)
            {
                // apparently it won't talk to us right now
                // this could be set up in a loop to retry....
                if (e.ErrorCode != (Int32)E_RPC_SERVER_UNAVAILABLE)
                {
                    throw;
                }
            }
        }

        #endregion
	}
}
