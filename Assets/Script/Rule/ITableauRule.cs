namespace Rules
{
    using Core;

    public class TableauRule : IGameRule
    {
        public bool IsValidDrop(Card currentCard, Card targetCard)
        {
            if (currentCard == null) return false;

            if (targetCard == null)
                return currentCard.Value == Card.CardValue.King;

            if (!targetCard.IsFaceUp) return false;

            bool isOppositeColor = IsOppositeColor(currentCard.CardSuit, targetCard.CardSuit);
            bool isOneLess = (int)currentCard.Value == (int)targetCard.Value - 1;

            return isOppositeColor && isOneLess;
        }

        private bool IsOppositeColor(Card.Suit a, Card.Suit b)
        {
            return (IsRed(a) && IsBlack(b)) || (IsBlack(a) && IsRed(b));
        }

        private bool IsRed(Card.Suit suit) =>
            suit == Card.Suit.Heart || suit == Card.Suit.Diamond;

        private bool IsBlack(Card.Suit suit) =>
            suit == Card.Suit.Spade || suit == Card.Suit.Club;
    }
}
