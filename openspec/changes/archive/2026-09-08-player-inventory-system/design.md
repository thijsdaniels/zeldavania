## Context

See [proposal.md](proposal.md) for motivation. Currently, `PlayerStanding.cs`, `PlayerRunning.cs`, `PlayerFalling.cs`, and `PlayerJumping.cs` hardcode `Input.IsActionJustPressed(Controller.X)` to transition to `PlayerAttacking` (Sword) and `Input.IsActionJustPressed(Controller.B)` to transition to `PlayerAiming` (Bow). The HUD only displays the health bar, and system options are in `PauseMenu`.

## Goals / Non-Goals

**Goals:**
- Implement a modular `Inventory` and equipment component hierarchy under `Player` where equipment items (e.g. `SwordItem`, `BowItem`) encapsulate their own metadata (`ItemName`, `Description`, `Icon`), ammo, and states.
- Parameterize `PlayerAttacking` (`[Export] string _animationName = "Sword"`) to make it a generic, reusable melee attack state across multiple weapons.
- Implement a dedicated `InventoryMenu.tscn` / `InventoryMenu.cs` (opened via Select / Tab) with item grid navigation, details display, and X/Y/B assignment shortcuts with auto-swapping.
- Implement a diamond-arranged active item cluster widget in `Hud.tscn` displaying slot icons and consumable ammo counters.
- Decouple player movement states from item specifics via a generic `StateItemExtensions.TryTriggerItemAction` extension method and `IActionTriggerable` state interface, keeping the FSM 100% pure.

**Non-Goals:**
- Fire arrow / elemental arrow sub-type selector (deferred to Issue #53).
- Treasure chest fanfare / world acquisition sequence (deferred to Issue #42 / Issue #57).
- Save/load serialization of inventory state to disk (deferred to Issue #30).

## Architecture & Scene Tree

```
Player (CharacterBody2D)
├── AnimatedSprite2D
├── Hurtbox / Damageable
├── FiniteStateMachine (Core Locomotion)
│   ├── Standing
│   ├── Running
│   ├── Falling
│   ├── Jumping
│   └── Hurt
│
└── Equipment (Node / Inventory)
    ├── SwordItem (EquipmentItem)
    │   ├── [Export] ItemName = "Kokiri Sword"
    │   ├── [Export] Icon = sword_icon.png
    │   └── State: PlayerAttacking (Hitbox, swing audio, [Export] AnimationName = "Sword")
    │
    └── BowItem (EquipmentItem)
        ├── [Export] ItemName = "Fairy Bow"
        ├── [Export] Icon = bow_icon.png
        ├── [Export] ArrowAmmo = 30
        ├── State: PlayerAiming (Crosshair, bullet-time, aiming math)
        └── State: PlayerShooting (Spawns Arrow.tscn)
```

## Decisions

### 1. Self-Contained Equipment Components
- **Decision**: Each equippable weapon/tool is a component node (e.g. `EquipmentItem`) that encapsulates its UI metadata (Name, Description, Icon), ammo properties, and action states.
- **Rationale**: Keeps all code, assets, and state references for an item isolated in one place without bloating the core player character node.

### 2. Parameterized Melee Attack State
- **Decision**: Export `_animationName` on `PlayerAttacking.cs` (defaulting to `"Sword"`).
- **Rationale**: Allows `PlayerAttacking` to be reused across future melee weapons (e.g. Greatsword, Hammer) by simply assigning different animation names, audio, and hitbox parameters in the Godot inspector.

### 3. Dedicated `InventoryMenu` vs. System `PauseMenu`
- **Decision**: Separate the menus:
  - `Start` (Xbox) / `Escape` (Keyboard) $\rightarrow$ `PauseMenu` (Resume, Restart, Options, Quit).
  - `Select` (Xbox) / `Tab` or `I` (Keyboard) $\rightarrow$ `InventoryMenu` (Item Grid, Descriptions, X/Y/B Equip slots).
- **Rationale**: Clean separation of system pause functions from gameplay lore and equipment management.

### 4. Pure FSM Action Dispatch via `StateItemExtensions` & `IActionTriggerable`
- **Decision**:
  - Item action states (e.g. `PlayerAiming`) implement `IActionTriggerable`:
    ```csharp
    public interface IActionTriggerable
    {
        bool TryInitialize(Player player, string actionButton);
    }
    ```
  - Movement states use a generic extension method:
    ```csharp
    public static bool TryTriggerItemAction(this State state, Player player, out State targetState);
    ```
- **Rationale**: Keeps movement states (`PlayerStanding`, `PlayerRunning`, `PlayerFalling`, `PlayerJumping`) completely agnostic to specific weapons/tools while ensuring the active state remains the sole authority calling `Transition(targetState)`.

### 5. Slot Assignment & Auto-Swapping
- **Decision**: 3 discrete slots: `ActionSlot.X`, `ActionSlot.Y`, and `ActionSlot.B`.
- **Behavior**: If assigning Item A to a slot while Item A is already on another slot, the two slots swap values cleanly.

### 6. HUD Diamond Action Cluster
- **Decision**: A custom widget in `Hud.tscn` (top-right corner) with 3 diamond-arranged slots (Top: Y, Left: X, Right: B) showing item icons, button prompts, and ammo count badges.

## Risks / Trade-offs

- **[Risk]** Pausing interaction conflicts between PauseMenu and InventoryMenu.
  - **Mitigation**: Both menus check if the other is open and ignore toggle inputs if another pause layer is active.
- **[Risk]** Out-of-ammo feedback on quick-tap shooting.
  - **Mitigation**: `IActionTriggerable.TryInitialize` in `PlayerAiming` returns `false` when arrows $\le 0$, allowing the player to remain in locomotion without getting stuck in aiming.
