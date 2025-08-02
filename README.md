# Solitaire_card_game
Solitaire game

Assets/
└── Script/
    ├── Core/
    │   ├── Card.cs
    │   ├── ICard.cs
    │   ├── IFlippable.cs
    │   ├── CardData.cs
    ├── DragDrop/
    │   ├── CardDrag.cs
    │   ├── IDraggable.cs
    ├── Managers/
    │   ├── DeckManager.cs
    │   ├── RuleManager.cs
    │   ├── UndoManager.cs
    │   └── GameManager.cs (nếu cần)
    ├── Rules/
    │   ├── IGameRule.cs
    │   ├── TableauRule.cs
    │   ├── FoundationRule.cs
    ├── Undo/
    │   ├── IUndoAction.cs
    │   ├── UndoActionMove.cs
    │   ├── UndoActionFlipStock.cs
    ├── Zones/
    │   ├── IZone.cs
    │   ├── TableauZone.cs
    │   ├── FoundationZone.cs
    ├── UI/
    │   ├── UndoButton.cs
    │   ├── StockPileClick.cs
    └── Utilities/
        ├── SortingOrderHelper.cs
        └── DropZoneDetector.cs
