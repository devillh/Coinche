using ProtoBuf;
using System.Collections.Generic;

namespace CardGamesServer
{
    [ProtoContract]
    public class Bidding
    {
        public enum Color
        {
            SPADE,
            HEART,
            CLUB,
            DIAMOND,
            TRUMPS,
            NOTRUMPS,
            UNDEFINED
        }

        [ProtoMember(1)]
        public int value;
        [ProtoMember(2)]
        public Color color;
        [ProtoMember(3)]
        public int coinche;
        [ProtoMember(4)]
        public int skip;
        [ProtoMember(5)]
        public int team;

        public Bidding()
        {
            value = 79;
            Reset();
        }

        public int GetSkipValue()
        {
            return skip;
        }

        public int GetValue()
        {
            return value;
        }

        public Color GetColor()
        {
            return color;
        }

        public void Reset()
        {
            value = 79;
            color = Color.UNDEFINED;
            team = 0;
            coinche = 0;
            skip = 0;
        }

        public bool SetBidding(int val, Color col, int skp, int coincheVal, int teamPlayers)
        {
            if ((val != 0 && val < value) && teamPlayers != 1 && teamPlayers != 2)
                return false;
            else if (skp != 0)
            {
                skip += 1;
                if (coinche > 1 && skp != 0)
                    skip = 5;
                return (true);
            }
            else if (coincheVal != 0)
            {
                coinche += 2;
                skip = 0;
                return (true);
            }

            skip += 1;
            if (coinche > 1 && skp != 0)
                skip = 5;
            value = val;
            color = col;
            team = teamPlayers;
            return true;
        }

        public bool IsEnd()
        {
            System.Console.WriteLine("skip : " + skip + " coinche " + coinche + " val " + value);
            if (skip >= Server.game.MaxPlayer || (coinche > 1 && skip != 0) || coinche > 3)
                return (true);
            return (false);
        }

        public int GetCoinche()
        {
            return coinche;
        }

        public int GetTeam()
        {
            return team;
        }
    }
}
