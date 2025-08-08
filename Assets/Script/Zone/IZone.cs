using UnityEngine;
using Core;

namespace Zones
{
    public interface IZone
    {
        bool IsCardAllowedToDrop(Card card, int stackLength);
        Card GetTopCard();
        Transform GetTransform();
    }
}
