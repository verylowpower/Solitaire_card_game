using UnityEngine;

namespace Core
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
    public class Card : MonoBehaviour, ICard, IFlippable
    {
        public enum Suit { Spade, Club, Heart, Diamond }
        public enum CardValue { Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King }

        public Suit CardSuit { get; private set; }
        public CardValue Value { get; private set; }

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite frontFace;
        [SerializeField] private Sprite backFace;

        public bool IsFaceUp { get; set; }
        public bool IsTopCard { get; set; }
        public bool WasMovedFromWaste { get; set; }

        private void Reset()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetSuitAndValue(Suit suit, CardValue value)
        {
            CardSuit = suit;
            Value = value;
        }

        public void SetSprites(Sprite front, Sprite back)
        {
            frontFace = front;
            backFace = back;
        }

        public void SetFaceUp(bool isFaceUp)
        {
            IsFaceUp = isFaceUp;
            spriteRenderer.sprite = isFaceUp ? frontFace : backFace;
        }
    }
}