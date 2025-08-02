using UnityEngine;
using UnityEngine.UI;
using Undo;
using Managers;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class UndoButton : MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnUndoClicked);
        }

        public void OnUndoClicked()
        {
            UndoManager.Instance?.Undo();
        }
    }
}
