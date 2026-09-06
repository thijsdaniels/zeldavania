## Purpose

Defines combat and navigation behaviors for the Goblin enemy archetype, including ground target chasing and directional melee spear stabbing attacks.

## ADDED Requirements

### Requirement: Goblin Melee Stabbing Attack
The Goblin enemy controller SHALL execute a melee stabbing attack when in close proximity to the player target, activating a damage hitbox during active attack frames before recovering.

#### Scenario: Entering stabbing attack state in melee range
- **GIVEN** a Goblin enemy in `EnemyChasing` state pursuing a player target
- **WHEN** the player target is within horizontal reach distance
- **AND** the player target is vertically aligned on the same elevation plane
- **THEN** the finite state machine transitions the Goblin to `EnemyAttacking`
- **AND** the Goblin faces the direction of the target.

#### Scenario: Bypassing attack when target is above or below strike plane
- **GIVEN** a Goblin enemy in `EnemyChasing` state pursuing a player target
- **WHEN** the player target is vertically outside the horizontal strike plane (e.g. on a ledge or platform above)
- **THEN** the Goblin does not transition to `EnemyAttacking`
- **AND** the Goblin decelerates or continues tracking horizontally without triggering false attack loops.

#### Scenario: Executing stabbing animation and damage delivery
- **GIVEN** the Goblin enters `EnemyAttacking` state
- **WHEN** the stabbing animation plays
- **THEN** the Goblin movement is locked for the duration of the strike
- **AND** the melee attack `Hitbox` is enabled during the active damage frames to deliver damage and knockback on contact with the player's `Hurtbox`.

#### Scenario: Completing attack and recovery
- **GIVEN** the Goblin is in `EnemyAttacking` state
- **WHEN** the attack animation and recovery cooldown complete
- **THEN** the attack `Hitbox` is deactivated
- **AND** the Goblin transitions back to `EnemyStanding` or resumes pursuit in `EnemyChasing`.

#### Scenario: Falling while in attack state
- **GIVEN** the Goblin is in `EnemyAttacking` state
- **WHEN** the Goblin loses floor contact (`IsOnFloor() == false`)
- **THEN** the finite state machine transitions to `EnemyFalling`
- **AND** the attack `Hitbox` is deactivated.
