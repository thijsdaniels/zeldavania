## 1. Asset Preparation & Equipment Core Model

- [x] 1.1 Import and convert item icons (`Sword.png`, `Bow.png`, `Bomb.png`, `Boomerang.png`, `ActionSlotFrame.png`) from legacy assets into `assets/textures/objects/items/` and `assets/textures/userInterface/` following project PascalCase conventions.
- [x] 1.2 Create `EquipmentItem` base component (`scripts/inventory/EquipmentItem.cs`), `ActionSlot` enum, and specialized item components (`SwordItem`, `BowItem`) encapsulating metadata (ItemName, Description, Icon, Ammo) and target states.
- [x] 1.3 Implement `Inventory` component (`scripts/inventory/Inventory.cs`) managing registered equipment items, active slot bindings (`Slot.X`, `Slot.Y`, `Slot.B`), auto-swapping logic, and consumable ammo tracking with event signals (`OnItemAssigned`, `OnAmmoChanged`).

## 2. State Decoupling & Parameterized Melee Attack

- [x] 2.1 Parameterize `PlayerAttacking` with `[Export] string _animationName = "Sword"` so it dynamically plays configured animations and handles melee swing lifecycles generically.
- [x] 2.2 Define `IActionTriggerable` interface and update `PlayerAiming` to implement it, validating arrow ammo and capturing the triggering button dynamically.
- [x] 2.3 Create `StateItemExtensions` (`scripts/extensions/State/ItemActions.cs`) providing generic `TryTriggerItemAction(this State, Player, out State)` logic.
- [x] 2.4 Refactor `PlayerStanding`, `PlayerRunning`, `PlayerFalling`, and `PlayerJumping` to route item action transitions through `this.TryTriggerItemAction(_player, out targetState)` and verify zero hardcoded item checks.

## 3. In-Game HUD Action Cluster & Ammo Display

- [x] 3.1 Create `ItemActionSlot` and diamond-arranged `ItemActionCluster` UI scenes and scripts in `scenes/userInterface/` with button prompts and item icon displays.
- [x] 3.2 Update `Hud.cs` and `Hud.tscn` to bind to `Inventory` signals, rendering active item icons and consumable ammo badges in real time.

## 4. Dedicated Inventory Menu Screen

- [x] 4.1 Create `InventoryMenu.tscn` and `InventoryMenu.cs` in `scenes/userInterface/` triggered via `Select` (Xbox) and `Tab` / `I` (Keyboard).
- [x] 4.2 Implement grid cursor navigation, item details banner (name & description), and X/Y/B assignment shortcuts with auto-swapping in `InventoryMenu.cs`.

## 5. Scene Integration & Verification

- [x] 5.1 Structure `Equipment` / `Inventory` hierarchy on `Player.tscn` with default loadout (X: Sword, B: Bow, Y: None), connect to `Hud.tscn` and `InventoryMenu.tscn`, and map `Select` / `Tab` input actions in `project.godot`.
- [x] 5.2 Build project using `dotnet build` and verify that all inventory assignments, state transitions, HUD updates, and inventory menu interactions function seamlessly.
