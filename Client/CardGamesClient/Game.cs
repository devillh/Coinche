using System.Collections.Generic;
using NetworkCommsDotNet.Connections;
using System;
using System.Linq;

namespace CardGamesClient
{
    class Game
    {
        private static Dictionary<string, Action<Game, List<string>>> commands = new Dictionary<string, Action<Game, List<string>>>()
        {
            { "/join" , Join},
            {"/play", PlayCard},
            {"/cards", DisplayHand},
            {"/coinche", Coinche},
            {"/surcoinche", Coinche},
            {"/skip", SkipTurn},
            {"/bidding", Bid},
            {"/points", ShowPoints}
        };
        public Bidding bidding = new Bidding();
        private List<Card> Hand = new List<Card>();
        public int Points { get; set; }
        public int Id { get; set; }
        private Connection conn;

        public Game()
        {
            Points = 0;
        }

        public void MakeHand(Card card)
        {
            Hand.Add(card);
        }

        static void DisplayHand(Game game, List<string> show)
        {
            for (int i = 0; i < game.Hand.Count; i++)
            {
                Console.WriteLine("[" + i + "] : " + game.Hand[i].GetNumber().ToString() + " of " + game.Hand[i].GetColor().ToString());
            }
        }

        static public void ShowPoints(Game game, List<string> show)
        {
            System.Console.WriteLine("You have " + game.Points + "!");
        }

        static public void SendData(Connection conn, Message msg)
        {
            conn.SendObject("CardGame", msg, Client.customSendReceiveOptions);
        }

        static public void Join(Game game, List<string> join)
        {
            Message msg = new Message()
            {
                Step = Message.Steps.GAME,
                Code = 1
            };
            SendData(game.conn, msg);
        }

        public void SetGame(int player)
        {
            Id = player;
        }

        public void Run(Connection connect)
        {
            string input;
            var quit = false;
            conn = connect;
            while (!quit)
            {
                input = System.Console.ReadLine().Trim();
                if (input.Length > 0)
                {
                    if (input == "/quit")
                        quit = true;
                    else if (input == "/usage")
                        System.Console.WriteLine("/join: join a new game\n/play [nb]: play the card by selecting its number\n" +
                            "/cards: show all your cards\n/bidding [nb] [color]: bid a number of points to reach and a color\n" +
                            "/coinche: bid your ennemies won't fit their bidding\n/surcoinche: bid you'll succeed\n/skip: pass your turn");
                    else if (input[0] == '/')
                    {
                        CommandsManager(input);
                    }
                    else
                    {
                        conn.SendObject("Chat", input, Client.customSendReceiveOptions);
                    }
                }
            }
        }

        public void DisplayBidding()
        {
            if (bidding.GetColor() != Bidding.Color.UNDEFINED)
            {
                System.Console.WriteLine("Team #" + bidding.GetTeam() + " made the following bidding :"
                    + bidding.GetValue() + " of " + bidding.GetColor());
                if (bidding.GetCoinche() == 2)
                    System.Console.WriteLine("Team #" + bidding.GetTeam() + " has coinche. Write /surcoinche to surcoinche or /skip to skip your turn");
                else
                    System.Console.WriteLine("Do you want to /skip, /bidding [nb] [color] or /coinche?");
            }
        }

        static void Coinche(Game game, List<string> coinche)
        {
            game.bidding.SetBidding(true, ((game.Id + 1) % 2) + 1);
            Message msg = new Message()
            {
                Id = game.Id,
                Step = Message.Steps.BIDDING,
                BiddingCoinche = 1,
                BiddingTeam = (((game.Id + 1) % 2) + 1),
                Code = 0
            };
            SendData(game.conn, msg);
        }

        static void SkipTurn(Game game, List<string> skip)
        {
            game.bidding.SetBidding(true);
            Message msg = new Message()
            {
                Id = game.Id,
                Step = Message.Steps.BIDDING,
                BiddingSkip = 1,
                BiddingTeam = (((game.Id + 1) % 2) + 1),
                Code = 0
            };
            SendData(game.conn, msg);
        }

        static void Bid(Game game, List<string> values)
        {
            if (values.Count != 3)
            {
                System.Console.Error.WriteLine("Bad number of parameters. Use /bidding [number] [color]");
                return;
            }
            try
            {
                int nb = int.Parse(values[1]);

                if (nb % 10 != 0 || nb < game.bidding.GetValue())
                {
                    System.Console.Error.WriteLine("Invalid value ! You must choose a number multiple of ten and superior of the current bidding " + game.bidding.GetCoinche());
                    return;
                }
                else if (!Bidding.Color.IsDefined(typeof(Bidding.Color), values[2]) || (Bidding.Color)Enum.Parse(typeof(Bidding.Color), values[2]) == Bidding.Color.UNDEFINED)
                {
                    System.Console.Error.WriteLine("Invalid Color ! You must choose amongst these colors :");
                    foreach (Bidding.Color col in Enum.GetValues(typeof(Bidding.Color)))
                    {
                        if (col != Bidding.Color.UNDEFINED)
                            System.Console.Error.WriteLine("-" + col);
                    }
                    return;
                }
                Bidding.Color color = (Bidding.Color)Enum.Parse(typeof(Bidding.Color), values[2]);
                game.bidding.SetBidding(nb, color);
                Message msg = new Message()
                {
                    Step = Message.Steps.BIDDING,
                    Id = game.Id,
                    BiddingColor = color,
                    BiddingNb = nb,
                    BiddingTeam = (((game.Id + 1) % 2) + 1),
                    Code = 0
                };
                SendData(game.conn, msg);
            }
            catch
            {
                System.Console.Error.WriteLine("Bad parameters. Use /bidding [number] [color]");
            }
        }

        public bool CommandsManager(string input)
        {
            List<string> list;
            list = input.Split(' ').ToList();
            if (commands.ContainsKey(list[0]))
                commands[list[0]](this, list);
            else
                System.Console.Error.WriteLine("Invalid Command");
            return true;
        }

        static public void PlayCard(Game game, List<string> input)
        {
            if (input.Count != 2)
            {
                System.Console.WriteLine("Bad number of parameters. Use /play [number]");
                return;
            }
            try
            {
                int idx = int.Parse(input[1]);
                if (idx < 0 || idx >= game.Hand.Count)
                {
                    Console.Error.WriteLine("Wrong parameter, select a number between 0 and 8");
                    return;
                }
                Card card = game.Hand[idx];
                game.Hand.Remove(card);
                Message msg = new Message()
                {
                    Id = game.Id,
                    Step = Message.Steps.TURN,
                    CardColor = card.GetColor(),
                    CardNb = card.GetNumber(),
                    Owner = game.Id,
                    Code = 0
                };
                SendData(game.conn, msg);
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Wrong parameter, select a number between 0 and 8");
            }
        }
    }
}
