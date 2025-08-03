Assets/
└── Script/
├── Core/
│ ├── Card.cs
│ ├── ICard.cs
│ ├── IFlippable.cs
│ └── CardData.cs
│
├── DragDrop/
│ ├── CardDrag.cs
│ ├── IDraggable.cs
│ └── Helper/
│     ├── DragStackHelper.cs
│     ├── DropZoneDetector.cs
│     ├── PositionHelper.cs
│     └── SortingOrderHelper.cs
│
├── Managers/
│ ├── DeckManager.cs
│ ├── RuleManager.cs │  
│ └── UndoManager.cs
│
├── Rules/
│ ├── IGameRule.cs
│ ├── TableauRule.cs
│ └── FoundationRule.cs
│
├── Undo/
│ ├── IUndoAction.cs
│ ├── UndoActionMove.cs
│ └── UndoActionFlipStock.cs
│
├── Zones/
│ ├── IZone.cs
│ ├── TableauZone.cs
│ ├── FoundationZone.cs
│ └── DropZoneHelper.cs
│
├── UI/
│ ├── UndoButton.cs
│ ├── StockPileClick.cs
│ └── TableauDropZoneUpdater.cs
