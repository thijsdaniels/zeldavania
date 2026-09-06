# Tasks: Player Sword Attack & Starter Weapon

## 1. Input Mapping

- [x] 1.1 Add keyboard bindings (`Key.J` and `Key.X`) for the `X` action in `project.godot` alongside Joypad Button 2 and verify with `git diff project.godot`.

## 2. State & Combat Logic Implementation

- [x] 2.1 Implement `scripts/entities/player/states/PlayerAttacking.cs` with `"Sword"` animation playback, facing-direction hitbox offset alignment, active swing frame monitoring (frames 1–3), input buffering during recovery (frame 4), grounded deceleration, and aerial gravity handling.
- [x] 2.2 Add `_attackingState` exports and attack input transitions (`Controller.X`) in `PlayerStanding.cs`, `PlayerRunning.cs`, and `PlayerFalling.cs`.

## 3. Player Scene & Node Composition

- [x] 3.1 Add `SwordHitbox` (`Area2D` using `Zeldavania.Combat.Hitbox`, collision layer 64 / mask 128) with child `CollisionShape2D` to `scenes/entities/Player.tscn`.
- [x] 3.2 Add `Attacking` state node (`PlayerAttacking`) under `State` in `scenes/entities/Player.tscn` and wire all exported references (`_body`, `_sprite`, `_hitbox`, `_hitboxShape`, `_standingState`, `_runningState`, `_fallingState`).

## 4. Compilation & Verification

- [x] 4.1 Run `dotnet build` to verify clean C# compilation across all player state scripts.
- [x] 4.2 Verify grounded attack execution, aerial swing gravity, directional hitbox flipping, input buffering, and state return to idle/running.

## 5. Combat Feel & Enemy Reaction Polish

- [x] 5.1 Implement `EnemyHurt.cs` state with hitstun, knockback impulse, hurtbox invulnerability, and red/alpha hit flash.
- [x] 5.2 Wire `Damageable`, `Hurtbox`, `EnemyHurt` state node, and `HitEffect` audio player in `Goblin.tscn`.
- [x] 5.3 Update `EnemyDying.cs` to preserve knockback momentum and gravity throughout the defeat animation.
- [x] 5.4 Fix `WithSynchronizedAnimations.cs` so stationary speedscale defaults to 1.0f, preventing animation freeze on attack entry.
