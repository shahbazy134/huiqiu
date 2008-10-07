using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConsoleTCPServer_SSL
{
    class Program
    {
        static void Main(string[] args)
        {
            TCPServer_SSL server = new TCPServer_SSL();
            server.Listen();
        }
    }
}
