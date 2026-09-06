# Design Document: Player Sword Attack

## Context

See [proposal.md](proposal.md) for motivation and [specs/](specs/) for requirement specifications.

The player character entity (`Player.tscn`) uses a node-based Finite State Machine (`FiniteStateMachine` / `State`) with separate state nodes for `Standing`, `Running`, `Falling`, `Jumping`, `Climbing`, `Swimming`, and `Hurt`. The `AnimatedSprite2D` already includes the 5-frame `"Sword"` animation from `Zack.png` (configured with `loop = 0` at 12 FPS). The combat trait architecture provides a reusable `Hitbox` `Area2D` and `Hit` data payload.

## Goals / Non-Goals

**Goals:**
- Provide a responsive sword attack usable from grounded (`Standing`, `Running`) and aerial (`Falling`) states.
- Synchronize attack hitbox monitoring and position to active swing frames (frames 1–3) and player facing direction (`FlipH`).
- Implement input buffering during recovery (frame 4) to allow chaining consecutive attacks without dropped inputs.
- Keep mid-air gravity active during aerial attacks for natural arc trajectories.
- Add keyboard bindings for the `X` action in `project.godot`.

**Non-Goals:**
- Multi-weapon inventory cycling or UI weapon wheel (covered by Issue #49).
- Charge attacks / spin attacks.
- Projectile sword beams (handled in separate upgrade tickets).

## Decisions

### 1. Dedicated `PlayerAttacking` State Node
- **Architecture**: A new `PlayerAttacking` C# class inheriting from `State`.
- **Node Hierarchy in `Player.tscn`**:
  ```
  Player (CharacterBody2D)
  ├── AnimatedSprite2D
  ├── SwordHitbox (Hitbox: Area2D, Layer 7: PlayerHitbox, Mask 8: EnemyHurtbox)
  │   └── CollisionShape2D (RectangleShape2D, disabled by default)
  └── State (FiniteStateMachine)
      └── Attacking (PlayerAttacking)
  ```
- **Inspector Exports**:
  ```csharp
  [Export] private CharacterBody2D _body;
  [Export] private AnimatedSprite2D _sprite;
  [Export] private Hitbox _hitbox;
  [Export] private CollisionShape2D _hitboxShape;
  [Export] private AudioStreamPlayer2D _soundEffect;
  [ExportGroup("Physics")]
  [Export] private float _deceleration = 800f;
  [Export] private float _forwardImpulse = 25f;
  [ExportGroup("Transitions")]
  [Export] private State _standingState;
  [Export] private State _runningState;
  [Export] private State _fallingState;
  ```

### 2. Hitbox Timing & Orientation
- **Offset Calculation**:
  - Base offset: `Vector2(10, -7)` relative to the player origin.
  - When facing left (`_sprite.FlipH == true`): offset is `Vector2(-10, -7)`.
- **Frame Lifecycle**:
  - `Enter()`:
    - Determine facing direction from `_sprite.FlipH`.
    - Adjust `_hitbox.Position` to forward offset.
    - If grounded and standing, apply slight initial forward impulse in facing direction.
    - Play `"Sword"` animation.
    - Reset `_hasBufferedAttack = false`.
    - Disable hitbox initially (`_hitboxShape.Disabled = true`).
    - Play optional sound effect if assigned.
  - `UpdatePhysics(double delta)`:
    - Check current `_sprite.Frame`:
      - Frames 1..3: `_hitboxShape.Disabled = false`
      - Frame 0 & Frame 4: `_hitboxShape.Disabled = true`
    - Check input buffer:
      - If `_sprite.Frame >= 3` and `Input.IsActionJustPressed(Controller.X)`: set `_hasBufferedAttack = true`.
    - Apply movement physics:
      - If grounded: apply deceleration inertia toward 0.
      - If airborne: apply gravity acceleration `_body.ApplyGravity((float)delta)`.
    - Check state completion on `_sprite.AnimationFinished` (or frame completion):
      - If `_hasBufferedAttack`: re-trigger `Enter()` for chained swing.
      - Else if `_body.IsOnFloor()`: transition to `_runningState` (if directional input != 0) or `_standingState`.
      - Else: transition to `_fallingState`.
  - `Exit()`:
    - Always ensure `_hitboxShape.Disabled = true` to prevent lingering hitboxes on interruptions (e.g. `PlayerHurt`).

### 3. Controller & Input Mapping
- In `project.godot`, update the `X` input action:
  - Gamepad: Joypad Button 2 (`X` / `Square`) [existing].
  - Keyboard: `Key.J` (Physical Keycode 74) and `Key.X` (Physical Keycode 88).

### 4. Enemy Damage Reaction (`EnemyHurt.cs`)
- **Hitstun & Knockback**:
  - When taking damage, `EnemyHurt` triggers `0.18s` of hitstun with directional knockback (`0.55x` force ratio, `-80px/s` vertical pop).
  - `Hurtbox.IsInvulnerable` is set during hitstun to prevent multi-hit damage from a single sword swing.
- **Visual & Audio Feedback**:
  - Rapid alpha blink with red modulation (`Color(1f, 0.2f, 0.2f, 0.4f)`).
  - Plays `HitEffect` (`playerHit.wav`).
- **Defeat Momentum**:
  - `EnemyDying` applies gravity and deceleration throughout the defeat animation rather than abruptly freezing velocity.

### 5. Animation SpeedScale Resilience
- In `WithSynchronizedAnimations.cs`, `SpeedScale` defaults to `1.0f` when `direction.Length() == 0`, preventing stationary states from freezing playback.

## Risks / Trade-offs

- **[Risk] Interruption by damage (`PlayerHurt`)**: Player could get hit mid-swing, leaving the attack hitbox active if disabled only on animation completion.
  - **Mitigation**: Disable hitbox explicitly in `Exit()`, ensuring clean cleanup regardless of exit transition.
- **[Risk] Landing mid-air attack**: Player might start an attack in the air and land on the ground before the swing finishes.
  - **Mitigation**: Continue the attack animation seamlessly; physics update detects floor contact and switches from aerial gravity to grounded deceleration without cutting the animation short.
- **[Risk] Accidental buffered double-swings**:
  - **Mitigation**: Restrict input buffering detection strictly to recovery frames (`Frame >= 3`), ignoring premature button presses during initial windup.
- **[Risk] Multi-hitting in one swing**:
  - **Mitigation**: `Hurtbox.IsInvulnerable = true` during hitstun window.
