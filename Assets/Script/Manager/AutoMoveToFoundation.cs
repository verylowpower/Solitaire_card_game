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
            Debug.Log("TryAutoComplete called");
            if (!CanAutoComplete())
            {
                Debug.Log("Auto-complete conditions not met.");
                return;
            }

            Debug.Log("Conditions met. Auto moving...");
            MoveAllValidCardsToFoundation();
        }

        // Kiểm tra có thể auto-complete hay không (mô phỏng bằng RuleManager)
        private bool CanAutoComplete()
        {
            if (foundationZones == null || foundationZones.Count == 0)
            {
                Debug.LogWarning("Foundations not assigned in inspector.");
                return false;
            }

            // Nếu tất cả tableau đã trống thì cho phép
            if (AllTableauCleared()) return true;

            // Lấy tất cả card ngoài foundation
            Card[] allCards = GameObject.FindObjectsByType<Card>(FindObjectsSortMode.None);
            List<Card> remaining = new List<Card>();
            foreach (Card c in allCards)
            {
                if (IsInAnyFoundation(c.transform)) continue; // đã ở foundation -> bỏ qua

                // Nếu có card úp ngoài foundation => không thể auto
                if (!c.IsFaceUp)
                {
                    Debug.Log("Cannot auto-complete: found face-down card outside foundation: " + c.name);
                    return false;
                }
                remaining.Add(c);
            }

            // Mô phỏng: lặp, tìm card nào có thể đặt lên một foundation (theo RuleManager),
            // nếu tìm được thì "loại" card đó khỏi remaining và tiếp tục.
            bool movedAny;
            do
            {
                movedAny = false;
                for (int i = remaining.Count - 1; i >= 0; i--)
                {
                    Card card = remaining[i];
                    for (int f = 0; f < foundationZones.Count; f++)
                    {
                        Transform foundation = foundationZones[f];
                        Card top = DropZoneHelper.GetTopCard(foundation);
                        if (RuleManager.instance.IsValidFoundationDrop(card, top))
                        {
                            // "di chuyển" giả định: loại bỏ card khỏi danh sách remaining
                            remaining.RemoveAt(i);
                            movedAny = true;
                            break;
                        }
                    }
                }
            } while (movedAny);

            return remaining.Count == 0;
        }

        private bool IsInAnyFoundation(Transform t)
        {
            if (foundationZones == null) return false;
            foreach (Transform f in foundationZones)
            {
                if (t.IsChildOf(f)) return true;
            }
            return false;
        }

        private bool AllTableauCleared()
        {
            if (tableauZones == null) return false;
            foreach (Transform column in tableauZones)
            {
                if (column.childCount > 0) return false;
            }
            return true;
        }

        // Di chuyển thực tế — lặp nhiều lượt cho đến khi không còn lá nào có thể di chuyển
        private void MoveAllValidCardsToFoundation()
        {
            if (foundationZones == null || foundationZones.Count == 0)
            {
                Debug.LogWarning("Foundations not assigned in inspector.");
                return;
            }

            bool moved;
            do
            {
                moved = false;
                Card[] allCards = GameObject.FindObjectsByType<Card>(FindObjectsSortMode.None);
                foreach (Card card in allCards)
                {
                    if (!card.IsFaceUp) continue;
                    if (IsInAnyFoundation(card.transform)) continue;

                    foreach (Transform foundation in foundationZones)
                    {
                        Card topCard = DropZoneHelper.GetTopCard(foundation);
                        if (RuleManager.instance.IsValidFoundationDrop(card, topCard))
                        {
                            Debug.Log("Moving " + card.name + " to " + foundation.name);

                            // Ghi undo nếu cần: UndoManager.Instance?.RecordMove(...)
                            card.transform.SetParent(foundation);
                            card.transform.localPosition = Vector3.zero;
                            card.GetComponent<Collider2D>().enabled = false;

                            SortingOrderHelper.ApplySortingOrder(card.transform, foundation.childCount - 1);

                            moved = true;
                            break;
                        }
                    }

                    if (moved) break; // bắt đầu vòng quét lại từ đầu sau khi di chuyển 1 lá
                }
            } while (moved);
        }
    }
}
