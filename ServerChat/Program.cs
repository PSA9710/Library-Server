using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerChat
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Multi-Threaded TCP Server Demo");
            TcpServer server = new TcpServer(5555);
        }
    }
    class TcpServer
    {
        private TcpListener _server;
        private Boolean _isRunning;

        public TcpServer(int port)
        {
            _server = new TcpListener(IPAddress.Any, port);
            _server.Start();

            _isRunning = true;
            //Process.Start(@"C:\Users\psa97\Desktop\oop\console_client\bin\Debug\console_client.exe");
            LoopClients();
        }
        //    Dictionary<int, TcpClient> userDictionary = new Dictionary<int, TcpClient>();
        List<TcpClient> ListClients = new List<TcpClient>();
        public void LoopClients()
        {
            int i = -1;
            while (_isRunning)
            {
                
                // wait for client connection
                TcpClient newClient = _server.AcceptTcpClient();

                ListClients.Add(newClient);
                // client found.
                // create a thread to handle communication
                Thread t = new Thread(() => HandleClient(newClient, i));
                t.Start();
                i++;
                Console.WriteLine("New User Connected"+i);

            }
        }

        public void HandleClient(object obj, int ind)
        {
            // retrieve client from parameter passed to thread
            TcpClient client = (TcpClient)obj;
            int index = (int)ind;
            // sets two streams
            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
            StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);
            // you could use the NetworkStream to read and write, 
            // but there is no forcing flush, even when requested

            Boolean isClientConnected = true;
            String sData = null;

            //delete this if you use WinForms/WPF
            sWriter.WriteLine("Your ID is " + index.ToString() + " wtf");
            sWriter.Flush();

            while (isClientConnected)
            {
                // reads from stream
                sData = sReader.ReadLine();
                Console.WriteLine(sData);
                if (sData == "#I am OuT#")
                {
                    isClientConnected = false; //escape sequence
                    Console.WriteLine("User " + index.ToString() + " disconnected");
                    ListClients.RemoveAt(index);
                }
                else
                {
                    Scrie(index, sData);  //broadcast to other users
                }
            }
        }

        public void Scrie(int index, String data)
        {
            foreach (var i in ListClients)
            {
                //do not broadcast to itself
                if (i == ListClients[index]) { Console.WriteLine("I was about to send back a message"); }
                else
                {
                    StreamWriter swr = new StreamWriter(i.GetStream(), Encoding.ASCII);
                    swr.WriteLine(data);
                    swr.Flush();
                }
            }
        }
    }
}
