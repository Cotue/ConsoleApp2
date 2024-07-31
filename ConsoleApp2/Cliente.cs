using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Tarea_Datos
{
    public class Client
    {
        public void Start_client()
        {
            try
            {
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse("192.168.0.107"), 5550);

                clientSocket.Connect(serverEndpoint);

                Thread receiveThread = new Thread(() => ReceiveMessages(clientSocket));
                receiveThread.Start();

                while (true)
                {
                    Console.WriteLine("Ingrese su mensaje (o 'salir' para terminar):");
                    string data = Console.ReadLine();

                    if (data.ToLower() == "salir")
                    {
                        byte[] info_salir = Encoding.UTF8.GetBytes(data);
                        clientSocket.Send(info_salir);
                        break;
                    }

                    byte[] info_a_enviar = Encoding.UTF8.GetBytes(data);
                    clientSocket.Send(info_a_enviar);
                }

                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (SocketException se)
            {
                Console.WriteLine($"Error de conexión: {se.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Se produjo un error: {ex.Message}");
            }
        }

        private void ReceiveMessages(Socket clientSocket)
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[100];
                    int received = clientSocket.Receive(buffer);

                    if (received == 0)
                    {
                        break;
                    }

                    string data = Encoding.UTF8.GetString(buffer, 0, received);
                    Console.WriteLine($"Mensaje recibido: {data}");
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
    }
}
