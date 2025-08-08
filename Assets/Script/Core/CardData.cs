using UnityEngine;

namespace Core
{
    [System.Serializable]
    public class CardData
    {
        public Card.Suit suit;
        public Card.CardValue value;
        public Sprite frontSprite;
        public Sprite backSprite;
    }
}