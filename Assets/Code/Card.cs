using DG.Tweening;
using UnityEngine;

public class Card : MonoBehaviour
{
    public static Card instance;

    public enum Suit { Heart = 0, Diamond = 1, Spade = 2, Club = 3 }
    public enum CardValue
    {
        Ace = 1, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6,
        Seven = 7, Eight = 8, Nine = 9, Ten = 10, Jack = 11, Queen = 12, King = 13
    }

    public Suit suit;
    public CardValue cardValue;
    public Sprite frontFace;
    public Sprite backFace;
    public bool isFaceUp = false;

    private SpriteRenderer spriteRenderer;
    private Collider2D cardCollider2D;
    public bool wasMovedFromWaste = false;

    public bool _isTopCard = false;
    public bool IsTopCard
    {
        get => _isTopCard;
        set
        {
            _isTopCard = value;
            UpdateColliderState();
        }
    }

    void Awake()
    {
        instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
        cardCollider2D = GetComponent<Collider2D>();

        if (spriteRenderer == null)
            Debug.LogError("Thiếu SpriteRenderer!");
        if (cardCollider2D == null)
            Debug.LogError("Thiếu Collider2D!");
    }

    void Start()
    {
        SetFaceUp(isFaceUp);
    }

    public void SetFaceUp(bool faceUp)
    {
        isFaceUp = faceUp;
        spriteRenderer.sprite = isFaceUp ? frontFace : backFace;
        UpdateColliderState();
    }

    private void UpdateColliderState()
    {
        if (cardCollider2D != null)
            cardCollider2D.enabled = IsTopCard || isFaceUp;
    }

    // public void FlipCard()
    // {
    //     if (isFaceUp) return;

    //     isFaceUp = true;
    //     transform.DOScaleX(0, 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
    //     {
    //         spriteRenderer.sprite = frontFace;
    //         transform.DOScaleX(1, 0.1f).SetEase(Ease.OutQuad);
    //     });
    // }

    public void FlipCard()
    {
        if (isFaceUp) return;

        isFaceUp = true;
        spriteRenderer.sprite = frontFace;
        transform.localScale = new Vector3(1, 1, 1); // Đảm bảo scale chuẩn
        UpdateColliderState();
    }


}

