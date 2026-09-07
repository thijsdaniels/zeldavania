## Purpose

Manages the player's inventory data, item unlock state, assignable action button slots (X, Y, B), slot swapping logic, and consumable ammo resources.

## ADDED Requirements

### Requirement: Item Registration & Definitions
The inventory system SHALL maintain item definitions containing item identifiers, display names, descriptions, icons, and whether the item consumes ammo.

#### Scenario: Querying item definition
- **WHEN** an item identifier is requested from the inventory system
- **THEN** the system returns its associated display name, description, icon resource, and consumable type.

### Requirement: Action Slot Assignment & Swapping
The inventory system SHALL manage three assignable action slots (`Slot.X`, `Slot.Y`, `Slot.B`) and support assigning and swapping items between slots.

#### Scenario: Assigning an unequipped item to a slot
- **WHEN** the player assigns an unlocked item to an empty slot or a slot holding a different unassigned item
- **THEN** the target slot is updated to hold the selected item.

#### Scenario: Assigning an already-equipped item swaps slots
- **GIVEN** Item A is assigned to Slot X and Item B is assigned to Slot Y
- **WHEN** the player assigns Item A to Slot Y
- **THEN** Slot Y receives Item A and Slot X receives Item B.

#### Scenario: Default initial loadout
- **WHEN** a new game session or player inventory is initialized
- **THEN** Slot X is assigned the Sword, Slot B is assigned the Bow, and Slot Y is empty (None).

### Requirement: Consumable Ammo Tracking
The inventory system SHALL track current and maximum ammo quantities for consumable items (such as arrows).

#### Scenario: Consuming ammo on item use
- **GIVEN** the player has 15 arrows and fires the bow
- **WHEN** the arrow projectile is fired
- **THEN** the arrow ammo count decreases to 14.

#### Scenario: Out of ammo prevents item usage
- **GIVEN** the player has 0 arrows and attempts to shoot the bow
- **WHEN** the shoot action is triggered
- **THEN** no arrow is fired and an empty-ammo feedback event is triggered.

#### Scenario: Replenishing ammo from pickups
- **GIVEN** the player has 10 arrows with a maximum capacity of 30
- **WHEN** the player collects an arrow refill of 5 arrows
- **THEN** the arrow ammo count increases to 15 without exceeding the maximum capacity of 30.
