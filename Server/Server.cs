using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.DPSBase;
using System;
using System.Collections.Generic;

namespace CardGamesServer
{
    class Server
    {
        private static List<Connection> _clients = new List<Connection>{};
        public static SendReceiveOptions customSendReceiveOptions = new SendReceiveOptions<ProtobufSerializer, QuickLZCompressor.QuickLZ>();
        public static Game game = new Game();
        private static Dictionary<Message.Steps, Action<Game, Connection, Message>> response = new Dictionary<Message.Steps, Action<Game, Connection, Message>>()
        {
            { Message.Steps.GAME, Commands.Game },
            { Message.Steps.BIDDING, Commands.Bidding },
            { Message.Steps.TURN, Commands.Turn },
            { Message.Steps.ERROR, Commands.Error }
        };


        private static void read_line()
        {
            string input;
            string display = "Type \"/stop\" to stop server or \"/show\" to display the listened ports.";
            var quit = false;

            System.Console.WriteLine(display);
            while (!quit)
            {
                
                input = System.Console.ReadLine().Trim().ToLower();
                if (input.Length > 0)
                {
                    if (input == "/stop")
                        quit = true;
                    else if (input == "/show")
                        ShowConnect();
                    else
                        System.Console.WriteLine(display);
                }
                //conn.SendObject("CardGame", message);
                //NetworkComms.SendObject("CardGame", serverIP, serverPort, message);
                //System.Console.WriteLine("Message sent " + message.Id);
                //if (message.Id != Message.Step.ERROR)
                //    message.Id += 1;
                //System.Console.WriteLine("Press q to quit or any other key to send another message.");
                //if (System.Console.ReadKey(true).Key == System.ConsoleKey.Q)
                //    state = false;
            }
        }

        static void ShowConnect()
        {
            System.Console.WriteLine("Server listening for TCP connection on:");
            foreach (System.Net.IPEndPoint localEndPoint in Connection.ExistingLocalListenEndPoints(ConnectionType.TCP))
                System.Console.WriteLine("{0}:{1}", localEndPoint.Address, localEndPoint.Port);
            System.Console.WriteLine("");
        }

        static void Main(string[] args)
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<Message>("CardGame", HandleGame, customSendReceiveOptions);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Chat", HandleChat, customSendReceiveOptions);
            NetworkComms.AppendGlobalConnectionEstablishHandler(HandleNewConnection);
            NetworkComms.AppendGlobalConnectionCloseHandler(HandleDisconnection);
            
            Connection.StartListening(ConnectionType.TCP, new System.Net.IPEndPoint(System.Net.IPAddress.Any, 8000));

            ShowConnect();
            read_line();

            NetworkComms.Shutdown();
         //   NetworkComms.AppendGlobalIncomingPacketHandler<string>("message", PrintIncomingMessage);
           // Connection.StartListening(ConnectionType.TCP, new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0));
           // System.Console.WriteLine("Server listening for TCP connection on:");
           // foreach (System.Net.IPEndPoint localEndPoint in Connection.ExistingLocalListenEndPoints(ConnectionType.TCP))
           //     System.Console.WriteLine("{0}:{1}", localEndPoint.Address, localEndPoint.Port);
           // System.Console.WriteLine("\nPress any key to close the server.");
           // System.Console.ReadKey();
           // NetworkComms.Shutdown();
        }

        private static void HandleGame(PacketHeader header, Connection connection, Message message)
        {
            System.Console.WriteLine("Data from Client #" + (_clients.IndexOf(connection) + 1) + " : " + message.Step);
            if (message.Step != Message.Steps.ERROR && game.CurStep != message.Step)
            {
                System.Console.WriteLine("Wrong step " + message.Step + " vs " + game.CurStep);
                Commands.SendError(connection, 103);
            }
            else if (response.ContainsKey(message.Step))
            {
                Player player = game.GetPlayer(connection);

                response[message.Step](game, connection, message);
            }
            else
            {
                Message resp = new Message
                {
                    Step = Message.Steps.ERROR,
                    Code = 100
                };
                System.Console.WriteLine("Invalid step : " + message.Step);
                connection.SendObject("CardGame", resp, customSendReceiveOptions);
            }
        }

        private static void HandleChat(PacketHeader header, Connection connection, string message)
        {
            string send_message;
            var nb = _clients.IndexOf(connection) + 1;
            System.Console.WriteLine("Message from #" + nb + " which said '" + message + "'.");

            send_message = "Client #" + nb + ": " + message;
            foreach (Connection client in _clients)
            {
                if (client.ConnectionAlive() && connection != client)
                {
                    client.SendObject("Chat", send_message, customSendReceiveOptions);
                }
            }
        }

        private static void HandleNewConnection(Connection connection)
        {
            System.Console.WriteLine("Client #" + (_clients.Count + 1) + " has connected !");
            _clients.Add(connection);
            Message message = new Message();

            connection.SendObject("CardGame", message, customSendReceiveOptions);
        }
   
        private static void HandleDisconnection(Connection connection)
        {
            var nb = _clients.IndexOf(connection);
            //_clients.Remove(connection);
            System.Console.WriteLine("Client #" + (nb + 1) + " has disconnected.");
            game.Disconnect(connection);
        }

    }
}
