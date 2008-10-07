using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

namespace ConsoleTCPClient_SSL
{
    class Program
    {
        static void Main(string[] args)
        {

            MakeClientCallToServer("Just wanted to say hi");
            MakeClientCallToServer("Just wanted to say hi again");
            MakeClientCallToServer("Are you ignoring me?");

            // now really beat on it...

            // make sure we use a bigger number than the number of thread pool threads
            string msg;
            for (int i = 0; i < 100; i++)
            {
                msg = string.Format("I'll not be ignored! (round {0})", i);
                ThreadPool.QueueUserWorkItem(new WaitCallback(MakeClientCallToServer), msg);
            }

            Console.WriteLine("\n Press any key to continue... (if you can find it...)");
            Console.Read();
        }

        static void MakeClientCallToServer(object objMsg)
        {
            string msg = (string)objMsg;
            TCPClient_SSL client = new TCPClient_SSL("127.0.0.1", "55555");
            client.ConnectToServer(string.Format(msg,
                Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId));
        }
    }

}
