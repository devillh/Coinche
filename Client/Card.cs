using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGamesClient
{
    class Card
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

        private Number _nb;
        private Color _color;

        public Card(Number nb, Color color)
        {
            _nb = nb;
            _color = color;
        }

        public Number getNumber()
        {
            return _nb;
        }

        public Color getColor()
        {
            return _color;
        }
    }
}
