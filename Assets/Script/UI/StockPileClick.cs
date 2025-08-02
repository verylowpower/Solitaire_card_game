using UnityEngine;
using Managers;

namespace UI
{
    public class StockPileClick : MonoBehaviour
    {
        private void OnMouseDown()
        {
            DeckManager.instance.OnStockPileClicked();
        }
    }
}
