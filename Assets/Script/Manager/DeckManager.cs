using UnityEngine;
using System.Collections.Generic;
using Core;
using Undo;
using Managers;
using Rules;

namespace Managers
{
    public class DeckManager : MonoBehaviour
    {
        public static DeckManager instance;

        public GameObject cardPrefabs;
        public Sprite[] cardSprite;
        public Transform[] tableauPiles;
        public Transform[] foundationPiles;
        public Sprite backSprite;

        public Transform stockPilePosition;
        public Transform wastePilePosition;

        private Stack<Card> stockPile = new();
        private Stack<Card> wastePile = new();
        private List<Card> deck = new();

        void Awake()
        {
            instance = this;

            DeckCreate();
            DeckSuffle();
            DealToTableau();
            SetupStockPile();
        }

        void DeckCreate()
        {
            foreach (Card.Suit suit in System.Enum.GetValues(typeof(Card.Suit)))
            {
                for (int val = 1; val <= 13; val++)
                {
                    GameObject cardObj = Instantiate(cardPrefabs, transform);
                    Card card = cardObj.GetComponent<Card>();

                    card.SetSuitAndValue(suit, (Card.CardValue)val);
                    int index = (int)suit * 13 + val - 1;
                    card.SetSprites(cardSprite[index], backSprite);
                    card.SetFaceUp(false);

                    deck.Add(card);
                }
            }
        }

        void DeckSuffle()
        {
            for (int i = 0; i < deck.Count - 1; i++)
            {
                int randIndex = Random.Range(0, deck.Count);
                (deck[i], deck[randIndex]) = (deck[randIndex], deck[i]);
            }

            for (int i = 0; i < deck.Count; i++)
            {
                deck[i].GetComponent<SpriteRenderer>().sortingOrder = i;
                deck[i].transform.SetSiblingIndex(i);
                deck[i].IsTopCard = false;
            }

            deck[^1].IsTopCard = true;
        }

        void DealToTableau()
        {
            int deckIndex = 0;

            for (int col = 0; col < 7; col++)
            {
                for (int row = 0; row <= col; row++)
                {
                    Card card = deck[deckIndex++];
                    card.transform.SetParent(tableauPiles[col]);
                    card.transform.localPosition = new Vector3(0, -row * 0.2f, 0);

                    var sr = card.GetComponent<SpriteRenderer>();
                    sr.sortingOrder = row;

                    bool isLastCard = row == col;
                    card.SetFaceUp(isLastCard);
                    card.IsTopCard = isLastCard;
                    card.GetComponent<Collider2D>().enabled = isLastCard;
                }
            }

            deck.RemoveRange(0, 28);
        }

        void SetupStockPile()
        {
            foreach (Card card in deck)
            {
                card.transform.SetParent(stockPilePosition);
                card.transform.localPosition = Vector3.zero;
                card.SetFaceUp(false);
                card.IsTopCard = false;
                card.GetComponent<Collider2D>().enabled = false;
                stockPile.Push(card);
            }

            deck.Clear();
        }

        public void OnStockPileClicked()
        {
            if (stockPile.Count > 0)
            {
                Card card = stockPile.Pop();
                UndoManager.Instance.RecordFlipStock(card);

                card.transform.SetParent(wastePilePosition);
                card.transform.localPosition = Vector3.zero;
                card.SetFaceUp(true);
                card.IsTopCard = true;
                card.GetComponent<Collider2D>().enabled = true;

                int highestOrder = -1;
                foreach (Transform child in wastePilePosition)
                {
                    if (child.TryGetComponent(out SpriteRenderer sr))
                        highestOrder = Mathf.Max(highestOrder, sr.sortingOrder);
                }

                card.GetComponent<SpriteRenderer>().sortingOrder = highestOrder + 1;
                card.transform.SetSiblingIndex(wastePilePosition.childCount - 1);
                wastePile.Push(card);
            }
            else
            {
                Stack<Card> temp = new();
                for (int i = wastePilePosition.childCount - 1; i >= 0; i--)
                {
                    Transform child = wastePilePosition.GetChild(i);
                    if (child.parent != wastePilePosition) continue;
                    if (child.TryGetComponent(out Card card))
                    {
                        card.transform.SetParent(stockPilePosition);
                        card.transform.localPosition = Vector3.zero;
                        card.SetFaceUp(false);
                        card.IsTopCard = false;
                        card.GetComponent<Collider2D>().enabled = false;
                        card.GetComponent<SpriteRenderer>().sortingOrder = 0;
                        card.transform.SetSiblingIndex(0);
                        card.WasMovedFromWaste = false;

                        temp.Push(card);
                    }
                }

                while (temp.Count > 0)
                    stockPile.Push(temp.Pop());
            }
        }

        public void ResetGame()
        {
            foreach (Transform tableau in tableauPiles)
            {
                for (int i = tableau.childCount - 1; i >= 0; i--)
                {
                    Destroy(tableau.GetChild(i).gameObject);
                }
            }

            foreach (Transform foundation in foundationPiles)
            {
                for (int i = foundation.childCount - 1; i >= 0; i--)
                {
                    Destroy(foundation.GetChild(i).gameObject);
                }
            }

            foreach (Transform child in stockPilePosition)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in wastePilePosition)
            {
                Destroy(child.gameObject);
            }

            deck.Clear();
            stockPile.Clear();
            wastePile.Clear();

            UndoManager.Instance?._undoStack.Clear();

            DeckCreate();
            DeckSuffle();
            DealToTableau();
            SetupStockPile();
        }

        public void PushBackToStock(Card card) => stockPile.Push(card);
    }
}
