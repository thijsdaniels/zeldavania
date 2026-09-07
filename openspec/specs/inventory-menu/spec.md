# Inventory Menu Specification

## Purpose

Provides a dedicated in-game inventory screen opened via Select / Tab, displaying unlocked items in a grid, detailed item descriptions, and allowing assignment of items to action buttons X, Y, and B with auto-swapping.

## Requirements

### Requirement: Inventory Menu Toggling
The inventory menu SHALL toggle game pause and animate transitions in response to dedicated inventory inputs (Xbox Select / View, Keyboard Tab / I).

#### Scenario: Opening inventory menu
- **GIVEN** the game is unpaused and the player is in gameplay
- **WHEN** the inventory input action is pressed
- **THEN** the scene tree pause state (`GetTree().Paused`) is set to true
- **AND** the inventory menu becomes visible and focused.

#### Scenario: Closing inventory menu via toggle
- **GIVEN** the inventory menu is open
- **WHEN** the inventory input action (or Cancel / Start) is pressed
- **THEN** the inventory menu hides and gameplay unpauses.

### Requirement: Item Grid & Details Display
The inventory menu SHALL render a grid of unlocked items and display the focused item's name and description.

#### Scenario: Displaying unlocked items
- **GIVEN** the player has unlocked the Sword and the Bow
- **WHEN** viewing the inventory menu
- **THEN** the grid displays slots for the Sword and Bow with their icons.

#### Scenario: Updating item details banner on cursor navigation
- **GIVEN** the inventory grid is open
- **WHEN** the player navigates the cursor to focus on the Bow
- **THEN** the details banner displays the Bow's display name and description.

### Requirement: Action Button Assignment & Swapping
The inventory menu SHALL allow assigning the focused item to action buttons X, Y, or B, swapping slots if the item is already equipped.

#### Scenario: Assigning focused item to slot
- **GIVEN** the cursor is focused on the Bow
- **WHEN** the player presses the "X" button
- **THEN** the Bow is assigned to Slot X
- **AND** the assigned slot indicators update to reflect the new assignment.

#### Scenario: Swapping already-assigned item
- **GIVEN** the Sword is on Slot X and the Bow is on Slot B
- **WHEN** the player focuses on the Bow and presses "X"
- **THEN** the Bow is assigned to Slot X and the Sword is assigned to Slot B.
