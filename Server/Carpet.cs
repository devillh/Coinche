using System.Collections.Generic;

namespace CardGamesServer
{
    public class Carpet
    {
        private List<Card> cards = new List<Card>();
        private Bidding bidding = new Bidding();
        public int winner;
        public int totalPoints;

        public Carpet()
        {
            Reset();
        }

        public void Clear()
        {
            Reset();
            cards.Clear();
        }

        public void Reset()
        {
            bidding.Reset();
            winner = 0;
        }

        public void AddCard(Card card)
        {
            cards.Add(card);
        }

        public void SetBidding(Bidding bid)
        {
            bidding = bid;
        }

        public void CountPoints()
        {
            Card card = cards[0];

            for (var i = 0; i < cards.Count; i++)
            {
                if (bidding.GetColor() == Bidding.Color.TRUMPS)
                {
                    totalPoints += cards[i].GetTrumpValue();
                    if (card.GetTrumpValue() < cards[i].GetTrumpValue())
                        card = cards[i];
                }
                else
                {
                    totalPoints += cards[i].GetValue();
                    if (card.GetValue() < cards[i].GetValue())
                        card = cards[i];
                }
            }
            winner = card.GetOwner();
        }

        public bool Turn()
        {
            for (int turns = 0; turns < 8; turns++)
            {
                cards.Clear();
                while (cards.Count != 4)
                {

                    //reception des cartes + ajout dans la List
                    //if déconnexion ->
                 return false;
                }
                CountPoints();
                // envoyer les points aux joueurs winner + coéquipier
                // envoyer 0 points aux autres
            }
            return true;
        }
        public void BiddingSuccess(List<Player> players)
        {
            int bestScore = 0;

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Team == bidding.GetTeam()
                    && players[i].TotalPoints >= bestScore)
                {
                    bestScore = players[i].TotalPoints;
                    winner = players[i].Team;
                }
            }
            if (bidding.GetCoinche() > 0)
                bestScore = bestScore * bidding.GetCoinche();
            if (bestScore >= bidding.GetValue())
            {
                // envoyer les points à l'équipe winner
                // envoyer 0 aux autres
            }
            else
            {
                // envoyer les points à l'autre équipe
                // envoyer 0 à l'équipe des autres

            }
        }
    }
}
