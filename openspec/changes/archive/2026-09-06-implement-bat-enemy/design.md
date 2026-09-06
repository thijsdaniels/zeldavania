## Context

See [`proposal.md`](file:///c:/Users/thijs/Repositories/thijsdaniels/zeldavania/openspec/changes/implement-bat-enemy/proposal.md) for motivation.

The project utilizes a node-based Finite State Machine architecture ([`FiniteStateMachine`](file:///c:/Users/thijs/Repositories/thijsdaniels/zeldavania/scripts/utilities/FiniteStateMachine.cs) and [`State`](file:///c:/Users/thijs/Repositories/thijsdaniels/zeldavania/scripts/utilities/State.cs)) and standardized combat traits ([`Damageable`](file:///c:/Users/thijs/Repositories/thijsdaniels/zeldavania/scripts/combat/Damageable.cs), [`Hurtbox`](file:///c:/Users/thijs/Repositories/thijsdaniels/zeldavania/scripts/combat/Hurtbox.cs), and [`Hitbox`](file:///c:/Users/thijs/Repositories/thijsdaniels/zeldavania/scripts/combat/Hitbox.cs)). The [`Enemy.cs`](file:///c:/Users/thijs/Repositories/thijsdaniels/zeldavania/scripts/entities/enemy/Enemy.cs) controller encapsulates perception triggers (`HearingArea`, `VisionArea`) and physics execution (`MoveAndSlide`) without coupling to ground physics or gravity.

## Goals / Non-Goals

**Goals:**
- Provide an aerial enemy archetype (`Bat.tscn`) capable of roosting on ceilings, performing parabolic swoop dives at the player, fluttering in sinusoidal flight patterns, and returning to roost when the player escapes.
- Reuse `Enemy.cs` as the generic entity controller and integrate seamlessly with `Damageable`, `Hurtbox`, `Hitbox`, `EnemyHurt`, and `EnemyDying`.
- Export `Bat.png` (64×16 spritesheet) from legacy PSD assets to configure `SpriteFrames` animations (`Sleeping`, `Flying`, `Hurt`).

**Non-Goals:**
- Dynamic ceiling pathfinding / multi-room navigation (bat returns to its local spawn/roost anchor).
- Projectile spitting or multi-stage boss behaviors (bat is a standard fodder flying archetype with 1-2 HP).
- Dynamic weighted loot table system (tracked in separate Issue #41).

## Decisions

### 1. Reusing `Enemy.cs` vs. Creating a Dedicated `FlyingEnemy` Class
- **Decision**: Reuse [`Enemy.cs`](file:///c:/Users/thijs/Repositories/thijsdaniels/zeldavania/scripts/entities/enemy/Enemy.cs) as the root controller script on `Bat.tscn`.
- **Rationale**: `Enemy.cs` is already completely decoupled from ground physics—it only connects sensory perception signals, stores `Target`, and invokes `MoveAndSlide()`. All flight mechanics are neatly encapsulated in dedicated `State` nodes.
- **Alternatives Considered**:
  - *Dedicated `Bat.cs` / `FlyingEnemy.cs`*: Adds redundant duplicate signal management and component exports.

### 2. State Breakdown for Aerial Navigation
- **Decision**: Implement 4 specialized flight states:
  1. `BatRoosting.cs`: Idles on the ceiling playing `Sleeping` animation, records initial roost anchor `GlobalPosition`, and listens to `_enemy.OnAlerted` / `_enemy.OnTargetSpotted`.
  2. `BatSwooping.cs`: Calculates dive trajectory toward the target's position with downward acceleration, transitions to `BatFluttering` upon completing the upward recovery arc.
  3. `BatFluttering.cs`: Maintains cruising altitude, applying sinusoidal vertical oscillation (`Velocity.Y = Mathf.Sin(_time * _frequency) * _amplitude`) and horizontal drift toward/around the player. Uses a cooldown timer to transition back into `BatSwooping` when target is in range, or `BatReturning` when target is lost.
  4. `BatReturning.cs`: Navigates back to the roost anchor `GlobalPosition`, aligning to the ceiling before transitioning back to `BatRoosting`.
- **Alternatives Considered**:
  - *Monolithic `BatFlying` state*: Handling dive math, sinusoidal oscillation, and return navigation in a single script creates high cyclomatic complexity.

### 3. Scene Composition (`Bat.tscn`)

```
Bat (CharacterBody2D / Enemy.cs, collision_layer = 4)
├── AnimatedSprite2D (SpriteFrames with "Sleeping", "Flying", "Hurt")
├── CollisionShape2D (CircleShape2D / RectangleShape2D)
├── ContactHitbox (Hitbox.cs, collision_layer = 32, collision_mask = 16, Damage = 1)
│   └── CollisionShape2D
├── Hurtbox (Hurtbox.cs, collision_layer = 128, collision_mask = 64)
│   └── CollisionShape2D
├── Damageable (Damageable.cs, MaxHealth = 1)
├── HearingArea (Area2D, collision_mask = 256)
│   └── CollisionShape2D (CircleShape2D, radius ~120px)
├── VisionArea (Area2D, collision_mask = 256)
│   └── CollisionPolygon2D / CollisionShape2D
├── SoundEffects (Node2D)
│   └── HitEffect (AudioStreamPlayer2D)
└── State (FiniteStateMachine, initial_state = Roosting)
    ├── Roosting (BatRoosting.cs)
    ├── Swooping (BatSwooping.cs)
    ├── Fluttering (BatFluttering.cs)
    ├── Returning (BatReturning.cs)
    ├── Hurt (EnemyHurt.cs, _gravity = 200, _chasingState -> Fluttering)
    └── Dying (EnemyDying.cs, _gravity = 500)
```

## Risks / Trade-offs

- **[Risk] Obstacle / Wall Collision during Swoop**: The bat might swoop into solid terrain or floors during a steep dive.
  - *Mitigation*: Use `MoveAndSlide()` collision response and clamp the minimum dive height above ground or abort dive on floor collision into `BatFluttering`.
- **[Risk] Jitter when reaching Roost Anchor**: Bat returning to roost might overshoot or oscillate around the anchor coordinate.
  - *Mitigation*: Use `GlobalPosition.DistanceTo(roostAnchor) < threshold` to snap and immediately enter `BatRoosting`.
