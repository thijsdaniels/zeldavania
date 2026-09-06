## Context

Enemies currently transition into `EnemyDying` on reaching 0 HP. However, `EnemyDying` solely plays the `_animation` (default `"Dying"`) and listens for `_sprite.AnimationFinished`. When an enemy has no `"Dying"` animation in its `SpriteFrames` (e.g. Bat), the state hangs in the single-frame `Hurt` animation while falling under gravity. Furthermore, hitboxes and hurtboxes remain active or lingering during collapse.

## Goals / Non-Goals

**Goals:**
- Provide a universal, reusable visual/audio death burst (`EnemyDeathEffect.tscn`) using `explode.wav` and a Zelda-style smoke puff animation.
- Make `EnemyDying` cleanly handle both animated collapses (e.g. Goblin) and immediate explosions (e.g. Bat) without hardcoding enemy types.
- Ensure hitboxes and hurtboxes are disabled the instant `EnemyDying` begins to prevent dead enemies from damaging the player or blocking projectiles.

**Non-Goals:**
- Custom drawn collapse sprites for the Bat (deferred to future art passes).
- Item drop table logic (hearts/rupees/ammo drops will plug into the death trigger separately).

## Decisions

### Decision 1: `EnemyDeathEffect` as a Standalone One-Shot Scene
- **Approach**: Create `scenes/objects/EnemyDeathEffect.tscn` with a dedicated script `scripts/objects/EnemyDeathEffect.cs`.
- **Composition**:
  - `AnimatedSprite2D` playing a 4-frame retro smoke poof (`assets/textures/effects/EnemyDeathPuff.png`).
  - `AudioStreamPlayer2D` playing `res://assets/sounds/effects/explode.wav` with random pitch modulation (e.g., `0.95` - `1.15`).
  - Auto-cleanup (`QueueFree()`) as soon as the animation / sound ends (~0.35s).
- **Alternative considered**: Inlining particles and sound inside `EnemyDying.cs` before `QueueFree()`. Rejected because freeing the enemy node would immediately terminate attached audio and particle instances.

### Decision 2: Graceful Animation Detection in `EnemyDying.cs`
- **Approach**: Check `_sprite?.SpriteFrames != null && !string.IsNullOrEmpty(_animation) && _sprite.SpriteFrames.HasAnimation(_animation)`.
  - If `true`: Hook `AnimationFinished`, play `_animation`, apply knockback friction/gravity. When finished, spawn death effect and free enemy.
  - If `false`: Immediately spawn death effect and free enemy.
- **Rationale**: Clean, decoupled, zero-configuration for new enemies without death art, while preserving death art when available.

### Decision 3: Immediate Hitbox & Hurtbox Deactivation
- **Approach**: On `EnemyDying.Enter()`, set hurtbox and contact hitbox monitoring/monitorable to `false` and set invulnerability to `true`.
- **Rationale**: Prevents ghost collisions or unfair damage while an enemy is collapsing.

## Risks / Trade-offs

- **[Risk]** Spawning death effects into the parent scene when enemies die at boundary transitions.
  - **Mitigation**: Check `_enemy.GetParent() != null` and add the effect to the enemy's parent scene tree at `_enemy.GlobalPosition`.
