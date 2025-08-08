using UnityEngine;
using UnityEngine.UI;
using Managers;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class ResetButton : MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnResetClicked);
        }

        private void OnResetClicked()
        {
            DeckManager.instance.ResetGame();
        }
    }
}
