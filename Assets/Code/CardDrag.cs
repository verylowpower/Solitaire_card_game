using UnityEngine;

public class CardDrag : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    // private Vector3 originalPosition;
    private Transform originalParent;
    private Camera mainCamera;
    private Transform[] draggedStack;
    private Vector3[] originalLocalPositions;


    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        Card card = GetComponent<Card>();
        if (!card.isFaceUp) return;

        originalParent = transform.parent;

        // Nếu đang ở waste pile → chỉ kéo lá trên cùng
        if (originalParent != null && originalParent.name.Contains("Waste"))
        {
            int lastIndex = originalParent.childCount - 1;
            if (transform.GetSiblingIndex() != lastIndex)
            {
                Debug.Log("Không thể kéo lá này từ waste (không phải lá trên cùng)");
                return;
            }

            // Đánh dấu là kéo từ waste
            card.wasMovedFromWaste = true;

            // Chỉ kéo 1 lá
            draggedStack = new Transform[1];
            draggedStack[0] = transform;
        }
        else
        {
            // Kéo từ tableau: kéo cả stack phía dưới
            int index = transform.GetSiblingIndex();
            int count = originalParent.childCount;

            draggedStack = new Transform[count - index];
            for (int i = 0; i < draggedStack.Length; i++)
            {
                draggedStack[i] = originalParent.GetChild(index + i);
            }
        }

        isDragging = true;

        //originalPosition = transform.position;

        originalLocalPositions = new Vector3[draggedStack.Length];
        for (int i = 0; i < draggedStack.Length; i++)
        {
            originalLocalPositions[i] = draggedStack[i].localPosition;
        }

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mousePos.x, mousePos.y, 0f);

        int baseOrder = GetHighestSortingOrderInScene() + 1;
        for (int i = 0; i < draggedStack.Length; i++)
        {
            var sr = draggedStack[i].GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sortingOrder = baseOrder + i;
        }

        Debug.Log("Dragging " + draggedStack.Length + " card(s).");
    }


    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 basePosition = new Vector3(mousePos.x, mousePos.y, 0f) + offset;

        for (int i = 0; i < draggedStack.Length; i++)
        {
            draggedStack[i].position = basePosition + new Vector3(0, -i * 0.2f, 0);

        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        Transform dropZone = GetValidDropZone(out string zoneType);

        if (dropZone != null)
        {
            Card currentCard = draggedStack[0].GetComponent<Card>();
            Card topCard = GetTopCard(dropZone);

            bool isValid = false;

            if (zoneType == "FoundationDropZone" && draggedStack.Length == 1)
            {
                isValid = RuleManager.instance.IsValidFoundationDrop(currentCard, topCard);
            }
            else if (zoneType == "TableauDropZone")
            {
                isValid = RuleManager.instance.IsValidTableauDrop(currentCard, topCard);
            }

            if (isValid)
            {
                // 🟢 Xác định lá sẽ bị lộ ra sau khi kéo stack
                Transform revealedCard = null;
                bool revealedCardFaceUp = false;
                int revealedCardSortingOrder = 0;
                int revealedCardSiblingIndex = 0;

                int draggedIndex = draggedStack[0].GetSiblingIndex();
                if (originalParent.childCount > draggedIndex)
                {
                    int revealIndex = draggedIndex - 1;
                    if (revealIndex >= 0)
                    {
                        revealedCard = originalParent.GetChild(revealIndex);
                        var rc = revealedCard.GetComponent<Card>();
                        if (rc != null)
                        {
                            revealedCardFaceUp = rc.isFaceUp;
                            revealedCardSortingOrder = rc.GetComponent<SpriteRenderer>().sortingOrder;
                            revealedCardSiblingIndex = rc.transform.GetSiblingIndex();
                        }
                    }
                }


                UndoManager.Instance.RecordMove(
                    draggedStack,
                    originalParent,
                    dropZone,
                    originalLocalPositions
                );


                if (zoneType == "FoundationDropZone")
                    DropCard(draggedStack[0], dropZone);
                else
                    DropStack(dropZone);


                TryFlipLastCard(originalParent);

                Debug.Log("Dropped valid stack to " + zoneType);
            }
            else
            {
                ReturnToOriginalPosition();
                Debug.Log("Invalid drop by rule.");
            }
        }
        else
        {
            ReturnToOriginalPosition();
            Debug.Log("Drop zone not found. Reverting.");
        }
    }


    void DropStack(Transform dropZone)
    {
        int baseCount = dropZone.childCount;
        for (int i = 0; i < draggedStack.Length; i++)
        {
            var card = draggedStack[i];
            card.SetParent(dropZone);
            card.localPosition = new Vector3(0, -(baseCount + i) * 0.2f, 0);
            card.SetSiblingIndex(dropZone.childCount - 1);
        }
    }

    void DropCard(Transform card, Transform dropZone)
    {
        card.SetParent(dropZone);
        card.localPosition = Vector3.zero;
        card.SetSiblingIndex(dropZone.childCount - 1);
    }

    void ReturnToOriginalPosition()
    {
        for (int i = 0; i < draggedStack.Length; i++)
        {
            var card = draggedStack[i];
            card.SetParent(originalParent);


            card.localPosition = originalLocalPositions[i];

            var sr = card.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sortingOrder = GetHighestSortingOrderInScene() + i;
        }
    }


    Transform GetValidDropZone(out string zoneType)
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        float minDistance = float.MaxValue;
        Transform closestZone = null;
        zoneType = "";

        foreach (GameObject zone in GameObject.FindGameObjectsWithTag("TableauDropZone"))
        {
            float dist = Vector2.Distance(mousePos, zone.transform.position);
            if (dist < 1f && dist < minDistance)
            {
                minDistance = dist;
                closestZone = zone.transform;
                zoneType = "TableauDropZone";
            }
        }

        foreach (GameObject zone in GameObject.FindGameObjectsWithTag("FoundationDropZone"))
        {
            float dist = Vector2.Distance(mousePos, zone.transform.position);
            if (dist < 1f && dist < minDistance)
            {
                minDistance = dist;
                closestZone = zone.transform;
                zoneType = "FoundationDropZone";
            }
        }

        return closestZone;
    }

    void TryFlipLastCard(Transform column)
    {
        if (column.childCount == 0) return;

        Transform lastCard = column.GetChild(column.childCount - 1);
        Card card = lastCard.GetComponent<Card>();

        if (card != null && !card.isFaceUp)
        {
            card.SetFaceUp(true);
            card.GetComponent<Collider2D>().enabled = true;

            var sr = card.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.sortingOrder = GetHighestSortingOrderInScene() + 1;

            Debug.Log("Flipped last card in column: " + column.name);
        }
    }

    int GetHighestSortingOrderInScene()
    {
        int highest = 0;
        SpriteRenderer[] allCards = Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None);
        foreach (var sr in allCards)
        {
            if (sr.sortingOrder > highest)
                highest = sr.sortingOrder;
        }
        return highest;
    }

    Card GetTopCard(Transform dropZone)
    {
        if (dropZone.childCount == 0) return null;

        Transform lastChild = dropZone.GetChild(dropZone.childCount - 1);
        return lastChild.GetComponent<Card>();
    }

}
