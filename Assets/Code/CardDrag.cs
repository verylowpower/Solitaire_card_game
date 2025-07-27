using UnityEngine;

public class CardDrag : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 originalPosition;
    private Transform originalParent;
    private Camera mainCamera;
    private Transform[] draggedStack;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        Card card = GetComponent<Card>();
        if (!card.isFaceUp) return;

        isDragging = true;
        originalPosition = transform.position;
        originalParent = transform.parent;

        // Nếu kéo từ waste pile thì đánh dấu là đã được sử dụng
        if (originalParent != null && originalParent.name.Contains("Waste"))
        {
            card.wasMovedFromWaste = true;
        }

        int index = transform.GetSiblingIndex();
        int count = originalParent.childCount;

        draggedStack = new Transform[count - index];
        for (int i = 0; i < draggedStack.Length; i++)
        {
            draggedStack[i] = originalParent.GetChild(index + i);
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

        Debug.Log("Dragging stack.");
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
            if (zoneType == "FoundationDropZone" && draggedStack.Length == 1)
            {
                DropCard(draggedStack[0], dropZone);
                TryFlipLastCard(originalParent);
                Debug.Log("Dropped 1 card to foundation.");
            }
            else if (zoneType == "TableauDropZone")
            {
                DropStack(dropZone);
                TryFlipLastCard(originalParent);
                Debug.Log("Dropped stack to tableau.");
            }
            else
            {
                ReturnToOriginalPosition();
                Debug.Log("Invalid drop for this zone.");
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
            card.position = originalPosition + new Vector3(0, -i * 0.2f, 0);

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
}
