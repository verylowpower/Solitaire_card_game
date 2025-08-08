using UnityEngine;
using Core;

namespace DragDrop.Helper
{
    public static class DragStackHelper
    {
        public static Transform[] GetDraggedStack(Transform cardTransform, Transform parent, Card card)
        {
            if (parent == null) return null;

            if (parent.name.Contains("Waste"))
            {
                if (cardTransform.GetSiblingIndex() != parent.childCount - 1)
                {
                    Debug.Log("Không thể kéo lá này từ waste (không phải lá trên cùng)");
                    return null;
                }

                card.WasMovedFromWaste = true;
                return new Transform[] { cardTransform };
            }

            int index = cardTransform.GetSiblingIndex();
            int count = parent.childCount;
            Transform[] stack = new Transform[count - index];
            for (int i = 0; i < stack.Length; i++)
                stack[i] = parent.GetChild(index + i);

            return stack;
        }
    }
}
