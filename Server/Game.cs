using System.Collections.Generic;
using System;
using NetworkCommsDotNet.Connections;

namespace CardGamesServer
{
    public class Game
    {
        public List<Card> Deck { get; }
        public List<Player> Players { get; }
        public Bidding Bidding;
        public Carpet Carpet;
        public int CurPlayer { get; private set; }
        public Message.Steps CurStep { get; private set; }
        public int Round;
        public int MaxPlayer { get; }

        public int MaxRound { get; }
        public int CountPlay;


        public Game()
        {
            Deck = new List<Card>();
            Players = new List<Player>();
            Bidding = new Bidding();
            Carpet = new Carpet();
            CurPlayer = 0;
            CurStep = Message.Steps.GAME;
            Round = 0;
            MaxPlayer = 4;
            MaxRound = 8;
            CountPlay = 0;
            for (Card.Color j = Card.Color.SPADE; j <= Card.Color.DIAMOND; j++)
                for (Card.Number i = Card.Number.SEVEN; i <= Card.Number.ACE; i++)
                {
                    Card card = new Card(i, j, 0);
                    Deck.Add(card);
                }
            Shuffle();
            Bidding.Reset();
            Carpet.Clear();
        }

        public void Distribute()
        {
            for (int i = 0; i < Players.Count; i++)
                Players[i].Hand.Clear();
            Shuffle();
            foreach (Player player in Players)
            {
                player.Hand.Clear();
            }
            int idx = 0;

            foreach (Player player in Players)
            {
                for (int cnt = 0; cnt < 3; ++cnt)
                {
                    player.Distribute(Deck[idx++]);
                }
            }
            foreach (Player player in Players)
            {
                for (int cnt = 0; cnt < 2; ++cnt)
                {
                    player.Distribute(Deck[idx++]);
                }
            }
            foreach (Player player in Players)
            {
                for (int cnt = 0; cnt < 3; ++cnt)
                {
                    player.Distribute(Deck[idx++]);
                }
            }
            NextStep();

        }


        public Player GetPlayer(Connection conn)
        {
            foreach(Player player in Players)
            {
                if (conn == player.Conn)
                    return (player);
            }
            return (null);
        }
        public Player GetPlayer(int id)
        {
            if (id < 0 || id > Players.Count)
                return (null);
            int cnt = 1;
            foreach (Player player in Players)
            {
                if (cnt == id)
                    return (player);
                cnt += 1;
            }
            return (null);
        }
        public void AskRestartGame()
        {
            for (int i = 0; i < Players.Count; i++)
            {
                // envoie un message pour demander aux joueurs s'ils veulent refaire une partie
            }
            // get les réponses
            // if player dit oui, le laisse
            // if player dit non, coupe la connexion
        }

        public bool Start()
        {
            System.Console.WriteLine("Start game !");
            NextStep();
            Message message = new Message
            {
                Code = 2,
                Step = Message.Steps.GAME
            };
            
            foreach (Player player in Players)
            {
                Commands.SendData(player.Conn, message);
            }

            System.Console.WriteLine("Distribution demarree");
            Distribute();
            System.Console.WriteLine("Distribution finie");
            return true;
        }

        public void SendCurPlayer()
        {
            Message msg = new Message()
            {
                Step = Message.Steps.GAME,
                Code = 4,
                Id = CurPlayer
            };

            foreach (Player player in Players)
            {
                Commands.SendData(player.Conn, msg);
            }
        }

        public int NextPlayer()
        {
            CurPlayer = (CurPlayer % MaxPlayer) + 1;
            SendCurPlayer();
            return (CurPlayer);
        }

        public int GetWinner()
        {
            int id = 0;
            int maxpts = 0;

            foreach (Player player in Players)
            {
                if (player.TotalPoints > maxpts)
                {
                    id = ((player.Id + 1) % 2) + 1;
                    maxpts = player.TotalPoints;
                }
            }
            return (id);
        }

