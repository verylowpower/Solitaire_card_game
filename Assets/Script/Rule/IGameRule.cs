namespace Rules
{
    using Core;

    public interface IGameRule
    {
        bool IsValidDrop(Card currentCard, Card targetCard);
    }
}
