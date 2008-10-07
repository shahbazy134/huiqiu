using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

namespace ConsoleTCPClient
{
    class Program
    {
        static void Main()
        {

            MakeClientCallToServer("Just wanted to say hi");
            MakeClientCallToServer("Just wanted to say hi again");
            MakeClientCallToServer("Are you ignoring me?");

            // now send a bunch of messages...
            string msg;
            for (int i = 0; i < 100; i++)
            {
                msg = string.Format(Thread.CurrentThread.CurrentCulture,
                    "I'll not be ignored! (round {0})", i);
                ThreadPool.QueueUserWorkItem(new WaitCallback(MakeClientCallToServer), msg);
            }

            Console.WriteLine("\n Press any key to continue... (if you can find it...)");
            Console.Read();
        }

        static void MakeClientCallToServer(object objMsg)
        {
            string msg = (string)objMsg;
            MyTcpClient client = new MyTcpClient(IPAddress.Loopback, 55555);
            client.ConnectToServer(msg);
        }
    }

}
