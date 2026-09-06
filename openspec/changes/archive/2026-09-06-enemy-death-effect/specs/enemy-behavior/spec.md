## MODIFIED Requirements

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
