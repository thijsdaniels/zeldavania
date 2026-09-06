# Bat Enemy Specification

## Purpose

Defines aerial flight behaviors for the Bat enemy archetype including ceiling roosting, sinusoidal aerial flutter, curved swoop attacks, and roost return navigation.

## Requirements

### Requirement: Bat Ceiling Roosting and Awakening
The Bat enemy controller SHALL support roosting upside-down on ceilings in an idle sleeping state and awakening into an aerial flutter state when a player target enters detection range.

#### Scenario: Awakening from ceiling roost
- **GIVEN** a Bat enemy in roosting state attached to or positioned near a ceiling
- **WHEN** the player enters the Bat's detection range (hearing or proximity area)
- **THEN** the Bat transitions to the aerial flutter state
- **AND** records its roost anchor origin.

### Requirement: Bat Swoop Attack
The Bat enemy controller SHALL execute a smooth curved or parabolic dive attack toward the player target's position when initiating an attack.

#### Scenario: Executing swoop dive
- **GIVEN** a Bat enemy initiating a swoop attack toward a player target
- **WHEN** the swoop physics update runs
- **THEN** the Bat accelerates along a curved arc descending toward the target's position
- **AND** passes through or just above the target position before ascending into its recovery flight path.

#### Scenario: Inflicting contact damage during swoop
- **GIVEN** a Bat enemy swooping toward the player
- **WHEN** the Bat's contact `Hitbox` overlaps the player's `Hurtbox`
- **THEN** contact damage and knockback are delivered to the player.

### Requirement: Bat Aerial Flutter Patrol
The Bat enemy controller SHALL maintain hovering and flight movement via sinusoidal vertical oscillation and horizontal drifting when cruising or seeking targets.

#### Scenario: Sinusoidal flight motion
- **GIVEN** a Bat enemy in the aerial flutter state
- **WHEN** updating physics
- **THEN** the Bat applies a sinusoidal vertical wave pattern (`sin(time * freq) * amp`) while maintaining horizontal movement.

#### Scenario: Triggering swoop attack
- **GIVEN** a Bat enemy in the aerial flutter state tracking an active target
- **WHEN** the swoop cooldown timer elapses and the player remains within attack range
- **THEN** the Bat transitions to the swoop attack state.

### Requirement: Bat Return to Roost
The Bat enemy controller SHALL navigate back to its ceiling roost position when the target is lost or out of detection range.

#### Scenario: Returning to ceiling when target lost
- **GIVEN** a Bat enemy in the aerial flutter state
- **WHEN** the player target moves outside detection range or is lost
- **THEN** the Bat transitions to the returning state
- **AND** steers upward toward its recorded roost anchor position
- **AND** transitions back to roosting once reaching the ceiling anchor.
