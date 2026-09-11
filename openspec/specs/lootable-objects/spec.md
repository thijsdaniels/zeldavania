# Lootable Objects Specification

## Purpose

Provides a modular loot resolution system allowing entities to configure drop tables and trigger item drops upon destruction or direct inventory rewards upon interaction.

## Requirements

### Requirement: Weighted Loot Table Resolution
The lootable system SHALL support configuring weighted drop entries to randomly roll and select a collectible scene or empty drop based on relative weights.

#### Scenario: Rolling a drop from a weighted table
- **WHEN** an entity triggers loot generation with configured drop weights
- **THEN** a drop entry is selected according to its relative probability weight.

### Requirement: Destruction Drop Spawning
The lootable component SHALL listen for damage depletion events and spawn the resolved collectible entity into the scene with an initial velocity impulse.

#### Scenario: Dropping loot on destruction
- **GIVEN** an entity configured with drop-on-destroy is depleted of hit points
- **WHEN** the damage depletion signal fires
- **THEN** the resolved collectible scene is instantiated at the entity's position and propelled upward with initial velocity.

#### Scenario: Empty drop roll
- **GIVEN** an entity is destroyed and the drop roll resolves to no drop (empty)
- **WHEN** the damage depletion signal fires
- **THEN** no collectible is spawned into the scene.

### Requirement: Direct Interaction Looting
The lootable component SHALL support direct item acquisition when activated via interaction, granting the collectible effect directly to the interacting collector without spawning a physics object in the world.

#### Scenario: Direct collection on interact
- **GIVEN** an entity configured with loot-on-interact
- **WHEN** a player interacts with the entity
- **THEN** the resolved collectible reward is applied directly to the player and marked as looted.
