using ProtoBuf;

namespace CardGamesClient
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
        private int value;
        [ProtoMember(2)]
        private Color color;
        [ProtoMember(3)]
        private int coinche;
        [ProtoMember(4)]
        private int skip;
        [ProtoMember(5)]
        private int team;

        public Bidding()
        {
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
            value = 80;
            color = Color.UNDEFINED;
            team = 0;
            coinche = 0;
            skip = 0;
        }

        public bool SetBidding(int val, Color col)
        {
            if (val < value)
                return false;
            color = col;
            return true;
        }

        public void SetBidding(bool skp)
        {
            skip += 1;
        }

        public void SetBidding(bool coincheValue, int teamPlayer)
        {
            if (coincheValue)
                coinche += 2;
            team = teamPlayer;
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
