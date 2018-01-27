using NetworkCommsDotNet.Connections;
using System;

namespace CardGamesClient
{
    class Commands
    {
        public static void Distribution(Game game, Connection conn, Message message)
        {
            Console.WriteLine("You received " + message.CardNb + " of " + message.CardColor);
            Card card = new Card(message.CardNb, message.CardColor, game.Id);
            game.MakeHand(card);
        }

        public static void Lobby(Game game, Connection conn, Message message)
        {
            if (message.Code == 0)
            {
                game.SetGame(message.Id);
                System.Console.WriteLine("Welcome to Coinche Game !\nPlease join a game with /join or see usage with /usage");
            }
            else
            {
                System.Console.WriteLine("Welcome back to the Lobby!");

            }
        }
        public static void Game(Game game, Connection conn, Message message)
        {
            switch (message.Code)
            {
                case 0:
                    System.Console.WriteLine("You joined the game !");
                    game.Id = message.Id;
                    break;

                case 1:
                    System.Console.WriteLine("The player #" + message.Id + " joined the game !");
                    break;

                case 2:
                    System.Console.WriteLine("Game Start !");
                    break;

                case 3:
                    if (message.Id == 0)
                    {
                        System.Console.WriteLine("Game Over ! It's a tie with " + message.Points + " points");
                    }
                    else if (message.Id == (((game.Id + 1)% 2) + 1))
                        System.Console.WriteLine("Game Over ! Your team win with " + message.Points + " !");
                    else
                        System.Console.WriteLine("Game Over ! Your team lose ! The other team won with " + message.Points);
                    break;
                case 4:
                    if (message.Id == game.Id)
                        System.Console.WriteLine("It's your turn !");
                    else
                        System.Console.WriteLine("It's the turn of Player #" + message.Id + " to play");
                    break;
                default:
                    break;
            }
        }
        public static void Bidding(Game game, Connection conn, Message message)
        {
            switch (message.Code)
            {
                case 0:
                    System.Console.WriteLine("Start of Bidding !");
                    break;
                case 1:
                    if (message.BiddingSkip != 0)
                        System.Console.WriteLine("Player #" + message.Id + " skiped");
                    else if (message.BiddingCoinche != 0)
                    {
                        System.Console.WriteLine("Player #" + message.Id + " has " + (message.BiddingCoinche > 3 ? "re" : "") + "coinche");
                    }
                    else
                    {
                        game.bidding.SetBidding(message.BiddingNb, message.BiddingColor, message.BiddingSkip, message.BiddingCoinche, message.BiddingTeam);
                        game.DisplayBidding();
                    }
                    break;
                case 2:
                    System.Console.WriteLine("End of Bidding !");
                    break;
            }
        }

        public static void Turn(Game game, Connection conn, Message message)
        {
            switch (message.Code)
            {
                case 0:
                    System.Console.WriteLine("First round, let's play!");
                    break;
                case 1:
                    if (message.Owner != game.Id)
                        System.Console.WriteLine("Player #" + message.Id + " has played " + message.CardNb + " of " + message.CardColor + ".");
                    else
                        System.Console.WriteLine("You played " + message.CardNb + " of " + message.CardColor + ".");
                    break;
                case 2:
                    if (message.Points != 0)
                        System.Console.WriteLine("You won this turn with" + message.Points + " points!");
                    else
                        System.Console.WriteLine("You loose this turn...");
                    break;
                case 3:
                    if (message.Id == ((game.Id % 2) + 1))
                        System.Console.WriteLine("You won this round with" + message.Points + " points!");
                    else
                        System.Console.WriteLine("You loose this round...");
                    break;
                default:
                    break;
            }
        }

            public static void Error(Game game, Connection conn, Message message)
        {
            switch (message.Code)
            {
                case 100:
                    System.Console.WriteLine("Command unknown");
                    break;
                case 101:
                    System.Console.WriteLine("Can't join the game");
                    break;
                case 102:
                    System.Console.WriteLine("A player has disconnected");
                    break;
                case 103:
                    System.Console.WriteLine("Invalid command");
                    break;
                case 104:
                    System.Console.WriteLine("You are already in game !");
                    break;

                case 105:
                    System.Console.WriteLine("It's not your turn !");
                    break;

                default:
                    System.Console.WriteLine("Unknown error");
                    break;
            }
        }
    }
}
