using UnityEngine;
using Core;
using Managers;

namespace Zones
{
    public class TableauZone : MonoBehaviour, IZone
    {
        public bool IsCardAllowedToDrop(Card card, int stackLength)
        {
            Card topCard = GetTopCard();
            return RuleManager.instance.IsValidTableauDrop(card, topCard);
        }

        public Card GetTopCard()
        {
            if (transform.childCount == 0) return null;

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (child.TryGetComponent(out Card c) && c.IsFaceUp)
                    return c;
            }

            return null;
        }

        public Transform GetTransform() => transform;
    }
}
