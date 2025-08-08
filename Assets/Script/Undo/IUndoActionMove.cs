using UnityEngine;
using Core;

namespace Undo
{
    public class UndoActionMove : IUndoAction
    {
        private readonly Transform[] _cards;
        private readonly Transform _fromParent;
        private readonly Vector3[] _originalPositions;
        private readonly bool[] _originalFaceUp;
        private readonly int[] _originalSiblingIndexes;
        private readonly int[] _originalSortingOrders;

        private readonly Transform _revealedCard;
        private readonly bool _revealedCardFaceUp;
        private readonly int _revealedCardSortingOrder;
        private readonly int _revealedCardSiblingIndex;

        public UndoActionMove(
            Transform[] cards,
            Transform fromParent,
            Vector3[] originalPositions,
            bool[] originalFaceUp,
            int[] originalSiblingIndexes,
            int[] originalSortingOrders,
            Transform revealedCard,
            bool revealedCardFaceUp,
            int revealedCardSortingOrder,
            int revealedCardSiblingIndex)
        {
            _cards = cards;
            _fromParent = fromParent;
            _originalPositions = originalPositions;
            _originalFaceUp = originalFaceUp;
            _originalSiblingIndexes = originalSiblingIndexes;
            _originalSortingOrders = originalSortingOrders;
            _revealedCard = revealedCard;
            _revealedCardFaceUp = revealedCardFaceUp;
            _revealedCardSortingOrder = revealedCardSortingOrder;
            _revealedCardSiblingIndex = revealedCardSiblingIndex;
        }

        public void Undo()
        {
            for (int i = 0; i < _cards.Length; i++)
            {
                var card = _cards[i];
                card.SetParent(_fromParent, false);
                card.localPosition = _originalPositions[i];
                card.SetSiblingIndex(_originalSiblingIndexes[i]);

                var cardComp = card.GetComponent<Card>();
                cardComp.SetFaceUp(_originalFaceUp[i]);

                var sr = card.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sortingOrder = _originalSortingOrders[i];
            }

            if (_revealedCard != null)
            {
                var rc = _revealedCard.GetComponent<Card>();
                rc.SetFaceUp(_revealedCardFaceUp);
                rc.transform.SetSiblingIndex(_revealedCardSiblingIndex);
                var sr = rc.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sortingOrder = _revealedCardSortingOrder;
            }
        }
    }
}
