## ADDED Requirements

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
