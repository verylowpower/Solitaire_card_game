using UnityEngine;

namespace DragDrop.Helper
{
    public static class DropZoneDetector
    {
        public static Transform GetValidDropZone(Camera cam, out string zoneType)
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            float minDistance = float.MaxValue;
            Transform closestZone = null;
            zoneType = "";

            // Check Tableau Zones
            foreach (GameObject zone in GameObject.FindGameObjectsWithTag("Tableau"))
            {
                float dist = Vector2.Distance(mousePos, zone.transform.position);
                if (dist < 1f && dist < minDistance)
                {
                    minDistance = dist;
                    closestZone = zone.transform;
                    zoneType = "Tableau";
                }
            }

            // Check Foundation Zones
            foreach (GameObject zone in GameObject.FindGameObjectsWithTag("Foundation"))
            {
                float dist = Vector2.Distance(mousePos, zone.transform.position);
                if (dist < 1f && dist < minDistance)
                {
                    minDistance = dist;
                    closestZone = zone.transform;
                    zoneType = "Foundation";
                }
            }

            return closestZone;
        }
    }
}
