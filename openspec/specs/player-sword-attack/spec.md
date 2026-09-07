# Player Sword Attack Specification

## Purpose

Governs the player character's offensive sword attack state, swing animation lifecycle, directional hitbox activation, movement inertia and aerial physics, and input buffering for chained attacks.

## Requirements

### Requirement: Attack Execution & State Entry
The system SHALL transition the player into the `PlayerAttacking` state when any action button (`Controller.X`, `Controller.Y`, or `Controller.B`) assigned to a melee weapon is triggered from valid states, playing the weapon's configured attack animation.

#### Scenario: Attacking while standing
- **GIVEN** the player is in `PlayerStanding` state on the floor
- **WHEN** an action button assigned to the Sword is pressed
- **THEN** the player transitions to `PlayerAttacking` state
- **AND** the configured attack animation (e.g. `"Sword"`) plays on the animated sprite.

#### Scenario: Attacking while running
- **GIVEN** the player is in `PlayerRunning` state on the floor
- **WHEN** an action button assigned to the Sword is pressed
- **THEN** the player transitions to `PlayerAttacking` state
- **AND** the configured attack animation plays on the animated sprite.

#### Scenario: Attacking while airborne
- **GIVEN** the player is in `PlayerFalling` state in the air
- **WHEN** an action button assigned to the Sword is pressed
- **THEN** the player transitions to `PlayerAttacking` state
- **AND** the configured attack animation plays on the animated sprite.


### Requirement: Directional Hitbox Synchronization
The system SHALL synchronize the attack `Hitbox` activation window and physical offset with the active frames and facing orientation of the `"Sword"` animation.

#### Scenario: Hitbox activation during swing frames
- **GIVEN** the player is in `PlayerAttacking` state
- **WHEN** the `"Sword"` animation reaches frames 1, 2, or 3
- **THEN** the player's attack `Hitbox` is enabled (`Monitoring = true` and collision shape enabled)
- **AND** positioned forward relative to the player's facing direction (`FlipH`).

#### Scenario: Hitbox deactivation on recovery frames
- **GIVEN** the player is in `PlayerAttacking` state
- **WHEN** the `"Sword"` animation advances to frame 4 (or leaves the state)
- **THEN** the player's attack `Hitbox` is disabled.

### Requirement: Attack Physics & Movement
The system SHALL apply friction deceleration during grounded attacks and maintain continuous gravity during aerial attacks.

#### Scenario: Grounded attack deceleration
- **GIVEN** the player is executing an attack on the floor
- **WHEN** physics updates occur during the attack
- **THEN** horizontal velocity is decelerated using ground friction
- **AND** character body is moved using standard collision routines.

#### Scenario: Aerial attack gravity and landing
- **GIVEN** the player is executing an attack in mid-air
- **WHEN** physics updates occur
- **THEN** vertical gravity continues to accelerate the player downward
- **AND** if the player contacts the floor while still in the attack, the player remains in `PlayerAttacking` until the animation completes.

### Requirement: Input Buffering & Attack Completion
The system SHALL buffer attack inputs received during the recovery phase to chain consecutive swings, or transition cleanly back to standard movement states upon animation finish.

#### Scenario: Attack completes without follow-up input
- **GIVEN** the player is in `PlayerAttacking` state and no attack input was buffered
- **WHEN** the `"Sword"` animation finishes playing
- **THEN** the player transitions to `PlayerStanding` if grounded (or `PlayerRunning` if horizontal input is active), or `PlayerFalling` if in mid-air.

#### Scenario: Buffered attack chains next swing
- **GIVEN** the player is in the recovery phase of `PlayerAttacking`
- **WHEN** the Attack action is pressed
- **THEN** the attack input is recorded as buffered
- **AND** upon animation completion, the player restarts `PlayerAttacking` immediately.
