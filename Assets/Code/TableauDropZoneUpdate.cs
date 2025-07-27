using UnityEngine;

public class TableauDropZoneUpdater : MonoBehaviour
{
    public Transform columnParent; 
    public float spacing = -0.2f;
    public float extraOffset = -0.3f; // để kéo DropZone xuống dưới cùng

    void Update()
    {
        if (columnParent == null) return;

        int cardCount = columnParent.childCount;
        if (cardCount == 0)
        {
            transform.position = columnParent.position + new Vector3(0, extraOffset, 0);
        }
        else
        {
            Transform lastCard = columnParent.GetChild(cardCount - 1);
            Vector3 bottomPos = lastCard.position + new Vector3(0, spacing, 0);
            transform.position = bottomPos;
        }
    }
}
