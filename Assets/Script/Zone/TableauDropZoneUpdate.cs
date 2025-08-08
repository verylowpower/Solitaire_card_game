using UnityEngine;

namespace Zones
{
    public class TableauDropZoneUpdater : MonoBehaviour
    {
        [Tooltip("Cột bài mà DropZone này sẽ theo dõi")]
        public Transform columnParent;

        [Tooltip("Khoảng cách giữa lá bài cuối cùng và DropZone")]
        public float spacing = -0.2f;

        [Tooltip("Khoảng dịch thêm nếu không có lá bài nào")]
        public float extraOffset = -0.3f;

        private void Update()
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
}
