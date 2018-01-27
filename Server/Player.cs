using NetworkCommsDotNet.Connections;
using System.Collections.Generic;

namespace CardGamesServer
{
    public class Player
    {
        public Connection Conn { get; }
        public int Id { get; }
        public int Team { get; }
        public int TotalPoints { get; set; }
        public int TurnPoints { get; set; }
        public List<Card> Hand { get; }

        public Player(Connection connection, int id)
        {
            Team = (id % 2) + 1;
            Id = id;
            Conn = connection;
            TotalPoints = 0;
            TurnPoints = 0;
            Hand = new List<Card>();
        }
        
        public void Distribute(Card card)
        {
            Message msg = new Message
            {
                Id = this.Id,

                CardNb = card.GetNumber(),
                CardColor = card.GetColor(),
                CardOwner = Id,
                Step = Message.Steps.DISTRIBUTION
            };

            //msg.Card.Display();
            Conn.SendObject("CardGame", msg/*, Server.customSendReceiveOptions*/);
            Hand.Add(card);
        }

        public void CardPlayed(Card card)
        {
            //Message msg = new Message
            //{
            //    Id = this.Id,
            //    Card = card,
            //    Step = Message.Steps.TURN,
            //    Code = 1
            //};

            //Conn.SendObject("CardGame", msg, Server.customSendReceiveOptions);
            Hand.Remove(card);
        }
    }
}
