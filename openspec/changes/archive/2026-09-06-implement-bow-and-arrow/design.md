## Context

See `proposal.md` for motivation and background.
Currently, the player uses a finite state machine (`FiniteStateMachine`, `State`) with melee combat (`PlayerAttacking`). Zack's sprite sheet (`Zack.png`) already has a 5-frame `Shoot` animation configured in `Player.tscn` at `y = 192`. Collision layers are partitioned into `Solids` (Layer 1), `PlayerHitbox` (Layer 7), and `EnemyHurtbox` (Layer 8).

## Goals / Non-Goals

**Goals:**
- Implement hybrid bow aiming and shooting controls with snappy quick-firing and hold-to-aim slow-motion bullet-time.
- Implement walk-aiming with smooth continuous elevation angle adjustments (-45° to +90°) and aim angle memory.
- Implement an aiming crosshair indicator rendered at radius $R$ from the player.
- Implement an `Arrow` projectile entity (`Arrow.tscn` / `Arrow.cs`) with TowerFall-style two-phase ballistics (straight distance cutoff followed by parabolic gravity arc).
- Implement arrow collision, enemy damage via `Hitbox`, surface embedding, and despawn lifecycle.

**Non-Goals:**
- Inventory UI, ammo counter widgets, or chest fanfare (deferred to Issue #41, #42, #49).
- Flammable environmental ignition / torch lighting (deferred to Issue #53, #56).
- Surface-dependent ricochet / material deflection physics.

## Decisions

### 1. State Machine Architecture: `PlayerAiming` and `PlayerShooting`

- **`PlayerAiming` State**:
  - Activated when holding the item button (`Controller.B`) from `PlayerStanding`, `PlayerRunning`, or `PlayerFalling`.
  - Sets `Engine.TimeScale = 0.3` for dramatic bullet-time aiming.
  - Allows slow walking at 50% speed (`_body.Velocity = new Vector2(hInput * _walkAimSpeed, _body.Velocity.Y)`).
  - Handles elevation adjustments: vertical input `Up` / `Down` sweeps the elevation angle $\theta$ between $-45^\circ$ and $+90^\circ$ at `_aimSweepSpeed` (rad/s).
  - Positions `Crosshair2D` at `GlobalPosition + AimVector * _crosshairDistance`.
  - Releasing `Controller.B` restores `Engine.TimeScale = 1.0`, hides the crosshair, and transitions to `PlayerShooting`.
- **`PlayerShooting` State**:
  - Plays the 5-frame `Shoot` sprite animation.
  - Instantiates `Arrow.tscn` at the player's muzzle point oriented along the aim vector.
  - Upon animation completion, transitions back to `PlayerStanding`, `PlayerRunning`, or `PlayerFalling`.
- **Quick-Tap Firing**:
  - In `PlayerStanding` / `PlayerRunning` / `PlayerFalling`, a quick tap of `Controller.B` directly initiates `PlayerShooting` using the remembered elevation angle in the active facing direction without triggering slow-motion.

*Alternative Considered*: Putting aiming and shooting logic inside `PlayerStanding`. *Rejected* to maintain single-responsibility state separation adhering to the project's FSM conventions.

### 2. Input Mapping (`Controller.cs` & `project.godot`)

- Add `Controller.B` mapping in `project.godot` (bound to Gamepad `B`, Keyboard `K` and `C`).
- Add helper methods in `Controller.cs` to query `Controller.B` pressed, just pressed, and just released.

### 3. Arrow Flight Ballistics & Entity Setup (`Arrow.tscn` / `Arrow.cs`)

- **Node Hierarchy**:
  ```
  Arrow (Area2D / CharacterBody2D)
  ├── Sprite2D (assets/textures/objects/Arrow.png or sliced collectible sprite)
  ├── CollisionShape2D (solids detection)
  └── Hitbox (Area2D, Layer 7 PlayerHitbox)
      └── CollisionShape2D (Layer 8 EnemyHurtbox detection)
  ```
- **Trajectory Integration**:
  - Initial parameters: `_launchSpeed` (e.g. 360 px/s), `_straightDistance` (e.g. 128 px), `_gravity` (e.g. 600 px/s²).
  - Accumulates `_distanceTraveled`.
  - While `_distanceTraveled < _straightDistance`, `Velocity = _launchDirection * _launchSpeed` (zero gravity).
  - When `_distanceTraveled >= _straightDistance`, `Velocity.Y += _gravity * delta`.
  - Rotates every frame: `Rotation = Velocity.Angle()`.

*Alternative Considered*: Pure rigid body 2D with continuous physics. *Rejected* because TowerFall's sharp transition from straight flight to gravity arc is more controllable with kinematic integration.

### 4. Impact, Embedding, and Despawn

- On colliding with `Solids` (Layer 1) or `EnemyHurtbox` (Layer 8):
  - Sets velocity to zero and disables collision / hitbox shapes immediately.
  - If hitting an enemy, applies damage via `Damageable` component on hurtbox.
  - If hitting a solid surface, embeds at the impact position and rotation.
  - Starts a `_despawnTimer` (1.5 seconds) with a subtle fade-out before `QueueFree()`.

## Risks / Trade-offs

- **[Risk]** `Engine.TimeScale = 0.3` slows down the player's own physics movement during aiming.
  - **Mitigation**: Scale the player's internal walk-aim velocity property so slow-walking remains responsive and feels deliberate in game time.
- **[Risk]** Fast-moving arrow passing through thin tile colliders (tunneling).
  - **Mitigation**: Use step raycasting or `MoveAndCollide` / fine delta checks in `_PhysicsProcess`.
