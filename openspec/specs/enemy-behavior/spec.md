# Enemy Behavior Specification

## Purpose
Coordinates sensory perception (hearing, vision) and state machine behaviors for generic ground enemies, including idling/sleeping, waking on alert, target chasing, active melee attacking, falling, and defeat.

## Requirements

### Requirement: Sensory Perception & Target Tracking
The `Enemy` coordinator SHALL detect the player through hearing and vision trigger areas.

#### Scenario: Hearing detection
- **GIVEN** an enemy in a resting or idling state
- **WHEN** the `Player` enters the enemy's `HearingArea`
- **THEN** the enemy emits the `OnAlerted` signal passing the player reference.

#### Scenario: Vision spotting and losing target
- **GIVEN** an enemy with an active `VisionArea`
- **WHEN** the `Player` enters the `VisionArea` and the enemy has no target
- **THEN** the enemy sets `Target = player`
- **AND** emits the `OnTargetSpotted` signal.
- **WHEN** the tracked `Target` exits the `VisionArea`
- **THEN** the enemy sets `Target = null`
- **AND** emits the `OnTargetLost` signal.

### Requirement: Enemy State Machine Behaviors
Enemies SHALL transition between modular states depending on sensory signals and environment.

#### Scenario: Sleeping and waking
- **GIVEN** an enemy in `EnemySleeping` state with vision monitoring disabled
- **WHEN** the `OnAlerted` signal is received
- **THEN** the enemy transitions to its configured wake state (`_onWake`)
- **AND** re-enables vision monitoring on state exit.

#### Scenario: Target chasing & navigation
- **GIVEN** an enemy in `EnemyChasing` state with a valid `Target`
- **WHEN** updating physics
- **THEN** the enemy accelerates horizontally toward the target's position up to its speed limit
- **WHEN** distance to target falls below the reach threshold
- **THEN** the enemy transitions to `_onTargetReached`.
- **WHEN** the target is lost (`Target == null`)
- **THEN** the enemy transitions to `_onTargetLost`.

#### Scenario: Airborne enemy falling
- **GIVEN** an enemy in any grounded state
- **WHEN** `IsOnFloor() == false`
- **THEN** the enemy transitions to `EnemyFalling` and applies gravity until grounded.

#### Scenario: Defeat and cleanup
- **GIVEN** an enemy taking fatal damage
- **WHEN** transitioning to `EnemyDying`
- **THEN** the enemy plays its death animation and queues removal from the scene tree.

### Requirement: Enemy Melee Stabbing Attack
The enemy controller SHALL execute a melee stabbing attack when in close proximity to the player target, activating a damage hitbox during active attack frames before recovering.

#### Scenario: Entering stabbing attack state in melee range
- **GIVEN** an enemy is in `EnemyChasing` state pursuing a player target
- **WHEN** the player target is within horizontal reach distance
- **AND** the player target is vertically aligned on the same elevation plane
- **THEN** the finite state machine transitions the enemy to `EnemyAttacking`
- **AND** the enemy faces the direction of the target.

#### Scenario: Bypassing attack when target is above or below strike plane
- **GIVEN** an enemy is in `EnemyChasing` state pursuing a player target
- **WHEN** the player target is vertically outside the horizontal strike plane (e.g. on a ledge or platform above)
- **THEN** the enemy does not transition to `EnemyAttacking`
- **AND** the enemy decelerates or continues tracking horizontally without triggering false attack loops.

#### Scenario: Executing stabbing animation and damage delivery
- **GIVEN** the enemy enters `EnemyAttacking` state
- **WHEN** the stabbing animation plays
- **THEN** the enemy movement is locked for the duration of the strike
- **AND** the melee attack `Hitbox` is enabled during the active damage frames to deliver damage and knockback on contact with the player's `Hurtbox`.

#### Scenario: Completing attack and recovery
- **GIVEN** the enemy is in `EnemyAttacking` state
- **WHEN** the attack animation and recovery cooldown complete
- **THEN** the attack `Hitbox` is deactivated
- **AND** the enemy transitions back to `EnemyStanding` or resumes pursuit in `EnemyChasing`.

#### Scenario: Falling while in attack state
- **GIVEN** the enemy is in `EnemyAttacking` state
- **WHEN** the enemy loses floor contact (`IsOnFloor() == false`)
- **THEN** the finite state machine transitions to `EnemyFalling`
- **AND** the attack `Hitbox` is deactivated.

### Requirement: Enemy Damage Reaction & Hitstun
The system SHALL transition an enemy to `EnemyHurt` upon receiving a valid hit, applying knockback momentum, visual flash, hitstun, and temporary hurtbox invulnerability.

#### Scenario: Receiving a non-lethal hit
- **GIVEN** an enemy has an active `Hurtbox` and `Damageable` component
- **WHEN** a `Hit` is received with damage less than remaining hit points
- **THEN** the finite state machine transitions to `EnemyHurt`
- **AND** the enemy plays the impact sound effect, triggers a rapid damage flash, and receives directional knockback away from the hit origin
- **AND** the enemy's `Hurtbox` is marked invulnerable during the hitstun window.

#### Scenario: Recovering from hitstun
- **GIVEN** an enemy is in `EnemyHurt` state
- **WHEN** the hitstun duration expires
- **THEN** `Hurtbox` invulnerability is cleared
- **AND** the enemy transitions to `EnemyChasing` if a target is tracked, or `EnemyStanding` otherwise.

#### Scenario: Defeat momentum preservation
- **GIVEN** an enemy takes lethal damage depleting its hit points
- **WHEN** transitioning to `EnemyDying`
- **THEN** the enemy preserves its knockback velocity and applies gravity while playing the death animation.

