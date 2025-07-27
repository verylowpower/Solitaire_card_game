using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using System.Data;

public class DeckManager : MonoBehaviour
{
    public static DeckManager instance;

    public GameObject cardPrefabs;
    public Sprite[] cardSprite;
    public Transform[] tableauPiles;
    public Sprite backSprite;

    public Transform stockPilePosition;
    public Transform wastePilePosition;

    private Stack<Card> stockPile = new Stack<Card>();
    private Stack<Card> wastePile = new Stack<Card>();

    private List<Card> deck = new();

    void Awake()
    {
        instance = this;

        DeckCreate();
        Debug.Log("Tạo xong bộ bài: " + deck.Count); // 52

        DeckSuffle();

        DealToTableau();     // phân 28 lá
        SetupStockPile();    // còn 24 lá

        Debug.Log("Còn lại trong stockPile: " + stockPile.Count); // 24
    }


    public void DeckCreate()
    {
        foreach (Card.Suit suit in System.Enum.GetValues(typeof(Card.Suit)))
        {
            for (int val = 1; val <= 13; val++)
            {
                GameObject cardObj = Instantiate(cardPrefabs, transform);
                if (cardObj == null)
                {
                    Debug.LogError("Không thể instantiate card prefab!");
                    continue;
                }

                Card card = cardObj.GetComponent<Card>();
                if (card == null)
                {
                    Debug.LogError($"Card prefab thiếu script Card! ({suit} {val})");
                    continue;
                }

                card.suit = suit;
                card.cardValue = (Card.CardValue)val;

                int suitIndex = (int)suit;
                int valueIndex = val - 1;
                int spriteIndex = suitIndex * 13 + valueIndex;

                card.frontFace = cardSprite[spriteIndex];
                card.backFace = backSprite;
                card.SetFaceUp(false);

                deck.Add(card);
            }
        }

        Debug.Log("DeckCreate hoàn tất, số lượng bài: " + deck.Count);
    }


    void DeckSuffle()
    {
        for (int i = 0; i < deck.Count - 1; i++)
        {
            int randIndex = Random.Range(0, deck.Count);

            (deck[randIndex], deck[i]) = (deck[i], deck[randIndex]);
        }

        for (int i = 0; i < deck.Count; i++)
        {
            deck[i].GetComponent<SpriteRenderer>().sortingOrder = i;
            deck[i].transform.SetSiblingIndex(i); //doi thu tu trong hierachy
            deck[i].IsTopCard = false;
        }

        deck[deck.Count - 1].GetComponent<Card>().IsTopCard = true;

        Debug.Log("Deck suffled");
    }

    void DealToTableau()
    {
        int deckIndex = 0;

        for (int col = 0; col < 7; col++)
        {
            for (int row = 0; row <= col; row++)
            {
                Card card = deck[deckIndex];
                deckIndex++;

                card.transform.SetParent(tableauPiles[col]);
                card.transform.localPosition = new Vector3(0, -row * 0.2f, 0);

                // Gán sortingOrder theo chiều dọc
                card.GetComponent<SpriteRenderer>().sortingOrder = row;

                bool isLastCard = (row == col);
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

            card.transform.SetParent(wastePilePosition);
            card.transform.localPosition = Vector3.zero;
            card.SetFaceUp(true);
            card.IsTopCard = true;
            card.GetComponent<Collider2D>().enabled = true;


            int highestOrder = -1;
            foreach (Transform child in wastePilePosition)
            {
                SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
                if (sr != null && sr.sortingOrder > highestOrder)
                    highestOrder = sr.sortingOrder;
            }
            card.GetComponent<SpriteRenderer>().sortingOrder = highestOrder + 1;


            card.transform.SetSiblingIndex(wastePilePosition.childCount - 1);

            wastePile.Push(card);
        }
        else
        {
            // Reset từ waste về stock
            Stack<Card> temp = new Stack<Card>();

            while (wastePile.Count > 0)
            {
                Card card = wastePile.Pop();

                if (card.wasMovedFromWaste)
                    continue;

                card.transform.SetParent(stockPilePosition);
                card.transform.localPosition = Vector3.zero;

                card.SetFaceUp(false);
                card.IsTopCard = false;
                card.GetComponent<Collider2D>().enabled = false;

                card.GetComponent<SpriteRenderer>().sortingOrder = 0;
                card.transform.SetSiblingIndex(0);

                temp.Push(card);
            }

            // Đưa các lá đủ điều kiện trở lại stack
            while (temp.Count > 0)
            {
                stockPile.Push(temp.Pop());
            }

        }
    }





}