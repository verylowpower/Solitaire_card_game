using UnityEngine;
using Core;
using Managers;

namespace Undo
{
    public class UndoActionFlipStock : IUndoAction
    {
        private readonly Card _card;
        private readonly Transform _stockPile;
        private readonly Transform _wastePile;

        public UndoActionFlipStock(Card card, Transform stockPile, Transform wastePile)
        {
            _card = card;
            _stockPile = stockPile;
            _wastePile = wastePile;
        }

        public void Undo()
        {
            if (_card != null && _card.transform.parent == _wastePile)
            {
                _card.transform.SetParent(_stockPile);
                _card.transform.localPosition = Vector3.zero;

                _card.SetFaceUp(false);
                _card.IsTopCard = false;
                _card.GetComponent<Collider2D>().enabled = false;

                _card.GetComponent<SpriteRenderer>().sortingOrder = 0;
                _card.transform.SetSiblingIndex(0);

                DeckManager.instance.PushBackToStock(_card);
            }
        }
    }
}
