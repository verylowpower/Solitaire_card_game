using UnityEngine;

namespace DragDrop.Helper
{
    public static class SortingOrderHelper
    {
        public static void BringToFront(Transform[] stack)
        {
            int startOrder = 30;
            for (int i = 0; i < stack.Length; i++)
            {
                var sr = stack[i].GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingOrder = startOrder + i;
                }
            }
        }

        public static int[] RecordSortingOrders(Transform[] stack)
        {
            int[] orders = new int[stack.Length];
            for (int i = 0; i < stack.Length; i++)
            {
                var sr = stack[i].GetComponent<SpriteRenderer>();
                orders[i] = sr != null ? sr.sortingOrder : 0;
            }
            return orders;
        }
        public static void ApplySortingOrder(Transform card, int sortingOrder)
        {
            var sr = card.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = sortingOrder;
            }
        }

        public static void ApplySortingOrders(Transform[] stack, int[] orders)
        {
            for (int i = 0; i < stack.Length; i++)
            {
                var sr = stack[i].GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.sortingOrder = orders[i];
                }
            }
        }
    }
}
