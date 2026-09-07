## Why

Now that both the Sword and Bow are implemented, the player needs an inventory and equipment system to manage tools, assign actions, and track consumables. Introducing a classic Zelda-style 3-button assignable item system (X, Y, B) with a dedicated Inventory screen (opened via Select / Tab) and modular equipment components empowers the player to customize their active loadout, seamlessly switch weapons and items, and cleanly scales to future weapons and elemental tools.

## What Changes

- **3-Button Bindable Action System**:
  - `A` is reserved for context actions (Jump, Climb, Interact).
  - `X`, `Y`, and `B` can each be freely assigned any unlocked active item (Sword, Bow, Bombs, etc.).
  - Default initial loadout assigns `X` = Sword, `B` = Bow, and `Y` = Empty.
- **Modular Equipment & Inventory Architecture**:
  - Equipment items are self-contained components (e.g. `SwordItem`, `BowItem`) encapsulating their visual metadata (Name, Description, Icon) and weapon states.
  - `PlayerAttacking` is parameterized with configurable animation names (e.g. `_animationName = "Sword"`) to support reusable melee weapon behaviors.
  - `Inventory` component manages unlocked items, slot assignments (X, Y, B), auto-swapping, and consumable ammo tracking.
- **Dedicated Inventory Menu Screen (`InventoryMenu.tscn`)**:
  - Opened via `Select` / `View` (Xbox) or `Tab` / `I` (Keyboard), distinct from the system `PauseMenu`.
  - Displays a grid of unlocked items with an item details banner and assigned slot badges.
  - Allows the player to navigate items and press `X`, `Y`, or `B` to assign the selected item to that button.
- **HUD Diamond Action Cluster**:
  - Renders a diamond-arranged widget in the HUD showing the active items on `(Y)`, `(X)`, and `(B)`.
  - Displays item icons and dynamic consumable ammo badges.
- **Generic State Item Extension & `IActionTriggerable`**:
  - Decouples player locomotion states from specific items using `StateItemExtensions.TryTriggerItemAction`.
  - Item action states (e.g. `PlayerAiming`) implement `IActionTriggerable` to validate ammo and capture the triggering button generically.

## Capabilities

### New Capabilities
- `player-inventory`: Modular equipment item components, inventory model, slot bindings (X, Y, B), slot swapping, consumable ammo management, and `IActionTriggerable` state integration.
- `inventory-menu`: Dedicated full-screen inventory UI menu (Select / Tab) with item grid navigation, details display, and button assignment shortcuts.

### Modified Capabilities
- `in-game-hud`: Displays the 3-button diamond action cluster with equipped item icons and ammo badges.
- `player-sword-attack`: Parameterizes attack animation name for reusable melee behaviors and triggers dynamically from assigned action slots.
- `player-bow-attack`: Invoked dynamically when activating whichever button slot is assigned to the Bow, integrating with `IActionTriggerable`.

## Impact

- Closes Issue #49 (`Implement In-Game HUD Active Item and Ammo Widget`) and establishes the modular foundation for Issue #42 (Treasure Chests) and Issue #53 (Fire Arrows).
- Decouples `PlayerStanding.cs`, `PlayerRunning.cs`, `PlayerFalling.cs`, `PlayerJumping.cs` action triggering via `StateItemExtensions`.
- Adds `InventoryMenu.tscn` / `InventoryMenu.cs` and integrates `ItemActionCluster` into `Hud.tscn`.
