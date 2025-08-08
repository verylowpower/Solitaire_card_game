using UnityEngine;
using System.Collections.Generic;
using Core;
using Undo;

namespace Managers
{
    public class UndoManager : MonoBehaviour
    {
        public static UndoManager Instance;
        public Stack<IUndoAction> _undoStack = new();

        public Transform stockPileTransform;
        public Transform wastePileTransform;

        private void Awake()
        {
            Instance = this;
        }

        public void RecordMove(Transform[] cards, Transform fromParent, Transform toParent, Vector3[] originalPositions)
        {
            bool[] faceUp = new bool[cards.Length];
            int[] siblingIndexes = new int[cards.Length];
            int[] sortingOrders = new int[cards.Length];

            for (int i = 0; i < cards.Length; i++)
            {
                var card = cards[i].GetComponent<Card>();
                faceUp[i] = card.IsFaceUp;
                siblingIndexes[i] = cards[i].GetSiblingIndex();
                sortingOrders[i] = card.GetComponent<SpriteRenderer>().sortingOrder;
            }

            Transform revealedCard = null;
            bool revealedCardFaceUp = false;
            int revealedCardSortingOrder = 0;
            int revealedCardSiblingIndex = 0;

            int draggedStartIndex = cards[0].GetSiblingIndex();
            if (fromParent.childCount > draggedStartIndex)
            {
                int revealIndex = draggedStartIndex - 1;
                if (revealIndex >= 0)
                {
                    revealedCard = fromParent.GetChild(revealIndex);
                    var rc = revealedCard.GetComponent<Card>();
                    if (rc != null)
                    {
                        revealedCardFaceUp = rc.IsFaceUp;
                        revealedCardSortingOrder = rc.GetComponent<SpriteRenderer>().sortingOrder;
                        revealedCardSiblingIndex = revealedCard.GetSiblingIndex();
                    }
                }
            }

            _undoStack.Push(new UndoActionMove(
                cards,
                fromParent,
                originalPositions,
                faceUp,
                siblingIndexes,
                sortingOrders,
                revealedCard,
                revealedCardFaceUp,
                revealedCardSortingOrder,
                revealedCardSiblingIndex));
        }

        public void RecordFlipStock(Card card)
        {
            _undoStack.Push(new UndoActionFlipStock(card, stockPileTransform, wastePileTransform));
        }

        public void Undo()
        {
            if (_undoStack.Count == 0) return;
            _undoStack.Pop().Undo();
        }
    }
}
