using System.Data;
using UnityEngine;

public class RuleManager : MonoBehaviour
{
    public static RuleManager instance;

    void Awake() => instance = this;

    public bool IsValidTableauDrop(Card currentCard, Card cardTarget)
    {
      
        if (cardTarget == null)
            return currentCard.cardValue == Card.CardValue.King;

        if (!cardTarget.isFaceUp) return false;

        bool isOppositeColor = IsOppositeColor(currentCard.suit, cardTarget.suit);
        bool isOneLess = currentCard.cardValue == cardTarget.cardValue - 1;

        return isOppositeColor && isOneLess;
    }


    public bool IsValidFoundationDrop(Card currentCard, Card topFoundationCard)
    {
        if (topFoundationCard == null)
        {
            return (int)currentCard.cardValue == 1;
        }

        bool sameSuit = currentCard.suit == topFoundationCard.suit;

        bool oneHigher = currentCard.cardValue == topFoundationCard.cardValue + 1;

        return sameSuit && oneHigher;
    }

    private bool IsOppositeColor(Card.Suit a, Card.Suit b)
    {
        return (isRed(a) && isBlack(b)) || (isBlack(a) && isRed(b));
    }

    private bool isRed(Card.Suit type) => type == Card.Suit.Heart || type == Card.Suit.Diamond;
    private bool isBlack(Card.Suit type) => type == Card.Suit.Spade || type == Card.Suit.Club;
}
