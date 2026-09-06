## Why

The player currently only possesses close-range melee attacks (`PlayerAttacking` sword swing). To expand combat depth, enable ranged engagements against flying enemies like bats and distant obstacles, and establish the projectile foundation for future exploration mechanics, we need to implement the Player Bow and Arrow system.

References GitHub Issue #21.

## What Changes

- **Bow Aiming & Shooting Input**:
  - Implements hybrid quick-tap vs. hold-and-aim controls using the configured item button (`B` / `Controller.B`).
  - Quick-tap fires an arrow immediately in the player's facing direction at the current elevation angle.
  - Holding the button transitions the player into an aiming stance (`PlayerAiming`), slowing down time (`Engine.TimeScale = 0.3`) and displaying an aiming crosshair indicator.
  - Walking while aiming is supported at reduced speed (50% run speed).
  - Elevation angle sweeps smoothly with `Up` / `Down` inputs (-45° to +90°).
  - Aim elevation angle is remembered across consecutive shots.
- **Player States**:
  - New `PlayerAiming` state for holding aim, slow-motion bullet-time, elevation adjustment, and slow walking.
  - New `PlayerShooting` state playing the 5-frame `Shoot` animation from `Zack.png`, instantiating the arrow projectile, and returning to idle/run.
- **Arrow Projectile (`Arrow.tscn` / `Arrow.cs`)**:
  - Parabolic ballistic trajectory inspired by TowerFall: flies flat and straight with zero gravity for an initial distance threshold (`StraightDistance`), then transitions to smooth gravity drop.
  - Projectile continuously rotates to match its velocity vector.
  - Carries a `Hitbox` on `PlayerHitbox` (Layer 7) that deals damage to `EnemyHurtbox` (Layer 8).
  - Embeds into `Solids` (Layer 1) or enemies upon impact, freezing physics and hitbox, then fading and despawning after a brief timeout.

## Non-Goals (Future Scope)

- Flammable ignition mechanics / torch lighting (tracked in Issue #53, #56).
- Inventory & ammo consumables management or chest pickup fanfare (tracked in Issue #41, #42, #49).
- Surface-dependent ricochet / deflection physics.

## Capabilities

### New Capabilities
- `player-bow-attack`: Covers bow aiming controls, bullet-time slow motion, player aiming/shooting state machine transitions, crosshair indicator, and arrow projectile physics/damage/embedding lifecycle.

### Modified Capabilities
*(None)*

## Impact

- **Player Controller & Input**: Maps item button (`B`) across keyboard and gamepad inputs.
- **Player State Machine**: Adds `PlayerAiming` and `PlayerShooting` states to `Player.tscn`.
- **Combat & Entities**: Adds `Arrow.tscn` projectile instance with collision handling against `Solids` and `EnemyHurtbox`.
