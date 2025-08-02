using UnityEngine;

namespace DragDrop.Helper
{
    public static class DropZoneDetector
    {
        public static Transform GetValidDropZone(Camera cam, out string zoneType)
        {
            zoneType = null;
            Vector2 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Tableau"))
                {
                    zoneType = "Tableau";
                    return hit.collider.transform;
                }

                if (hit.collider.CompareTag("Foundation"))
                {
                    zoneType = "Foundation";
                    return hit.collider.transform;
                }
            }

            return null;
        }
    }
}
