using System;
using System.Threading;

namespace Tarea_Datos
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread serverThread = new Thread(new ThreadStart(StartServer));
            serverThread.Start();

            Thread.Sleep(1000);

            Client client = new Client();
            client.Start_client();
        }

        static void StartServer()
        {
            Server server = new Server();
            server.Start_server();
        }
    }
}