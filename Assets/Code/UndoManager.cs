using UnityEngine;
using System.Collections.Generic;

public class UndoAction
{
    public Transform[] Cards;
    public Transform FromParent;
    public Transform ToParent;
    public Vector3[] OriginalPositions;
    public bool[] OriginalFaceUp;
    public int[] OriginalSiblingIndexes;
    public int[] OriginalSortingOrders;

    //card under
    public Transform RevealedCard;
    public bool RevealedCardFaceUp;
    public int RevealedCardSortingOrder;
    public int RevealedCardSiblingIndex;
}

public class UndoManager : MonoBehaviour
{
    public static UndoManager Instance;
    private Stack<UndoAction> _undoStack = new();

    void Awake()
    {
        Instance = this;
    }

    public void RecordMove(Transform[] cards, Transform fromParent, Transform toParent, Vector3[] originalLocalPositions)
    {
        bool[] faceUp = new bool[cards.Length];
        int[] siblingIndexes = new int[cards.Length];
        int[] sortingOrders = new int[cards.Length];

        for (int i = 0; i < cards.Length; i++)
        {
            var card = cards[i].GetComponent<Card>();
            faceUp[i] = card.isFaceUp;
            siblingIndexes[i] = cards[i].GetSiblingIndex();
            sortingOrders[i] = card.GetComponent<SpriteRenderer>().sortingOrder;
        }

        //Record under card
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
                    revealedCardFaceUp = rc.isFaceUp;
                    revealedCardSortingOrder = rc.GetComponent<SpriteRenderer>().sortingOrder;
                    revealedCardSiblingIndex = revealedCard.GetSiblingIndex();
                }
            }
        }

        _undoStack.Push(new UndoAction
        {
            Cards = cards,
            FromParent = fromParent,
            ToParent = toParent,
            OriginalPositions = originalLocalPositions,
            OriginalFaceUp = faceUp,
            OriginalSiblingIndexes = siblingIndexes,
            OriginalSortingOrders = sortingOrders,
            RevealedCard = revealedCard,
            RevealedCardFaceUp = revealedCardFaceUp,
            RevealedCardSortingOrder = revealedCardSortingOrder,
            RevealedCardSiblingIndex = revealedCardSiblingIndex
        });
    }

    public void Undo()
    {
        if (_undoStack.Count == 0) return;

        UndoAction action = _undoStack.Pop();

        for (int i = 0; i < action.Cards.Length; i++)
        {
            var card = action.Cards[i];

            //restore Parent, location
            card.SetParent(action.FromParent, false);
            card.localPosition = action.OriginalPositions[i];
            card.SetSiblingIndex(action.OriginalSiblingIndexes[i]);

            // Restore face card
            var cardComp = card.GetComponent<Card>();
            bool shouldBeFaceUp = action.OriginalFaceUp[i];
            cardComp.isFaceUp = shouldBeFaceUp;
            cardComp.GetComponent<SpriteRenderer>().sprite = shouldBeFaceUp ? cardComp.frontFace : cardComp.backFace;
            card.GetComponent<Collider2D>().enabled = shouldBeFaceUp;

            // Restore Order
            var sr = card.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sortingOrder = action.OriginalSortingOrders[i];

            Debug.Log($"[UNDO] Card: {cardComp.cardValue} {cardComp.suit} - isFaceUp: {shouldBeFaceUp}");
        }

        // Restore card under
        if (action.RevealedCard != null)
        {
            var rc = action.RevealedCard.GetComponent<Card>();
            rc.isFaceUp = action.RevealedCardFaceUp;
            rc.GetComponent<SpriteRenderer>().sprite = action.RevealedCardFaceUp ? rc.frontFace : rc.backFace;
            rc.GetComponent<Collider2D>().enabled = action.RevealedCardFaceUp;

            rc.transform.SetSiblingIndex(action.RevealedCardSiblingIndex);
            var sr = rc.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sortingOrder = action.RevealedCardSortingOrder;

            Debug.Log($"[UNDO] Revealed Card: {rc.cardValue} {rc.suit} - Restored faceUp: {rc.isFaceUp}");
        }

        Debug.Log("Undo completed.");
    }
}