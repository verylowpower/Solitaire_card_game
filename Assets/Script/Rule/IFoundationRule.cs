namespace Rules
{
    using Core;

    public class FoundationRule : IGameRule
    {
        public bool IsValidDrop(Card currentCard, Card targetCard)
        {
            if (currentCard == null) return false;

           
            if (targetCard == null)
                return currentCard.Value == Card.CardValue.Ace;

            bool sameSuit = currentCard.CardSuit == targetCard.CardSuit;
            bool oneHigher = (int)currentCard.Value == (int)targetCard.Value + 1;

            return sameSuit && oneHigher;
        }
    }
}
