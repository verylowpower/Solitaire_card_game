using UnityEngine;
using System.Collections.Generic;

public enum UndoType { Move, FlipStock }

public class UndoAction
{
    public UndoType ActionType = UndoType.Move;

    // --- Cho hành động Move ---
    public Transform[] Cards;
    public Transform FromParent;
    public Transform ToParent;
    public Vector3[] OriginalPositions;
    public bool[] OriginalFaceUp;
    public int[] OriginalSiblingIndexes;
    public int[] OriginalSortingOrders;

    // --- Cho card bị lật sau kéo ---
    public Transform RevealedCard;
    public bool RevealedCardFaceUp;
    public int RevealedCardSortingOrder;
    public int RevealedCardSiblingIndex;

    // --- Cho hành động FlipStock ---
    public Card FlippedCard;
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

    public void RecordFlipStock(Card card)
    {
        _undoStack.Push(new UndoAction
        {
            ActionType = UndoType.FlipStock,
            FlippedCard = card
        });
    }


    // public void Undo()
    // {
    //     if (_undoStack.Count == 0) return;

    //     UndoAction action = _undoStack.Pop();

    //     for (int i = 0; i < action.Cards.Length; i++)
    //     {
    //         var card = action.Cards[i];

    //         //restore Parent, location
    //         card.SetParent(action.FromParent, false);
    //         card.localPosition = action.OriginalPositions[i];
    //         card.SetSiblingIndex(action.OriginalSiblingIndexes[i]);

    //         // Restore face card
    //         var cardComp = card.GetComponent<Card>();
    //         bool shouldBeFaceUp = action.OriginalFaceUp[i];
    //         cardComp.isFaceUp = shouldBeFaceUp;
    //         cardComp.GetComponent<SpriteRenderer>().sprite = shouldBeFaceUp ? cardComp.frontFace : cardComp.backFace;
    //         card.GetComponent<Collider2D>().enabled = shouldBeFaceUp;

    //         // Restore Order
    //         var sr = card.GetComponent<SpriteRenderer>();
    //         if (sr != null)
    //             sr.sortingOrder = action.OriginalSortingOrders[i];

    //         Debug.Log($"[UNDO] Card: {cardComp.cardValue} {cardComp.suit} - isFaceUp: {shouldBeFaceUp}");
    //     }

    //     // Restore card under
    //     if (action.RevealedCard != null)
    //     {
    //         var rc = action.RevealedCard.GetComponent<Card>();
    //         rc.isFaceUp = action.RevealedCardFaceUp;
    //         rc.GetComponent<SpriteRenderer>().sprite = action.RevealedCardFaceUp ? rc.frontFace : rc.backFace;
    //         rc.GetComponent<Collider2D>().enabled = action.RevealedCardFaceUp;

    //         rc.transform.SetSiblingIndex(action.RevealedCardSiblingIndex);
    //         var sr = rc.GetComponent<SpriteRenderer>();
    //         if (sr != null)
    //             sr.sortingOrder = action.RevealedCardSortingOrder;

    //         Debug.Log($"[UNDO] Revealed Card: {rc.cardValue} {rc.suit} - Restored faceUp: {rc.isFaceUp}");
    //     }

    //     Debug.Log("Undo completed.");
    // }

    public void Undo()
    {
        if (_undoStack.Count == 0) return;

        UndoAction action = _undoStack.Pop();

        if (action.ActionType == UndoType.Move)
        {
            // --- Undo hành động di chuyển bài ---
            for (int i = 0; i < action.Cards.Length; i++)
            {
                var card = action.Cards[i];

                // Khôi phục vị trí và cha
                card.SetParent(action.FromParent, false);
                card.localPosition = action.OriginalPositions[i];
                card.SetSiblingIndex(action.OriginalSiblingIndexes[i]);

                // Khôi phục trạng thái mặt bài
                var cardComp = card.GetComponent<Card>();
                bool shouldBeFaceUp = action.OriginalFaceUp[i];
                cardComp.isFaceUp = shouldBeFaceUp;
                cardComp.GetComponent<SpriteRenderer>().sprite = shouldBeFaceUp ? cardComp.frontFace : cardComp.backFace;
                card.GetComponent<Collider2D>().enabled = shouldBeFaceUp;

                // Khôi phục sorting order
                var sr = card.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sortingOrder = action.OriginalSortingOrders[i];

                Debug.Log($"[UNDO] Card: {cardComp.cardValue} {cardComp.suit} - isFaceUp: {shouldBeFaceUp}");
            }

            // --- Undo card bị lộ ra sau khi kéo ---
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

            Debug.Log("Undo completed (Move).");
        }
        else if (action.ActionType == UndoType.FlipStock)
        {
            // --- Undo hành động lật bài từ stock → waste ---
            Card card = action.FlippedCard;
            if (card != null && card.transform.parent == DeckManager.instance.wastePilePosition)
            {
                card.transform.SetParent(DeckManager.instance.stockPilePosition);
                card.transform.localPosition = Vector3.zero;

                card.SetFaceUp(false);
                card.IsTopCard = false;
                card.GetComponent<Collider2D>().enabled = false;

                card.GetComponent<SpriteRenderer>().sortingOrder = 0;
                card.transform.SetSiblingIndex(0);

                DeckManager.instance.PushBackToStock(card);

                Debug.Log($"[UNDO] FlipStock undone: {card.cardValue} {card.suit}");
            }
        }
    }

}