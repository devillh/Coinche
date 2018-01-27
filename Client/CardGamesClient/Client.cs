using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using NetworkCommsDotNet.DPSBase;
using System;
using System.Collections.Generic;

namespace CardGamesClient
{
    
    class Client
    {
        static private Game game = new Game();


        private static Dictionary<Message.Steps, Action<Game, Connection, Message>> commands = new Dictionary<Message.Steps, Action<Game, Connection, Message>>()
        {
            { Message.Steps.LOBBY, Commands.Lobby },
            { Message.Steps.DISTRIBUTION, Commands.Distribution },
            { Message.Steps.GAME, Commands.Game },
            { Message.Steps.BIDDING, Commands.Bidding },
            { Message.Steps.TURN, Commands.Turn },
            { Message.Steps.ERROR, Commands.Error }
        };


        public static SendReceiveOptions customSendReceiveOptions = new SendReceiveOptions<ProtobufSerializer, QuickLZCompressor.QuickLZ>();

        static void Main(string[] args)
        {

            try
            {
                
                string serverIP;
                string input;
                int serverPort;

                System.Console.WriteLine("Please enter the server IP and press return.");
                input = System.Console.ReadLine();
                serverIP = input;

                //serverIP = "127.0.0.1";
                System.Console.WriteLine("Please enter the port and press return.");
                input = System.Console.ReadLine();
                serverPort = int.Parse(input);
                //serverPort = 8000;
                System.Console.WriteLine("Connection to {0}:{1}...", serverIP, serverPort);

                
                ConnectionInfo server = new ConnectionInfo(serverIP, serverPort);
                Connection conn = TCPConnection.GetConnection(server, customSendReceiveOptions);
                
                conn.AppendIncomingPacketHandler<Message>("CardGame", HandleGame, customSendReceiveOptions);
                conn.AppendIncomingPacketHandler<string>("Chat", HandleChat, customSendReceiveOptions);
                
                game.Run(conn);
                    //conn.SendObject("CardGame", message);
                    //NetworkComms.SendObject("CardGame", serverIP, serverPort, message);
                    //System.Console.WriteLine("Message sent " + message.Id);
                    //if (message.Id != Message.Step.ERROR)
                    //    message.Id += 1;
                    //System.Console.WriteLine("Press q to quit or any other key to send another message.");
                    //if (System.Console.ReadKey(true).Key == System.ConsoleKey.Q)
                    //    state = false;
                
                NetworkComms.Shutdown();
            }
            catch (System.Exception error)
            {
                System.Console.Error.WriteLine(error.GetBaseException());
            }
         }

        private static void HandleChat(PacketHeader header, Connection connection, string message)
        {
            System.Console.WriteLine(message);
        }

        private static void HandleGame(PacketHeader header, Connection connection, Message message)
        {
            if (commands.ContainsKey(message.Step))
            {
//                System.Console.WriteLine("A message was received " + message.Id + "step " + message.Step + "code " + message.Code);
                commands[message.Step](game, connection, message);
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
    }
}