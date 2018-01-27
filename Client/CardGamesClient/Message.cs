using ProtoBuf;

namespace CardGamesClient
{
    [ProtoContract]
    public class Message
    {
        public enum Steps
        {
            LOBBY,
            GAME,
            DISTRIBUTION,
            BIDDING,
            TURN,
            ERROR
        }

        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public int Code { get; set; }

        [ProtoMember(3)]
        public Steps Step { get; set; }

        [ProtoMember(4)]
        public int Points { get; set; }

        [ProtoMember(5)]
        public Card.Number CardNb;

        [ProtoMember(6)]
        public Card.Color CardColor;

        [ProtoMember(7)]
        public int Owner;

        [ProtoMember(8)]
        public int BiddingNb;

        [ProtoMember(9)]
        public Bidding.Color BiddingColor;

        [ProtoMember(10)]
        public int BiddingSkip;

        [ProtoMember(11)]
        public int BiddingCoinche;

        [ProtoMember(12)]
        public int BiddingTeam;

        //[ProtoMember(6)]
        public Bidding Bidding { get; set; }

        public Message()
        {

        }



        //public Message(int id, Game.Step step, Card card, Bidding bidding, int points, int code)
        //{
        //    this.Id = id;
        //    this.Step = step;
        //    this.Card = card;
        //    this.Bidding = bidding;
        //    this.Points = points;
        //    this.Code = code;
        //}
    }
}
