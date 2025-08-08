using UnityEngine;
using Core;
using Managers;
using Zones;
using Undo;
using DragDrop.Helper;

namespace DragDrop
{
    [RequireComponent(typeof(Card))]
    public class CardDrag : MonoBehaviour, IDraggable
    {
        private bool isDragging;
        private Vector3 offset;
        private Transform originalParent;
        private Camera mainCamera;
        private Transform[] draggedStack;
        private Vector3[] originalLocalPositions;
        private int[] originalSortingOrders;


        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void OnMouseDown()
        {
            Card card = GetComponent<Card>();
            if (!card.IsFaceUp) return;

            originalParent = transform.parent;

            draggedStack = DragStackHelper.GetDraggedStack(transform, originalParent, card);
            if (draggedStack == null) return;

            isDragging = true;
            originalLocalPositions = PositionHelper.RecordLocalPositions(draggedStack);
            offset = PositionHelper.CalculateOffset(transform.position, mainCamera);
            originalSortingOrders = SortingOrderHelper.RecordSortingOrders(draggedStack);
            SortingOrderHelper.BringToFront(draggedStack);
        }

        private void OnMouseDrag()
        {
            if (!isDragging) return;

            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 basePosition = mousePos + offset;
            PositionHelper.MoveStack(draggedStack, basePosition);
        }

        private void OnMouseUp()
        {
            if (!isDragging) return;
            isDragging = false;

            Transform dropZone = DropZoneDetector.GetValidDropZone(mainCamera, out string zoneType);
            if (dropZone == null)
            {
                ReturnToOriginalPosition();
                Debug.Log("Drop zone not found. Reverting.");
                return;
            }

            Card currentCard = draggedStack[0].GetComponent<Card>();
            Card topCard = DropZoneHelper.GetTopCard(dropZone);
            bool isValid = RuleManager.instance.ValidateMove(currentCard, topCard, zoneType, draggedStack.Length);

            if (isValid)
            {
                UndoManager.Instance.RecordMove(draggedStack, originalParent, dropZone, originalLocalPositions);
                DropZoneHelper.Drop(draggedStack, dropZone, zoneType);
                DropZoneHelper.TryFlipLastCard(originalParent);
                Object.FindAnyObjectByType<AutoMoveToFoundation>()?.TryAutoComplete();
                Debug.Log("Dropped valid stack to " + zoneType);
            }
            else
            {
                ReturnToOriginalPosition();
                Debug.Log("Invalid drop by rule.");
            }
        }

        private void ReturnToOriginalPosition()
        {
            for (int i = 0; i < draggedStack.Length; i++)
            {
                Transform card = draggedStack[i];
                card.SetParent(originalParent);
                card.localPosition = originalLocalPositions[i];
                SortingOrderHelper.ApplySortingOrders(draggedStack, originalSortingOrders);

            }
        }
    }
}

// using UnityEngine;
// using Core;
// using Managers;
// using Zones;
// using Undo;
// using DragDrop.Helper;

// namespace DragDrop
// {
//     [RequireComponent(typeof(Card))]
//     public class CardDrag : MonoBehaviour, IDraggable
//     {
//         private bool isDragging;
//         private Vector3 offset;
//         private Transform originalParent;
//         private Camera mainCamera;
//         private Transform[] draggedStack;
//         private Vector3[] originalLocalPositions;
//         private int[] originalSortingOrders;

//         private int draggingFingerId = -1;

//         private void Start()
//         {
//             mainCamera = Camera.main;
//         }

//         private void Update()
//         {
//             if (Input.touchCount > 0)
//             {
//                 for (int i = 0; i < Input.touchCount; i++)
//                 {
//                     Touch touch = Input.GetTouch(i);
//                     Vector3 touchWorldPos = mainCamera.ScreenToWorldPoint(touch.position);
//                     touchWorldPos.z = 0f;

//                     switch (touch.phase)
//                     {
//                         case TouchPhase.Began:
//                             TryStartDrag(touch, touchWorldPos);
//                             break;

//                         case TouchPhase.Moved:
//                         case TouchPhase.Stationary:
//                             if (isDragging && touch.fingerId == draggingFingerId)
//                                 DragMove(touchWorldPos);
//                             break;

//                         case TouchPhase.Ended:
//                         case TouchPhase.Canceled:
//                             if (isDragging && touch.fingerId == draggingFingerId)
//                                 EndDrag();
//                             break;
//                     }
//                 }
//             }
//         }

//         private void TryStartDrag(Touch touch, Vector3 touchWorldPos)
//         {
//             // Kiểm tra xem touch có nhấn vào đúng lá bài không
//             RaycastHit2D hit = Physics2D.Raycast(touchWorldPos, Vector2.zero);
//             if (hit.collider == null || hit.collider.gameObject != gameObject) return;

//             Card card = GetComponent<Card>();
//             if (!card.IsFaceUp) return;

//             originalParent = transform.parent;
//             draggedStack = DragStackHelper.GetDraggedStack(transform, originalParent, card);
//             if (draggedStack == null) return;

//             isDragging = true;
//             draggingFingerId = touch.fingerId;

//             originalLocalPositions = PositionHelper.RecordLocalPositions(draggedStack);
//             offset = PositionHelper.CalculateOffset(transform.position, mainCamera);
//             originalSortingOrders = SortingOrderHelper.RecordSortingOrders(draggedStack);
//             SortingOrderHelper.BringToFront(draggedStack);
//         }

//         private void DragMove(Vector3 touchWorldPos)
//         {
//             Vector3 basePosition = touchWorldPos + offset;
//             PositionHelper.MoveStack(draggedStack, basePosition);
//         }

//         private void EndDrag()
//         {
//             isDragging = false;
//             draggingFingerId = -1;

//             Transform dropZone = DropZoneDetector.GetValidDropZone(mainCamera, out string zoneType);
//             if (dropZone == null)
//             {
//                 ReturnToOriginalPosition();
//                 Debug.Log("Drop zone not found. Reverting.");
//                 return;
//             }

//             Card currentCard = draggedStack[0].GetComponent<Card>();
//             Card topCard = DropZoneHelper.GetTopCard(dropZone);
//             bool isValid = RuleManager.instance.ValidateMove(currentCard, topCard, zoneType, draggedStack.Length);

//             if (isValid)
//             {
//                 UndoManager.Instance.RecordMove(draggedStack, originalParent, dropZone, originalLocalPositions);
//                 DropZoneHelper.Drop(draggedStack, dropZone, zoneType);
//                 DropZoneHelper.TryFlipLastCard(originalParent);
//                 Object.FindAnyObjectByType<AutoMoveToFoundation>()?.TryAutoComplete();
//                 Debug.Log("Dropped valid stack to " + zoneType);
//             }
//             else
//             {
//                 ReturnToOriginalPosition();
//                 Debug.Log("Invalid drop by rule.");
//             }
//         }

//         private void ReturnToOriginalPosition()
//         {
//             for (int i = 0; i < draggedStack.Length; i++)
//             {
//                 Transform card = draggedStack[i];
//                 card.SetParent(originalParent);
//                 card.localPosition = originalLocalPositions[i];
//             }
//             SortingOrderHelper.ApplySortingOrders(draggedStack, originalSortingOrders);
//         }
//     }
// }
