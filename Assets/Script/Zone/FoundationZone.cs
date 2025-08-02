using UnityEngine;
using Core;
using Managers;

namespace Zones
{
    public class FoundationZone : MonoBehaviour, IZone
    {
        public bool IsCardAllowedToDrop(Card card, int stackLength)
        {
            if (stackLength > 1) return false; // chỉ cho phép kéo 1 lá vào foundation
            Card topCard = GetTopCard();
            return RuleManager.instance.IsValidFoundationDrop(card, topCard);
        }

        public Card GetTopCard()
        {
            if (transform.childCount == 0) return null;

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (child.TryGetComponent(out Card c))
                    return c;
            }

            return null;
        }

        public Transform GetTransform() => transform;
    }
}
