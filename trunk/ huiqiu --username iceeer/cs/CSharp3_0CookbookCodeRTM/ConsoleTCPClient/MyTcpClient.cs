using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

namespace ConsoleTCPClient
{
    class MyTcpClient : IDisposable
    {
        private TcpClient _client;
        private IPAddress _address;
        private int _port;
        private IPEndPoint _endPoint;
        private bool _disposed;


        public MyTcpClient(IPAddress address, int port)
        {
            _address = address;
            _port = port;
            _endPoint = new IPEndPoint(_address, _port);
        }

        public void ConnectToServer(string msg)
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(_endPoint);

                // Get the bytes to send for the message
                byte[] bytes = Encoding.ASCII.GetBytes(msg);
                // get the stream to talk to the server on
                using (NetworkStream ns = _client.GetStream())
                {
                    // send message
                    Trace.WriteLine("Sending message to server: " + msg);
                    ns.Write(bytes, 0, bytes.Length);


                    // Get the response
                    // Buffer to store the response bytes.
                    bytes = new byte[1024];

                    // Display the response
                    int bytesRead = ns.Read(bytes, 0, bytes.Length);
                    string serverResponse = Encoding.ASCII.GetString(bytes, 0, bytesRead);
                    Trace.WriteLine("Server said: " + serverResponse);
                }
            }
            catch (SocketException se)
            {
                Trace.WriteLine("There was an error talking to the server: " +
                    se.ToString());
            }
            finally
            {
                Dispose();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_client != null)
                        _client.Close();
                }
                _disposed = true;
            }
        }

        #endregion
    }
}
