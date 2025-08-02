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
