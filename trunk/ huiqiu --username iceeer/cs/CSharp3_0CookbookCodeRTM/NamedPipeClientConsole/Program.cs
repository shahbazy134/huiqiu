using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.IO.Pipes;

namespace NamedPipes
{
    class NamedPipeClientConsole
    {
        static void Main()
        {
            // set up a message to send
            string messageText = "This is my message!";
            int bytesRead;

            // set up the named pipe client and close it when complete
            using (NamedPipeClientStream clientPipe =
                    new NamedPipeClientStream(".", "mypipe", PipeDirection.InOut, PipeOptions.None))
            {
                // connect to the server stream
                clientPipe.Connect();
                // set the read mode to message
                clientPipe.ReadMode = PipeTransmissionMode.Message;

                // write the message ten times
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("Sending message: " + messageText);
                    byte[] messageBytes = Encoding.Unicode.GetBytes(messageText);
                    // check and write the message
                    if (clientPipe.CanWrite)
                    {
                        clientPipe.Write(messageBytes, 0, messageBytes.Length);
                        clientPipe.Flush();
                        // wait till it is read
                        clientPipe.WaitForPipeDrain();
                    }

                    // set up a buffer for the message bytes
                    messageBytes = new byte[256];
                    do
                    {
                        // collect the message bits in the stringbuilder
                        StringBuilder message = new StringBuilder();

                        // read all of the bits until we have the 
                        // complete response message
                        do
                        {
                            // read from the pipe
                            bytesRead =
                                clientPipe.Read(messageBytes, 0, messageBytes.Length);
                            // if we got something, add it to the message
                            if (bytesRead > 0)
                            {
                                message.Append(
                                    Encoding.Unicode.GetString(messageBytes, 0, bytesRead));
                                Array.Clear(messageBytes, 0, messageBytes.Length);
                            }
                        }
                        while (!clientPipe.IsMessageComplete);

                        // set to zero as we have read the whole message
                        bytesRead = 0;
                        Console.WriteLine("    Received message: " + message.ToString());
                    }
                    while (bytesRead != 0);

                }
            }

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
