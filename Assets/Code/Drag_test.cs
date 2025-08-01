using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Vector3 _originalPosition;
    private Transform _originalParent;
    private bool _isDragging = false;
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        // Lưu lại vị trí và parent ban đầu
        _originalPosition = transform.position;
        _originalParent = transform.parent;
        _isDragging = true;
    }

    void OnMouseDrag()
    {
        if (!_isDragging) return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // khoảng cách từ camera đến object
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(mousePos);
        transform.position = worldPos;
    }

    void OnMouseUp()
    {
        _isDragging = false;

        // Giả sử DropZone có tag là "DropZone"
        Collider2D hit = Physics2D.OverlapPoint(transform.position);
        if (hit != null && hit.CompareTag("ValidDropZone"))
        {
            transform.SetParent(hit.transform);
            transform.position = hit.transform.position;
        }
        else
        {
            // Không thả vào vùng hợp lệ thì trả về vị trí cũ
            transform.position = _originalPosition;
            transform.SetParent(_originalParent);
        }
    }

    
    public void UndoMove()
    {
        transform.position = _originalPosition;
        transform.SetParent(_originalParent);
    }
}