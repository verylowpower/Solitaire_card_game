using UnityEngine;

public class StockPileClick : MonoBehaviour
{
    void OnMouseDown()
    {
        DeckManager.instance.OnStockPileClicked();
    }
}
