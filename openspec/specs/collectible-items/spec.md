# Collectible Items Specification

## Purpose

Defines the lifecycle, physical behavior, and polymorphic collection rules for pickup items in the game world.

## Requirements

### Requirement: Collectible Physics & Lifecycle
The collectible entity SHALL support a physical spawn pop velocity, settling on surfaces with a gentle hover bobbing animation, and an optional lifetime timeout with blinking visual cues before despawning.

#### Scenario: Spawning into the world
- **WHEN** a collectible is spawned into the scene
- **THEN** it launches upward in an arc and lands on terrain surfaces before entering a floating idle state.

#### Scenario: Lifetime expiration
- **GIVEN** a collectible configured with a lifetime duration
- **WHEN** the remaining lifetime drops below the blink threshold
- **THEN** the collectible blinks rapidly and removes itself from the scene upon timer expiration.

### Requirement: Polymorphic Collection & Audio Feedback
The collectible entity SHALL detect player contact, execute its specific reward logic on the collector, play a pickup sound effect (`collect.wav`), and remove itself from the scene.

#### Scenario: Successful collection
- **WHEN** a player character overlaps with an active collectible
- **THEN** the collectible executes its reward effect, plays the pickup sound effect, and frees itself.

### Requirement: Arrow Ammo Collectible Tiers
The arrow collectible SHALL support multiple distinct ammo quantities (10, 20, 30 arrows) corresponding to specific visual sprite frames and replenish the player's bow ammunition up to the maximum capacity.

#### Scenario: Collecting 10-arrow bundle
- **GIVEN** a player with 15 out of 30 arrows touches a 10-arrow collectible
- **WHEN** collection occurs
- **THEN** the player's bow ammo increases to 25.

#### Scenario: Collecting arrow bundle at or near capacity
- **GIVEN** a player with 28 out of 30 arrows touches a 10-arrow collectible
- **WHEN** collection occurs
- **THEN** the player's bow ammo is capped at 30 and the collectible is successfully consumed.

### Requirement: Heart Health Collectible
The heart collectible SHALL replenish 4 hit points (1 full heart container) on the collector's health component without exceeding maximum health.

#### Scenario: Collecting heart while damaged
- **GIVEN** a player with 6 out of 12 HP touches a heart collectible
- **WHEN** collection occurs
- **THEN** the player's current HP increases to 10 and the health bar updates.

#### Scenario: Collecting heart at full health
- **GIVEN** a player with 12 out of 12 HP touches a heart collectible
- **WHEN** collection occurs
- **THEN** current HP remains 12 and the collectible is successfully consumed.
