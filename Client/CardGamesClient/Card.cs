using ProtoBuf;

namespace CardGamesClient
{
    [ProtoContract]
    public class Card
    {
        public enum Color
        {
            SPADE,
            HEART,
            CLUB,
            DIAMOND
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
            ACE
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
        public int owner { get; }

        public int GetOwner()
        {
            return owner;
        }

        public Color GetColor()
        {
            return color;
        }

        public Number GetNumber()
        {
            return nb;
        }

        public Card(Number number, Color colour, int id)
        {
            nb = number;
            color = colour;
            owner = id;
        }
    }
}
