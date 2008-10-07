using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;
using System.IO;

using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;


namespace ConsoleTCPServer_SSL
{
    class TCPServer_SSL
    {
        private TcpListener _listener = null;
        private IPAddress _address = IPAddress.Parse("127.0.0.1");
        private int _port = 55555;

        #region CTORs
        public TCPServer_SSL()
        {
        }

        public TCPServer_SSL(string address, string port)
        {
            _port = Convert.ToInt32(port);
            _address = IPAddress.Parse(address);
        }
        #endregion // CTORs

        #region Properties
        public IPAddress Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }
        #endregion

        public void Listen()
        {
            try
            {
                _listener = new TcpListener(_address, _port);

                // fire up the server
                _listener.Start();

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Looking for someone to talk to... ");

                    // Wait for connection
                    TcpClient newClient = _listener.AcceptTcpClient();
                    Console.WriteLine("Connected to new client");

                    // spin a thread to take care of the client
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessClient), newClient);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // shut it down
                _listener.Stop();
            }

            Console.WriteLine("\nHit any key (where is ANYKEY?) to continue...");
            Console.Read();
        }

        private void ProcessClient(object client)
        {
            TcpClient newClient = (TcpClient)client;

            // Buffer for reading data
            byte[] bytes = new byte[1024];
            string clientData = null;


            SslStream sslStream = new SslStream(newClient.GetStream());
			sslStream.AuthenticateAsServer(GetServerCert("MyTestCert2"), false, SslProtocols.Default, true);


            // get the stream to talk to the client over
            //NetworkStream ns = newClient.GetStream();
            
            
            // Loop to receive all the data sent by the client.
            int bytesRead = 0;
            while ((bytesRead = sslStream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to an ASCII string.
                clientData = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                Console.WriteLine("Client says: {0}", clientData);


                // Thank them for their input
                bytes = Encoding.ASCII.GetBytes("Thanks call again!");

                // Send back a response.
                sslStream.Write(bytes, 0, bytes.Length);
            }

            // stop talking to client
            sslStream.Close();
            newClient.Close();
        }

        private static X509Certificate GetServerCert(string subjectName)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            X509CertificateCollection certificate = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true);
            
            if (certificate.Count > 0)
	            return (certificate[0]);
			else
				return (null);
        }
    }
}
