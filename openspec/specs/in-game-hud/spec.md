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

