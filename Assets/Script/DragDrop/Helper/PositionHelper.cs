using UnityEngine;

namespace DragDrop.Helper
{
    public static class PositionHelper
    {
        public static Vector3[] RecordLocalPositions(Transform[] cards)
        {
            Vector3[] positions = new Vector3[cards.Length];
            for (int i = 0; i < cards.Length; i++)
            {
                positions[i] = cards[i].localPosition;
            }
            return positions;
        }

        public static Vector3 CalculateOffset(Vector3 worldPos, Camera camera)
        {
            Vector3 mouseWorldPos = camera.ScreenToWorldPoint(Input.mousePosition);
            return new Vector3(worldPos.x - mouseWorldPos.x, worldPos.y - mouseWorldPos.y, 0); // giữ nguyên z = 0
        }

        public static void MoveStack(Transform[] stack, Vector3 basePosition)
        {
            float baseZ = stack[0].position.z; // giữ nguyên Z
            for (int i = 0; i < stack.Length; i++)
            {
                stack[i].position = new Vector3(
                    basePosition.x,
                    basePosition.y - i * 0.2f,
                    baseZ
                );
            }
        }

    }
}
