namespace Core
{
    public interface ICard
    {
        Card.Suit CardSuit { get; }
        Card.CardValue Value { get; }
        bool IsFaceUp { get; set; }
    }
}