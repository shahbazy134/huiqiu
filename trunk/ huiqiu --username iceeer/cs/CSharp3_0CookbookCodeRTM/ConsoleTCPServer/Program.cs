using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ConsoleTCPServer
{
    class Program
    {
        static MyTcpServer server;

        static void Main()
        {
            // run the server on a different thread
            ThreadPool.QueueUserWorkItem(RunServer);

            Console.WriteLine("Press Esc to stop the server...");
            ConsoleKeyInfo cki;
            while(true)
            {
                cki = Console.ReadKey();
                if (cki.Key == ConsoleKey.Escape)
                    break;
            }
        }

        static void RunServer( object stateInfo )
        {
            // fire it up
            server = new MyTcpServer(IPAddress.Loopback,55555);
            server.Listen();
        }
    }
}
