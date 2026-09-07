# In-Game HUD Specification

## Purpose

Provides an in-game heads-up display (HUD) overlay that renders real-time player status information, including current and maximum health visualized via a pixel-art health bar.

## Requirements

### Requirement: Health Bar Display
The HUD SHALL visualize the player's health using a pixel-art progress bar representing current hit points out of maximum hit points.

#### Scenario: Rendering full health bar at maximum health
- **GIVEN** the player has a maximum health of 12 hit points
- **WHEN** current health is equal to maximum health (12 HP)
- **THEN** the HUD displays the health bar at 100% full fill.

#### Scenario: Rendering partial health bar on damage
- **GIVEN** the player currently has 12 hit points
- **WHEN** the player takes damage reducing health to 6 HP
- **THEN** the HUD displays the health bar at 50% fill.

#### Scenario: Rendering empty health bar when depleted
- **GIVEN** the player has a maximum of 12 hit points
- **WHEN** the player takes lethal damage reducing current health to 0 HP
- **THEN** the HUD displays the health bar with 0% fill.

### Requirement: Real-Time Health Synchronization
The HUD SHALL update its visual health display in real time in response to player health changes.

#### Scenario: Health display updates immediately on taking damage
- **GIVEN** the HUD is displayed and connected to the player's health signals
- **WHEN** the player takes damage and emits a health change signal
- **THEN** the health bar immediately updates to reflect the new current hit points.

#### Scenario: Health display updates immediately on healing
- **GIVEN** the player is below maximum health
- **WHEN** the player receives healing and emits a health change signal
- **THEN** the health bar immediately updates to reflect the increased hit points.

#### Scenario: Health display updates on player defeat and reset
- **GIVEN** the player takes lethal damage and resets to spawn with full health
- **WHEN** the health reset signal is emitted
- **THEN** the health bar in the HUD is fully restored to 100% fill.

### Requirement: In-Game Dialogue Box Display
The HUD SHALL house a bottom-anchored dialogue box component capable of displaying interactive messages and streaming typewriter text.

#### Scenario: Displaying dialogue in HUD
- **GIVEN** an active dialogue message is triggered
- **WHEN** the HUD receives a request to show dialogue
- **THEN** the dialogue box becomes visible at the bottom of the viewport
- **AND** reveals text progressively with a button prompt to advance or close.

### Requirement: Active Item Action Cluster Display
The HUD SHALL display a diamond-arranged action slot widget showing the equipped items and button prompts for slots X, Y, and B.

#### Scenario: Rendering equipped item icons
- **GIVEN** Slot X has Sword, Slot B has Bow, and Slot Y is empty
- **WHEN** the HUD is displayed
- **THEN** Slot X renders the sword icon with an "X" button prompt, Slot B renders the bow icon with a "B" button prompt, and Slot Y renders an empty slot frame with a "Y" button prompt.

#### Scenario: Updating slot icons on inventory change
- **GIVEN** the player changes their slot assignments in the inventory
- **WHEN** an item assignment event is received by the HUD
- **THEN** the corresponding slot widget updates its icon and prompt immediately.

### Requirement: Active Consumable Ammo Counter Widget
The HUD SHALL display current consumable ammo counts for equipped consumable items.

#### Scenario: Displaying arrow count on equipped bow
- **GIVEN** the Bow is assigned to an active slot (e.g. Slot B) and the player has 20 arrows
- **WHEN** the HUD renders the active item slot
- **THEN** an ammo count badge showing "20" is displayed adjacent to the bow icon.

#### Scenario: Updating ammo count on consumption
- **GIVEN** the Bow is equipped and arrow ammo changes from 20 to 19
- **WHEN** an ammo change signal is received
- **THEN** the ammo count badge updates dynamically to "19".

#### Scenario: Non-consumable items display no ammo badge
- **GIVEN** the Sword is assigned to Slot X
- **WHEN** the HUD renders Slot X
- **THEN** no ammo count badge is displayed for the sword.