        public int GetWinnersPoints()
        {
            int id = GetWinner();

            if (id == 0)
                return (0);
            return (Players[id - 1].TotalPoints);
        }

        public void SendWinners()
        {
            Message msg = new Message
            {
                Step = Message.Steps.GAME,
                Code = 3,
                Id = GetWinner(),
                Points = GetWinnersPoints()
            };

            Console.WriteLine("Send winner " + msg.Id + " with " + msg.Points + " pts");
            foreach (Player player in Players)
            {
                Commands.SendData(player.Conn, msg);
            }
            Reset();
        }

        public Message.Steps NextStep()
        {
            if (CurStep != Message.Steps.TURN)
            {
                CurStep += 1;
                if (CurStep == Message.Steps.TURN && Bidding.value < 80)
                {
                    CountPlay = 0;
                    NextStep();
                    Carpet.Reset();
                }
                else
                {
                    Message msg = new Message
                    {
                        Step = CurStep,
                        Code = 0
                    };
                    foreach (Player p in Players)
                    {
                        Commands.SendData(p.Conn, msg);
                    }
                    NextPlayer();
                    Carpet.Reset();
                    if (CurStep == Message.Steps.TURN)
                        CountPlay = 0;
                }
            }
            else
            {
                Round += 1;
                CurStep = Message.Steps.DISTRIBUTION;
                if (Round >= MaxRound)
                {
                    SendWinners();
                }
                else
                {
                    Message msg = new Message
                    {
                        Step = CurStep,
                        Code = 0
                    };
                    foreach (Player p in Players)
                    {
                        Commands.SendData(p.Conn, msg);
                    }
                    Distribute();
                    NextPlayer();
                }
            }
            System.Console.WriteLine("step : " + CurStep);
            return (CurStep);
        }
        public bool MakeBiddings()
        {
            bool biddingOver = false;
            Message mesg = new Message()
            {
                Step = Message.Steps.BIDDING
            };

            foreach (Player player in Players)
            {
                Commands.SendData(player.Conn, mesg);
            }


            
            while (!biddingOver)
            {
                // envoyer un message au joueur à qui c'est le tour
                // _bidding = bidding envoyée par le client
                if (Bidding.GetSkipValue() == 4)
                    return false;
                else if ((Bidding.GetSkipValue() == 3 && 
                    Bidding.GetColor() != Bidding.Color.UNDEFINED) ||
                    Bidding.GetCoinche() == 4)
                    biddingOver = true;
                //Bidding.SetBidding(110, Bidding.Color.NOTRUMPS, true, true, 1);
            }
            NextStep();
            return true;
        }

        public void Shuffle()
        {
            Random rand = new Random();
            for (var i = Deck.Count - 1; i > 0; i--)
            {
                var n = rand.Next(i + 1);
                var tmp = Deck[i];
                Deck[i] = Deck[n];
                Deck[n] = tmp;
            }
        }

        public void Reset()
        {
            System.Console.WriteLine("Reset Game!");
            Shuffle();
            Bidding.Reset();
            Carpet.Clear();
            CurStep = Message.Steps.GAME;
            CurPlayer = 0;
            CountPlay = 0;
            Message msg = new Message
            {
                Code = 1,
                Step = Message.Steps.LOBBY
            };
            foreach (Player p in Players)
            {
                Commands.SendData(p.Conn, msg);
            }
            Players.Clear();
            Round = 0;
        }

        public void AddPlayer(Player player)
        {
            Players.Add(player);
        }

        public void Disconnect(Connection conn)
        {
            Player discon = GetPlayer(conn);

            if (discon != null)
            {
                Message err = new Message()
                {
                    Id = discon.Id,
                    Code = 102
                };
                foreach (Player player in Players)
                {
                    if (player != discon)
                    {
                        Commands.SendData(player.Conn, err);
                    }
                }
                Reset();
            }
        }
    }
}
