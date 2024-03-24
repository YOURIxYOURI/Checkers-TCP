using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;

namespace Checkers_server
{
    internal class TcpServer
    {
        private TcpListener listener;
        private bool isRunning = true;
        private TcpClient player1;
        private TcpClient player2;
        private NetworkStream player1Stream;
        private NetworkStream player2Stream;

        public TcpServer(string ipAddress, int port)
        {
            IPAddress address = IPAddress.Parse(ipAddress);
            listener = new TcpListener(address, port);
        }

        public async void Start()
        {
            listener.Start();
            Console.WriteLine("Server started. Waiting for connections...");

            while (isRunning)
            {
                if (player1 == null)
                {
                    player1 = listener.AcceptTcpClient();
                    Console.WriteLine("Player 1 connected.");
                    player1Stream = player1.GetStream();
                }
                else if (player2 == null)
                {
                    player2 = listener.AcceptTcpClient();
                    Console.WriteLine("Player 2 connected.");
                    player2Stream = player2.GetStream();
                }

                if (player1 != null && player2 != null)
                {
                    Thread gameThread = new Thread(new ThreadStart(StartGame));
                    gameThread.Start();
                    break;
                }
            }
        }

        private void StartGame()
        {
            Console.WriteLine("Game started.");
            CheckerGame game = new CheckerGame();

            //string player1Message = "You are player 1.";
            //string player2Message = "You are player 2.";

            //byte[] player1MessageBytes = Encoding.ASCII.GetBytes(player1Message);
            //byte[] player2MessageBytes = Encoding.ASCII.GetBytes(player2Message);

            //player1Stream.Write(player1MessageBytes, 0, player1MessageBytes.Length);
            //player2Stream.Write(player2MessageBytes, 0, player2MessageBytes.Length);

            while (true)
            {
                string updatedBoard = game.GetBoardState();
                byte[] response = Encoding.ASCII.GetBytes(updatedBoard);
                player1Stream.Write(response, 0, response.Length);
                player2Stream.Write(response, 0, response.Length);
                if (game.IsPlayer1Turn())
                {
                    string data = ReadMove(player1Stream);
                    if (data != null) 
                    {
                        Console.WriteLine("Received: " + data);

                        if (game.MakeMove(data, 'x'))
                        {
                            Console.WriteLine("Gracz 1 wykonał ruch");
                        }
                    }
                }
                else
                {
                    string data = ReadMove(player2Stream);
                    if (data != null)
                    {
                        Console.WriteLine("Received: " + data);

                        if (game.MakeMove(data, 'o'))
                        {
                            Console.WriteLine("Gracz 2 wykonał ruch");
                        }
                    }
                }
            }
        }
        private string ReadMove(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }
        public void Stop()
        {
            isRunning = false;
            listener.Stop();
        }
    }
}
