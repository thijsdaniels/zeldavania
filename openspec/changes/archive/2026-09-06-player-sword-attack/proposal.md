# Proposal: Player Sword Attack & Starter Weapon

## Why

The player currently has no attack ability or offensive combat state. In `PlayerStanding.cs`, attack transitions were left as commented-out placeholders. Implementing the starter sword slash addresses **[GitHub Issue #8](https://github.com/thijsdaniels/zeldavania/issues/8)**, establishing the primary offensive interaction mechanic for defeating enemies, cutting vegetation/bushes, and triggering combat switches across the world.

## What Changes

- Add keyboard action bindings for `Controller.X` (`Key.J` and `Key.X`) in `project.godot` alongside existing gamepad bindings.
- Implement the `PlayerAttacking` state in the player state machine (`scripts/entities/player/states/PlayerAttacking.cs`):
  - Triggers on attack input (`Controller.X`) from `PlayerStanding`, `PlayerRunning`, and `PlayerFalling`.
  - Plays the 5-frame `"Sword"` animation from `Zack.png` on `AnimatedSprite2D`.
  - Supports grounded friction deceleration with subtle forward step impulse.
  - Supports mid-air slashing maintaining aerial gravity and momentum.
  - Implements input buffering during recovery frames to smoothly chain follow-up attacks.
- Add and wire a directional `Hitbox` (`PlayerHitbox`, Collision Layer 7 / Mask 8) on the `Player` scene synchronized to active swing frames (frames 1–3) and facing orientation (`FlipH`).
- Provide an `AudioStreamPlayer2D` hook on `PlayerAttacking` for sword swing audio.
- Implement `EnemyHurt` state with hitstun, knockback, flash, and invulnerability window, and wire `Damageable`/`Hurtbox` to `Goblin.tscn`.
- Fix `WithSynchronizedAnimations.cs` so stationary entities default to `SpeedScale = 1.0f` to prevent animation freezing.

## Capabilities

### New Capabilities
- `player-sword-attack`: Defines specifications for player sword attack triggers, animation lifecycle, directional hitbox activation, movement deceleration/momentum, aerial swinging, and input buffering.

### Modified Capabilities
- `player-movement`: Adds attack action transitions from `PlayerStanding`, `PlayerRunning`, and `PlayerFalling` into the attacking state.
- `enemy-behavior`: Adds enemy hit reaction state (`EnemyHurt`), damage hitstun, knockback, and hurtbox invulnerability lifecycle.

## Impact

- **Affected Code**:
  - `project.godot`: Input map updates for `X` action (adding keyboard keys `J` and `X`).
  - `scripts/entities/player/states/PlayerStanding.cs`: Wire transition to `PlayerAttacking`.
  - `scripts/entities/player/states/PlayerRunning.cs`: Wire transition to `PlayerAttacking`.
  - `scripts/entities/player/states/PlayerFalling.cs`: Wire transition to `PlayerAttacking`.
  - `scripts/entities/player/states/PlayerAttacking.cs` (New): State implementation.
  - `scripts/entities/enemy/Enemy.cs`: Wire `Damageable` and `Hurtbox` references.
  - `scripts/entities/enemy/states/EnemyHurt.cs` (New): Enemy hurt reaction state with hitstun and knockback.
  - `scripts/entities/enemy/states/EnemyDying.cs`: Preserve knockback momentum and gravity during death.
  - `scripts/extensions/AnimatedSprite2D/WithSynchronizedAnimations.cs`: Ensure `SpeedScale` defaults to 1.0f when stationary.
- **Affected Scenes**:
  - `scenes/entities/Player.tscn`: Add `PlayerAttacking` state node, wire exports, and add `Hitbox` node configured for Layer 7 (`PlayerHitbox`) targeting Layer 8 (`EnemyHurtbox`).
  - `scenes/entities/Goblin.tscn`: Add `Damageable`, `Hurtbox`, `EnemyHurt` state node, and `HitEffect` audio player.
