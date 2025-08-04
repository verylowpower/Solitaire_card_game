using UnityEngine;

public class HintHighlighter : MonoBehaviour
{
    [SerializeField] private float duration = 1.5f;

    private void Start()
    {
        Destroy(gameObject, duration);
    }
}
