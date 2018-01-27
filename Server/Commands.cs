using NetworkCommsDotNet.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGamesServer
{
    class Commands
    {
        public Commands()
        {

        }

        public static void SendData(Connection conn, Message msg)
        {
            conn.SendObject("CardGame", msg, Server.customSendReceiveOptions);
        }

        public static void SendError(Connection conn, int error, int id = 0)
        {
            Message msg = new Message
            {
                Code = error,
                Id = id,
                Step = Message.Steps.ERROR
            };

            SendData(conn, msg);
        }


        public static void Game(Game game, Connection conn, Message message)
        {
            switch (message.Code)
            {
                case (1):
                    if (game.GetPlayer(conn) != null)
                    {
                        System.Console.WriteLine("send already in party");
                        SendError(conn, 104);
                    }
                    else if (game.Players.Count < game.MaxPlayer)
                    {
                        Player new_player = new Player(conn, (game.Players.Count + 1));

                        message.Id = new_player.Id;
                        message.Step = Message.Steps.GAME;
                        message.Code = 0;
                        System.Console.WriteLine("send new player " + message.Id + " " + game.Players.Count);
                        SendData(conn, message);
                        message.Code = 1;
                        foreach(Player player in game.Players)
                        {
                            System.Console.WriteLine("prevent new player");
                            SendData(player.Conn, message);
                        }
                        game.Players.Add(new_player);
                        if (game.Players.Count == game.MaxPlayer)
                        {
                            game.Start();
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("send party full");
                        SendError(conn, 101);
                    }
                    break;
                default:
                    SendError(conn, 100);
                    break;
            }
        }
        public static void Bidding(Game game, Connection conn, Message message)
        {
            if (game.GetPlayer(conn).Id != game.CurPlayer)
            {
                System.Console.WriteLine("Wrong Player id " + game.GetPlayer(conn).Id + " != current " + game.CurPlayer);
                SendError(conn, 105);
            }
            else if (message.Code == 0)
            {
                System.Console.WriteLine("Get contract");

                if (((message.BiddingCoinche != 0 && game.Bidding.GetValue() == 79)) || game.Bidding.SetBidding(message.BiddingValue, message.BiddingColor, message.BiddingSkip, message.BiddingCoinche, message.BiddingTeam) == false)
                {
                    System.Console.WriteLine("Invalid Contract");
                    SendError(conn, 103);
                }
                else
                {
                    message.Code = 1;

                    System.Console.WriteLine("ID " + message.Id + " new bidding " + message.BiddingValue + " " + message.BiddingColor + " skip " + message.BiddingSkip + " coinche " + message.BiddingCoinche + " team " + message.BiddingTeam);
                    foreach (Player player in game.Players)
                    {
                        SendData(player.Conn, message);
                    }
                    
                    if (game.Bidding.IsEnd())
                    {
                        System.Console.WriteLine("End of bidding");
                        game.Carpet.SetBidding(game.Bidding);
                        message.Step = Message.Steps.BIDDING;
                        message.Code = 2;
                        foreach (Player player in game.Players)
                        {
                            SendData(player.Conn, message);
                        }
                        game.NextStep();
                    }
                    else
                    {
                        game.NextPlayer();
                    }
                }
            }
            else
            {
                SendError(conn, 103);
            }
        }

        public static void Turn(Game game, Connection conn, Message message)
        {
            System.Console.WriteLine("turn:" + message.Id + "code " + message.Code);
            if (message.Code == 0)
            {
                Card card = new Card(message.CardNb, message.CardColor);

                Player player = game.GetPlayer(conn);

                player.CardPlayed(card);
                
                Message msg = new Message
                {
                    Id = player.Id,
                    Step = Message.Steps.TURN,
                    Code = 1
                };
                foreach (Player pl in game.Players)
                {
                    SendData(pl.Conn, msg);
                }
                card.Owner = player.Id;
                game.Carpet.AddCard(card);
                game.CountPlay += 1;
                
                if (game.CountPlay >= game.MaxPlayer)
                {
                    game.CountPlay = 0;
                    
                    if (player.Hand.Count == 0)
                    {
                        game.Carpet.CountPoints();
                        Player winner = game.GetPlayer(game.Carpet.winner);
                        Player winner2 = game.GetPlayer((game.Carpet.winner + 1) % 2 + 1);

                        winner.TurnPoints += game.Carpet.totalPoints;
                        winner2.TurnPoints += game.Carpet.totalPoints;
                        winner.TotalPoints += winner.TurnPoints;
                        winner2.TotalPoints += winner2.TurnPoints;
                        Message msg1 = new Message
                        {
                            Step = Message.Steps.TURN,
                            Code = 3,
                            Points = winner.TotalPoints
                        };
                        foreach (Player p in game.Players)
                        {
                            SendData(p.Conn, msg1);
                        }
                        
                        game.Carpet.Clear();
                        game.NextStep();
                        game.NextPlayer();
                    }
                    else
                    {
                        game.Carpet.CountPoints();
                        Player winner = game.GetPlayer(game.Carpet.winner);
                        Player winner2 = game.GetPlayer((game.Carpet.winner + 1) % 2 + 1);

                        winner.TurnPoints += game.Carpet.totalPoints;
                        winner2.TurnPoints += game.Carpet.totalPoints;
                        Message msg2 = new Message
                        {
                            Step = Message.Steps.TURN,
                            Code = 2,
                            Points = winner.TurnPoints
                        };
                        foreach (Player p in game.Players)
                        {
                            SendData(p.Conn, msg2);
                        }
                        game.Carpet.Clear();
                        game.NextPlayer();
                    }
                }
            }
            else
            {
                System.Console.WriteLine("player:" + message.Id);
            }
        }

        public static void Error(Game game, Connection conn, Message message)
        {
            switch (message.Code)
            {
                case 100:
                    System.Console.WriteLine("Receive unknown command from player #" + game.GetPlayer(conn).Id);
                    break;

                case 101:
                    System.Console.WriteLine("Player #" + game.GetPlayer(conn).Id + " doesn't have his 8 cards");
                    break;

                default:
                    System.Console.WriteLine("Player #" + game.GetPlayer(conn).Id + " sent unknown error " + message.Code);
                    SendError(conn, 100);
                    break;
            }
        }
    }
}
