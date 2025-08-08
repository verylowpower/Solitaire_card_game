using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using DragDrop.Helper;  
using Managers;
using Zones;

namespace Managers
{
    public class AutoMoveToFoundation : MonoBehaviour
    {
        [SerializeField] private List<Transform> tableauZones;
        [SerializeField] private List<Transform> foundationZones;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) 
            {
                TryAutoComplete();
            }
        }

        public void TryAutoComplete()
        {   
            Debug.Log("Press");
            if (!AllTableauCleared()) return;

            Debug.Log("All tableau columns cleared. Auto moving cards to foundation...");
            MoveAllValidCardsToFoundation();
        }

        private bool AllTableauCleared()
        {
            foreach (Transform column in tableauZones)
            {
                if (column.childCount > 0) return false;
            }
            return true;
        }

        private void MoveAllValidCardsToFoundation()
        {
            Card[] allCards = GameObject.FindObjectsByType<Card>(FindObjectsSortMode.None);
            foreach (Card card in allCards)
            {
                if (!card.IsFaceUp) continue;

                foreach (Transform foundation in foundationZones)
                {
                    Card topCard = DropZoneHelper.GetTopCard(foundation);
                    if (RuleManager.instance.IsValidFoundationDrop(card, topCard))
                    {
                        Debug.Log("Moving " + card.name + " to " + foundation.name);

                        card.transform.SetParent(foundation);
                        card.transform.localPosition = Vector3.zero;
                        card.GetComponent<Collider2D>().enabled = false;

                        SortingOrderHelper.ApplySortingOrder(card.transform, foundation.childCount - 1);
                        break;
                    }
                }
            }
        }
    }
}
