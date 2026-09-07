## Why

Environmental storytelling, signposts, lore tablets, treasure chests, and NPC conversations are foundational pillars of adventure and Metroidvania titles. Currently, the player character lacks a generic interaction pipeline, and there are no readable environmental objects in the world.

Implementing a pure Godot-native **Interaction System** alongside **Readable Signposts and Dialogue Boxes** introduces the interaction mechanics required across the entire game (signposts, chests, doors, NPCs) while immediately providing interactive in-game signposts with bottom-anchored typewriter dialogue banners.

## What Changes

- **Godot Node-Based Interaction Component (`Interactable2D.tscn` / `Interactable2D.cs`)**:
  - Reusable `Area2D` component placed on any interactive world entity (`Signpost`, `Chest`, `NPC`, etc.).
  - Configurable properties: `[Export] public bool IsActive`, `[Export] public string PromptAction = "Read"`.
  - Emits Godot signals: `Interacted(Player player)`, `PlayerEntered(Player player)`, `PlayerExited(Player player)`.
  - Embeds a floating in-world prompt cue (`PromptCue`) hovering above the object that automatically animates/fades in when a player is within range.

- **Player Interaction Detector (`InteractionDetector2D.tscn` / `InteractionDetector2D.cs`)**:
  - Child `Area2D` attached to `Player.tscn` on a dedicated interaction collision layer.
  - Monitors overlapping `Interactable2D` components, tracks the closest active target, and provides `CanInteract` property.

- **`PlayerInteracting` State (`scripts/entities/player/states/PlayerInteracting.cs`)**:
  - Transitioned to from `PlayerStanding` when pressing `(A)` in range of an active `Interactable2D`.
  - Freezes player horizontal and vertical movement (`Velocity = Vector2.Zero`) and plays the `Idle` animation.
  - Listens for interaction completion signals (e.g. dialogue dismissed) and cleanly transitions back to `PlayerStanding`.

- **Readable Signpost Entity (`Signpost.tscn` / `Signpost.cs`)**:
  - World entity using `assets/textures/objects/Signpost.png`.
  - Configurable message property: `[Export(PropertyHint.MultilineText)] public string Message { get; set; }`.
  - Employs `Interactable2D` child component to trigger dialogue presentation when interacted with.

- **In-Game Dialogue Box (`DialogueBox.tscn` / `DialogueBox.cs`)**:
  - Embedded inside `Hud.tscn` anchored cleanly to the bottom of the viewport.
  - Styled with retro dark translucent backing and clean borders matching the HUD / Pause Menu design system.
  - Fast typewriter text streaming with `(A)` button fast-forward/skip to end of message and `(A)` dismiss/close.
  - Emits `DialogueClosed` signal upon closing to restore player control.

## Capabilities

### New Capabilities
- `interaction-system`: Reusable `Interactable2D` node component, `InteractionDetector2D` player detector, and `PlayerInteracting` state lifecycle.
- `signpost-dialogue`: `Signpost.tscn` environmental entity, multiline message configuration, and bottom-anchored `DialogueBox.tscn` UI with typewriter reveal and dismiss actions.

### Modified Capabilities
- `player-movement`: `PlayerStanding` checks for nearby interactables on `(A)` press and transitions to `PlayerInteracting`.
- `in-game-hud`: Houses the bottom-anchored `DialogueBox` component and manages UI-level dialogue events.

## Impact

- Closes **Issue #6** (`Implement Interaction System and PlayerInteracting State`) and **Issue #65** (`Implement Readable Signposts and Interactive Dialogue Banner System`).
- Adheres strictly to `.agents/rules/godot-node-traits.md` by utilizing concrete Node/Scene components with zero redundant C# interfaces.
- Establishes the reusable interaction foundation for upcoming features: Issue #42 (Treasure Chests), Issue #28 (Save Statues / Checkpoints), and Issue #25 (Dungeon Doors).
