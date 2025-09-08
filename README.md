# 🃏 Solitaire Card Game (Unity)

Một phiên bản **Solitaire (Klondike)** được phát triển bằng **Unity** với đầy đủ các tính năng cơ bản như kéo thả lá bài, hệ thống undo, và xử lý luật chơi chuẩn.

## ✨ Tính năng chính

* 🎴 **Kéo thả stack**: Cho phép kéo nhiều lá bài cùng lúc trong tableau.
* 🔄 **Undo**: Hệ thống hoàn tác để đưa lá bài hoặc stack trở về vị trí ban đầu.
* 🃏 **Stock & Waste**: Hỗ trợ bốc bài từ stock pile sang waste pile theo luật chuẩn.
* 🏆 **Foundation**: Xếp bài theo chất từ A đến K để hoàn thành trò chơi.
* 👆 **Tương tác trực quan**: Hỗ trợ drag & drop với highlight dropzone.
* ⚡ **Hiệu năng**: Cấu trúc code tách theo **SOLID principle** dễ mở rộng.

## 📂 Cấu trúc thư mục

```
Assets/
└── Script/
    ├── Core/              # Card, CardData, Interface cơ bản
    ├── DragDrop/          # Xử lý kéo thả
    ├── Managers/          # DeckManager, RuleManager, UndoManager
    ├── Rules/             # Luật cho Tableau & Foundation
    ├── Undo/              # Hệ thống Undo
    ├── Zones/             # Các Zone (Tableau, Foundation, Stock, Waste)
    ├── UI/                # Nút bấm, UI event
    └── Utilities/         # Helper (Sorting, DropZoneDetector,...)
```

## 🎮 Cách chơi

1. **Bắt đầu game**: Các lá bài được chia theo luật Solitaire.
2. **Di chuyển bài**:

   * Kéo bài (hoặc stack) sang **Tableau**, **Foundation** nếu hợp lệ.
   * Bốc bài từ **Stock** sang **Waste**.
3. **Undo**: Nhấn nút Undo để hoàn tác thao tác vừa rồi.
4. **Thắng game**: Khi tất cả bài được xếp vào **Foundation** theo thứ tự A → K.

## 🛠️ Công nghệ

* **Unity 2022+**
* **C#**

## 🚀 Hướng phát triển

* Thêm hệ thống **hint** (gợi ý nước đi).
* Chế độ **auto-complete** khi gần thắng.
* Tùy chọn **1-card draw / 3-card draw**.
* Hiệu ứng hình ảnh & âm thanh.
