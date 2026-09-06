# Enemy Behavior Specification

## Purpose
Coordinates sensory perception (hearing, vision) and state machine behaviors for base enemies, including idling/sleeping, waking on alert, target tracking, falling, damage reactions, and defeat.

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

### Requirement: Enemy Resting and Awakening
The enemy finite state machine SHALL support resting/sleeping behavior and awakening upon sensory alert.

#### Scenario: Sleeping and waking
- **GIVEN** an enemy in `EnemySleeping` state with vision monitoring disabled
- **WHEN** the `OnAlerted` signal is received
- **THEN** the enemy transitions to its configured wake state (`_onWake`)
- **AND** re-enables vision monitoring on state exit.

### Requirement: Enemy Target Navigation
The enemy finite state machine SHALL navigate and accelerate toward tracked targets.

#### Scenario: Target chasing & navigation
- **GIVEN** an enemy in `EnemyChasing` state with a valid `Target`
- **WHEN** updating physics
- **THEN** the enemy accelerates horizontally toward the target's position up to its speed limit
- **WHEN** distance to target falls below the reach threshold
- **THEN** the enemy transitions to `_onTargetReached`.
- **WHEN** the target is lost (`Target == null`)
- **THEN** the enemy transitions to `_onTargetLost`.

### Requirement: Enemy Airborne Gravity
The enemy finite state machine SHALL apply gravity whenever an enemy becomes airborne.

#### Scenario: Airborne enemy falling
- **GIVEN** an enemy in any grounded state
- **WHEN** `IsOnFloor() == false`
- **THEN** the enemy transitions to `EnemyFalling` and applies gravity until grounded.

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

### Requirement: Enemy Defeat & Cleanup
The enemy finite state machine SHALL handle entity defeat upon taking lethal damage, deactivating combat collisions and spawning death effects.

#### Scenario: Defeat with death animation
- **GIVEN** an enemy taking fatal damage that has a configured death animation
- **WHEN** transitioning to `EnemyDying`
- **THEN** the enemy's hurtbox and contact hitbox are immediately disabled
- **AND** the enemy plays its death animation with movement friction and gravity
- **AND** when the animation finishes, spawns the universal death effect (smoke puff and explosion sound) and queues removal from the scene tree.

#### Scenario: Defeat without death animation
- **GIVEN** an enemy taking fatal damage that lacks a death animation (or has it unconfigured/empty)
- **WHEN** transitioning to `EnemyDying`
- **THEN** the enemy's hurtbox and contact hitbox are immediately disabled
- **AND** the enemy immediately spawns the universal death effect (smoke puff and explosion sound) and queues removal from the scene tree with zero delay.

