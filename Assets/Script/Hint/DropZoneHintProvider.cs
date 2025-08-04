using System.Collections.Generic;
using UnityEngine;
using Core;
using Managers;

namespace Hint
{
    public class DropZoneHintProvider : IHintProvider
    {
        public Transform GetHintDropZone(Card card, List<Transform> allDropZones)
        {
            foreach (var dropZone in allDropZones)
            {
                string zoneType = dropZone.tag;
                Card topCard = Zones.DropZoneHelper.GetTopCard(dropZone);

                if (zoneType == "Tableau" &&
                    RuleManager.instance.IsValidTableauDrop(card, topCard))
                {
                    return dropZone;
                }

                if (zoneType == "Foundation" &&
                    RuleManager.instance.IsValidFoundationDrop(card, topCard))
                {
                    return dropZone;
                }
            }

            return null;
        }
    }
}
