using System.Collections.Generic;
using ProtoBuf;

namespace CardGamesServer
{
    [ProtoContract]
    public class Card
    {
        public enum Color
        {
            SPADE,
            HEART,
            CLUB,
            DIAMOND,
            UNDEFINED
        }
        public enum Number
        {
            SEVEN,
            EIGHT,
            NINE,
            TEN,
            JACK,
            QUEEN,
            KING,
            ACE,
            UNDEFINED
        }

        [ProtoMember(1)]
        public Number nb;
        [ProtoMember(2)]
        public Color color;
        [ProtoMember(3)]
        public int value;
        [ProtoMember(4)]
        public int trumpValue;
        [ProtoMember(5)]
        public int Owner;
        private Dictionary<Number, int> cardsValues = new Dictionary<Number, int>()
        {
            {Number.SEVEN, 0},
            {Number.EIGHT, 0},
            {Number.NINE, 0},
            {Number.TEN, 10},
            {Number.JACK, 2},
            {Number.QUEEN, 3},
            {Number.KING, 4},
            {Number.ACE, 11}
        };

        private Dictionary<Number, int> cardsTrumpValues = new Dictionary<Number, int>()
        {
            {Number.SEVEN, 0},
            {Number.EIGHT, 0},
            {Number.NINE, 14},
            {Number.TEN, 10},
            {Number.JACK, 20},
            {Number.QUEEN, 3},
            {Number.KING, 4},
            {Number.ACE, 11}
        };
        

        protected Card() { }

        public Card(Number nbCard, Color colorCard, int ownerCard = 0)
        {
            nb = nbCard;
            Owner = ownerCard;
            color = colorCard;
            cardsValues.TryGetValue(nb, out value);
            cardsTrumpValues.TryGetValue(nb, out trumpValue);
        }

        public int GetOwner()
        {
            return Owner;
        }

        public Number GetNumber()
        {
            return nb;
        }

        public Color GetColor()
        {
            return color;
        }
        
        public int  GetValue()
        {
            return value;
        }

        public int GetTrumpValue()
        {
            return trumpValue;
        }

        public void Display()
        {
            System.Console.WriteLine("Card : " + nb + " of " + color);
        }
    }
}