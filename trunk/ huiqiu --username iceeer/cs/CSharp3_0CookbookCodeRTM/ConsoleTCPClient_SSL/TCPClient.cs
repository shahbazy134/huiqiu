using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;


namespace ConsoleTCPClient_SSL
{
    class TCPClient_SSL
    {
        private TcpClient _client = null;
        private IPAddress _address = IPAddress.Parse("127.0.0.1");
        private int _port = 5;
        private IPEndPoint _endPoint = null;

        public TCPClient_SSL(string address, string port)
        {
            _address = IPAddress.Parse(address);
            _port = Convert.ToInt32(port);
            _endPoint = new IPEndPoint(_address, _port);
        }

        public void ConnectToServer(string msg)
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(_endPoint);



                SslStream sslStream = new SslStream(_client.GetStream(), false, new RemoteCertificateValidationCallback(CertificateValidationCallback));
				sslStream.AuthenticateAsClient("MyTestCert2");
				DisplaySSLInformation("MyTestCert2", sslStream, true);


                // Get the bytes to send for the message
                byte[] bytes = Encoding.ASCII.GetBytes(msg);
                
                // get the stream to talk to the server on
                //NetworkStream ns = _client.GetStream();
                
                // send message
                Console.WriteLine("Sending message to server: " + msg);
                sslStream.Write(bytes, 0, bytes.Length);


                // Get the response
                // Buffer to store the response bytes.
                bytes = new byte[1024];

                // Display the response
                int bytesRead = sslStream.Read(bytes, 0, bytes.Length);
                string serverResponse = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                Console.WriteLine("Server said: " + serverResponse);

                // Close everything.
                sslStream.Close();
                _client.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine("There was an error talking to the server: {0}",
                    e.ToString());
            }
        }

        private bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
            {
                return true;
            }
            else
            {
                if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
                {
					Console.WriteLine("The X509Chain.ChainStatus returned an array of " + 
						"X509ChainStatus objects containing error information.");
				}
                else if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
                {
					Console.WriteLine("There was a mismatch of the name on a certificate.");
				}
                else if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNotAvailable)
                {
					Console.WriteLine("No certificate was available.");
				}
                else
                {
                    Console.WriteLine("SSL Certificate Validation Error!");
                }

                Console.WriteLine(Environment.NewLine + "SSL Certificate Validation Error!");
                Console.WriteLine(sslPolicyErrors.ToString());

                return false;
            }
        }

        private static void DisplaySSLInformation(string serverName, SslStream sslStream, bool verbose)
        {
            DisplayCertInformation(sslStream.RemoteCertificate, verbose);

            Console.WriteLine("\n\nSSL Connect Report for : {0}\n", serverName);
            Console.WriteLine("Is Authenticated:            {0}", sslStream.IsAuthenticated);
            Console.WriteLine("Is Encrypted:                {0}", sslStream.IsEncrypted);
            Console.WriteLine("Is Signed:                   {0}", sslStream.IsSigned);
            Console.WriteLine("Is Mutually Authenticated:   {0}\n", sslStream.IsMutuallyAuthenticated);

            Console.WriteLine("Hash Algorithm:              {0}", sslStream.HashAlgorithm);
            Console.WriteLine("Hash Strength:               {0}", sslStream.HashStrength);
            Console.WriteLine("Cipher Algorithm:            {0}", sslStream.CipherAlgorithm);
            Console.WriteLine("Cipher Strength:             {0}\n", sslStream.CipherStrength);

            Console.WriteLine("Key Exchange Algorithm:      {0}", sslStream.KeyExchangeAlgorithm);
            Console.WriteLine("Key Exchange Strength:       {0}\n", sslStream.KeyExchangeStrength);
            Console.WriteLine("SSL Protocol:                {0}", sslStream.SslProtocol);


        }

        private static void DisplayCertInformation(X509Certificate remoteCertificate, bool verbose)
        {
            Console.WriteLine("Certficate Information for:\n{0}\n", remoteCertificate.Subject);
            Console.WriteLine("Valid From:      \n{0}", remoteCertificate.GetEffectiveDateString());
            Console.WriteLine("Valid To:        \n{0}", remoteCertificate.GetExpirationDateString());
            Console.WriteLine("Certificate Format:     \n{0}\n", remoteCertificate.GetFormat());

            Console.WriteLine("Issuer Name:     \n{0}", remoteCertificate.Issuer);

            if (verbose)
            {
                Console.WriteLine("Serial Number:   \n{0}", remoteCertificate.GetSerialNumberString());
                Console.WriteLine("Hash:            \n{0}", remoteCertificate.GetCertHashString());
                Console.WriteLine("Key Algorithm:   \n{0}", remoteCertificate.GetKeyAlgorithm());
                Console.WriteLine("Key Algorithm Parameters:     \n{0}", remoteCertificate.GetKeyAlgorithmParametersString());
                Console.WriteLine("Public Key:     \n{0}", remoteCertificate.GetPublicKeyString());
            }
        }
    }
}
