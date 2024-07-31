using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Tarea_Datos
{
    public class Server
    {
        private List<Socket> clients = new List<Socket>();

        public void Start_server()
        {
            try
            {
                Socket listen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint connect = new IPEndPoint(IPAddress.Parse("192.168.0.107"), 5550);

                listen.Bind(connect);
                listen.Listen(10);

                Console.WriteLine("Esperando conexiones...");

                while (true)
                {
                    Socket conexion = listen.Accept();
                    lock (clients)
                    {
                        clients.Add(conexion);
                    }
                    Console.WriteLine("Cliente conectado.");
                    Thread clientThread = new Thread(() => HandleClient(conexion));
                    clientThread.Start();
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine($"Error de socket: {se.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Se produjo un error: {ex.Message}");
            }
        }

        private void HandleClient(Socket client)
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[100];
                    int received = client.Receive(buffer);

                    if (received == 0)
                    {
                        // El cliente ha cerrado la conexión
                        break;
                    }

                    string data = Encoding.UTF8.GetString(buffer, 0, received);
                    Console.WriteLine($"Mensaje recibido: {data}");

                    // Reenviar el mensaje a todos los clientes conectados
                    lock (clients)
                    {
                        foreach (Socket otherClient in clients)
                        {
                            if (otherClient != client)
                            {
                                otherClient.Send(buffer, received, SocketFlags.None);
                            }
                        }
                    }
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine($"Error de socket: {se.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Se produjo un error: {ex.Message}");
            }
            finally
            {
                lock (clients)
                {
                    clients.Remove(client);
                }
                client.Close();
            }
        }
    }
}
