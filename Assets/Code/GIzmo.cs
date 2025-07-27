using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class DropZoneGizmo : MonoBehaviour
{
    public Color gizmoColor = Color.green;
    public Vector2 size = new Vector2(1f, 1.5f); // Kích thước hình vuông/hình chữ nhật

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        // Tính vị trí góc giữa (centered box)
        Vector3 center = transform.position;
        Vector3 halfSize = new Vector3(size.x / 2f, size.y / 2f, 0f);

        // Vẽ hình chữ nhật
        Gizmos.DrawWireCube(center, new Vector3(size.x, size.y, 0.01f));
    }

    // void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawCube(transform.position, new Vector3(size.x * 0.9f, size.y * 0.9f, 0.01f));
    // }
}
