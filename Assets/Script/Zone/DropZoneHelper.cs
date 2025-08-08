using UnityEngine;
using Core;
using DragDrop.Helper;

namespace Zones
{
    public static class DropZoneHelper
    {
        public static Card GetTopCard(Transform dropZone)
        {
            if (dropZone.childCount == 0) return null;
            return dropZone.GetChild(dropZone.childCount - 1).GetComponent<Card>();
        }

        public static void Drop(Transform[] stack, Transform dropZone, string zoneType)
        {
            if (zoneType == "Foundation")
            {
                Transform card = stack[0];
                card.SetParent(dropZone);
                card.localPosition = Vector3.zero;
                card.SetSiblingIndex(dropZone.childCount - 1);
            }
            else
            {
                int baseCount = dropZone.childCount;
                for (int i = 0; i < stack.Length; i++)
                {
                    var card = stack[i];
                    card.SetParent(dropZone);
                    card.localPosition = new Vector3(0, -(baseCount + i) * 0.2f, 0);
                    card.SetSiblingIndex(dropZone.childCount - 1);
                }
            }
        }

        public static void TryFlipLastCard(Transform column)
        {
            if (column.childCount == 0) return;

            Transform lastCard = column.GetChild(column.childCount - 1);
            Card card = lastCard.GetComponent<Card>();

            if (card != null && !card.IsFaceUp)
            {
                card.SetFaceUp(true);
                card.GetComponent<Collider2D>().enabled = true;

                // Fix sortingOrder here:
                SortingOrderHelper.ApplySortingOrder(lastCard, column.childCount - 1);

                Debug.Log("Flipped last card in column: " + column.name);
            }
        }

    }
}
