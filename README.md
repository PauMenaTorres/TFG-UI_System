# 🧩 ModularUI
<br>
<p align="center"><img width="704" height="384" alt="Logo" src="https://github.com/user-attachments/assets/82ef5dff-0266-4a92-abe4-05b78a761a12" />
<br></br>

**An adaptive, cross-platform UI framework for Unity.**

ModularUI is a highly modular, scalable, and automated User Interface system designed to dynamically adapt to different platforms (Desktop, Mobile, VR) and game genres. With a focus on clean architecture, it provides developers with ready-to-use prefabs, dynamic theming, and fully constructed scenes.

---

## 🎬 Showcase Scenes

The best way to understand ModularUI is to experience it. The `Samples/` folder contains fully functional scenes that demonstrate the framework's capabilities out of the box:

* **Demo_Scene:** A complete showcase combining all the framework's systems.
* **MainMenu_Scene:** An advanced main menu flow ready for production.
* **HUD_Scene:** In-game overlays, health bars, and minimap integration.
* **Inventory_Scene:** Complete drag-and-drop inventory mechanics.
* **Dialogue_Scene:** Node-based dialogue parsing and display.

<br>
<p align="center"><img width="581" height="320" alt="MainMenu (online-video-cutter com)" src="https://github.com/user-attachments/assets/b1cd1be6-3c63-49e9-ad92-a7e6647905b1" />
<br>
<p align="center"><img width="576" height="324" alt="Inventory" src="https://github.com/user-attachments/assets/918b1ec3-f4a1-4b05-8d5c-d77f9acf9bab" />


---

## 🎨 Dynamic Theming & Genre Adaptation

ModularUI uses a centralized UI Configuration system. Depending on your game's genre (e.g., RPG, Sci-Fi Shooter, Minimalist Puzzle), you can swap the current Theme, and **all UI components in your project will update instantly**. 

* **Theme Data:** Colors, fonts, typography, and default sprites are controlled via Theme `ScriptableObjects`.
* **Live Updates:** Modifying a Theme asset or changing the active configuration immediately refreshes all modular components in the Editor, without needing to enter Play Mode.
<br>
<p align="center"><img width="1056" height="404" alt="GenreChanging" src="https://github.com/user-attachments/assets/0d75a7a4-c3ae-4655-85b8-c6f18a105210" />

---

## 💬 Advanced Dialogue System & Action Binding

The framework features a powerful, data-driven narrative module designed for seamless iteration:

* **Rich Text & Formatting:** Fully supports line breaks and dynamic text formatting directly from the data source.
* **Zero-Lookup Action Binding:** Link dialogue choices to game events effortlessly. You only need to type the target function's name as a string inside the Dialogue `ScriptableObject`.
* **Automatic Inspector Mapping:** The `ModularDialogueManager` editor automatically detects and binds the function to its corresponding logic without requiring any manual searching or complex setup.

<p align="center"><img width="684" height="376" alt="Captura de pantalla 2026-06-01 155240" src="https://github.com/user-attachments/assets/8f233e33-21a7-46f9-8017-8bec9543a5dc" />
<br>
<p align="center"><img width="572" height="265" alt="Captura de pantalla 2026-06-01 160334" src="https://github.com/user-attachments/assets/d1fdc60d-19bb-4853-bafe-fdd4d6f23e99" />
<br>
<p align="center"><img width="636" height="360" alt="Dialogue" src="https://github.com/user-attachments/assets/af51acdf-d37c-4a68-805c-5a9603b65fd6" />

---

## 🛠️ Editor Tools & Hierarchy Integration

ModularUI integrates seamlessly into the Unity Editor. You can build complex interfaces directly from the Hierarchy window with a simple right-click:

### Context Menu Creation
* **Full Templates (Recommended):** Navigate to `Modular UI > Templates` to instantly spawn fully constructed, platform-specific menus. Choose your target platform (`Desktop`, `Mobile`, or `VR`) and add a ready-to-use `Main Menu`, `Pause Menu`, `HUD`, or `Dialogue Panel`. 
* **Base Components:** If you need to build from scratch, go to `Modular UI > Base UI` to spawn individual elements like `Modular Canvas`, `Button`, `Text`, or `Image`. *(Note: The `Templates` menu contains the most up-to-date and fully adapted variants).*

### The Setup Wizard
When importing the package, the **ModularUI Wizard** appears automatically to handle initial settings. If needed, it can be re-accessed at any time via the top toolbar to instantly switch global configurations and platform presets.

<br>
<p align="center"><img width="259" height="467" alt="image" src="https://github.com/user-attachments/assets/1113c3fb-6f16-40dc-9638-031c78f0071c" /></p>

---

## 📦 Data-Driven Design (Scriptable Objects)

The framework relies heavily on `ScriptableObjects` to keep data completely decoupled from logic:
* **Themes (`ModularThemeData`):** Stores palettes, font assets, and UI styling for different game genres.
* **Dialogues (`DialogueNode` / `DialogueTest`):** Manages text nodes, choices, line breaks, and event strings.
* **Inventory (`ItemData`):** Defines item properties (icons, stats, descriptions) for the drag-and-drop inventory sub-system.

---

## 📂 Project Structure

The project is strictly organized to separate logic, assets, and platform-specific data:

* `BaseUI/` - Core abstract prefabs and base component structures.
* `Dialogues/` - Logical assets and data for the dialogue system.
* `Editor/` - Custom inspectors, layout fitters, and the Setup Wizard.
* `Minimap/` - Standalone minimap integration and render textures.
* `Resources/` - Pre-configured genre themes (RPG, FPS, MOBA, Strategy, etc.) and item assets.
* `Runtime/` - Core execution logic, abstract platform adapters, controllers, and screens.
* `Samples/` - Showcase demo scenes and environment testing scripts.
* `Settings/` - Global framework configurations (`UIConfiguration`).
* `Templates/` - Pre-built, platform-specific menus (`Base`, `Desktop`, `Mobile`, `VR`) that automatically adapt to your active Theme.

---
