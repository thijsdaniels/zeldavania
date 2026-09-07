# Interaction and Dialogue System Design

## Overview
This design implements a pure Godot-native, component-based interaction and dialogue pipeline. It introduces:
1. **`Interactable2D`**: Reusable component attached to entities with built-in floating prompt cues.
2. **`InteractionDetector2D`**: Attached to `Player.tscn` to detect and prioritize nearby interactables.
3. **`PlayerInteracting`**: A dedicated player state that locks character movement and manages interaction lifecycle.
4. **`Signpost`**: An environmental object scene using `Signpost.png` and multiline text.
5. **`DialogueBox`**: A bottom-anchored HUD dialogue banner with typewriter streaming, skip-to-end, and dismiss handling.

---

## 1. Node Hierarchy & Component Architecture

### Interactable2D (`scenes/utilities/Interactable2D.tscn`)
```
Interactable2D (Area2D)
├── CollisionShape2D
└── PromptCue (Node2D)
     ├── PanelContainer / Sprite2D (Button badge icon: "[A]")
     └── AnimationPlayer (Floating bounce / fade in-out)
```

- **Collision Layer**: Layer 10 (Interaction trigger layer).
- **Collision Mask**: 0 (Does not monitor physics bodies; monitored by `InteractionDetector2D`).
- **Exported Properties**:
  - `[Export] public bool IsActive { get; set; } = true;`
  - `[Export] public string PromptAction { get; set; } = "Read";`
  - `[Export] public Vector2 PromptOffset { get; set; } = new Vector2(0, -20);`
- **Signals**:
  - `[Signal] public delegate void InteractedEventHandler(Player player);`
  - `[Signal] public delegate void PlayerEnteredEventHandler(Player player);`
  - `[Signal] public delegate void PlayerExitedEventHandler(Player player);`

---

### Player Integration (`scenes/entities/Player.tscn`)
```
Player (CharacterBody2D)
├── ...
├── InteractionDetector2D (Area2D)
│    └── CollisionShape2D (Size: ~16x20)
└── State (FiniteStateMachine)
     ├── Standing (PlayerStanding)
     └── Interacting (PlayerInteracting)
```

- **`InteractionDetector2D`**:
  - **Collision Layer**: 0.
  - **Collision Mask**: Layer 10 (Detects `Interactable2D`).
  - **Methods / Properties**:
    - `public Interactable2D CurrentInteractable { get; }`
    - `public bool CanInteract => CurrentInteractable != null && CurrentInteractable.IsActive;`

- **`PlayerStanding.cs` Integration**:
  ```csharp
  case true when _interactionDetector != null 
             && _interactionDetector.CanInteract 
             && Input.IsActionJustPressed(Controller.A):
      _interactionDetector.CurrentInteractable.Interact(_player);
      Transition(_interactingState);
      break;
  ```

- **`PlayerInteracting.cs`**:
  - Sets `_body.Velocity = Vector2.Zero`.
  - Plays `_sprite.Play("Idle")`.
  - Listens for interaction conclusion: `OnInteractionEnded()` transitions to `_standingState`.

---

### Signpost Entity (`scenes/objects/Signpost.tscn`)
```
Signpost (Node2D)
├── Sprite2D (assets/textures/objects/Signpost.png)
├── StaticBody2D (Collision for player obstruction / solid base)
│    └── CollisionShape2D
└── Interactable2D (instance of scenes/utilities/Interactable2D.tscn)
     └── CollisionShape2D
```

- **Script (`Signpost.cs`)**:
  - `[Export(PropertyHint.MultilineText)] public string Message { get; set; } = "Default signpost message.";`
  - On `_Ready()`: Connects `_interactable.Interacted += HandleInteracted;`
  - `HandleInteracted(Player player)`: Queries `Hud` or `DialogueBox` to open the message.

---

### Dialogue Box UI (`scenes/userInterface/DialogueBox.tscn`)
```
DialogueBox (Control / CanvasLayer)
└── MarginContainer (Bottom-anchored)
     └── PanelContainer (Dark translucent 9-slice / FlatBox with gold/white border)
          └── MarginContainer
               └── VBoxContainer
                    ├── RichTextLabel (Message text display)
                    └── HBoxContainer (Bottom bar)
                         ├── Spacer
                         └── ContinuePrompt (Label: "(A) Continue" / "(A) Close")
```

- **Script (`DialogueBox.cs`)**:
  - `public void Open(string text, Action onClosed = null)`
  - `public void Close()`
  - State machine / flags: `IsRevealing`, `IsComplete`, `IsOpen`.
  - Typewriter reveal using a character timer / `VisibleCharacters` interpolation.
  - Input Handling:
    - If `IsRevealing` and `Input.IsActionJustPressed(Controller.A)` -> set `VisibleCharacters = -1` (instant complete).
    - If `IsComplete` and `Input.IsActionJustPressed(Controller.A)` -> call `Close()`.
  - Signal: `[Signal] public delegate void DialogueClosedEventHandler();`

---

## 2. Collision Matrix Updates

| Layer Name | Layer Bit | Usage |
| :--- | :--- | :--- |
| **Interaction** | **Layer 10 (Bitmask 512)** | Dedicated layer for `Interactable2D` and `InteractionDetector2D` to keep interaction isolated from solid world and combat hitboxes. |
