using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core;
using Hint;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class HintButton : MonoBehaviour
    {
        private Button hintButton;
        private HintManager hintManager;

        [SerializeField] private GameObject cardHighlightPrefab;  // Highlight cho card
        [SerializeField] private GameObject zoneHighlightPrefab;  // Highlight cho zone

        private void Awake()
        {
            hintButton = GetComponent<Button>();
            hintManager = new HintManager(new DropZoneHintProvider());
            hintButton.onClick.AddListener(OnHintClicked);
        }

        private void OnHintClicked()
        {
            List<Transform> allDropZones = GetAllDropZones();
            Card[] allCards = GameObject.FindObjectsByType<Card>(FindObjectsSortMode.None);

            foreach (Card card in allCards)
            {
                if (!card.IsFaceUp || !card.IsTopCard) continue;
                
                Transform hintZone = hintManager.GetHint(card, allDropZones);
                if (hintZone != null)
                {
                    Debug.Log("Hint: Move " + card.name + " to " + hintZone.name);

                    // Highlight cả card lẫn zone
                    ShowCardHighlight(card.transform);
                    ShowZoneHighlight(hintZone);

                    return; // chỉ highlight 1 nước đi
                }
            }

            Debug.Log("No valid hint found.");
        }

        private List<Transform> GetAllDropZones()
        {
            List<Transform> zones = new List<Transform>();
            foreach (var obj in GameObject.FindGameObjectsWithTag("Tableau"))
                zones.Add(obj.transform);
            foreach (var obj in GameObject.FindGameObjectsWithTag("Foundation"))
                zones.Add(obj.transform);
            return zones;
        }

        private void ShowCardHighlight(Transform cardTransform)
        {
            if (cardHighlightPrefab != null)
                Instantiate(cardHighlightPrefab, cardTransform.position, Quaternion.identity, cardTransform);
        }

        private void ShowZoneHighlight(Transform zoneTransform)
        {
            if (zoneHighlightPrefab != null)
                Instantiate(zoneHighlightPrefab, zoneTransform.position, Quaternion.identity, zoneTransform);
        }
    }
}
