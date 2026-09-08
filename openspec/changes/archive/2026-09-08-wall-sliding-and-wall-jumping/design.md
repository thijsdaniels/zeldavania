# Design: Wall Sliding and Wall Jumping

## 1. Node Hierarchy & State Machine Integration

The two new states `PlayerWallSliding` and `PlayerWallJumping` are added under `Player`'s `FiniteStateMachine` node in `scenes/entities/Player.tscn`:

```
Player (CharacterBody2D, Player.cs)
├── AnimatedSprite2D
├── CollisionShape2D
├── FiniteStateMachine
│   ├── PlayerStanding
│   ├── PlayerRunning
│   ├── PlayerJumping
│   ├── PlayerFalling
│   ├── PlayerLanding
│   ├── PlayerWallSliding (NEW)
│   ├── PlayerWallJumping (NEW)
│   ├── ...
```

---

## 2. Component Design & State Implementations

### A. `PlayerWallSliding.cs`
Inherits `State`.

#### Export Fields:
- `[Export] private CharacterBody2D _body;`
- `[Export] private Player _player;`
- `[Export] private AnimatedSprite2D _sprite;`
- `[ExportGroup("Wall Sliding")]`
  - `[Export] private float _slideGravity = 300f;`
  - `[Export] private float _slideTerminalVelocity = 45f;`
  - `[Export] private float _coyoteTime = 0.08f;`
- `[ExportGroup("Transitions")]`
  - `[Export] private State _fallingState;`
  - `[Export] private State _landingState;`
  - `[Export] private State _wallJumpingState;`

#### Behavior:
- **`Enter()`**:
  - Plays the `"WallSlide"` animation on `_sprite` (custom pixel-crafted frame featuring outward-facing slide pose with feet and hand against wall).
  - Determines active wall normal from `_body.GetWallNormal()`.
  - Orients sprite so back faces the wall (`_sprite.FlipH = wallNormal.X > 0`).

- **`UpdatePhysics(delta)`**:
  - Item attack check (`this.TryTriggerItemAction(_player, out targetState)`).
  - Jump input check: if `Input.IsActionJustPressed(Controller.A)`, transition to `_wallJumpingState`.
  - Floor check: if `_body.IsOnFloor()`, transition to `_landingState`.
  - Wall contact & input check:
    - If `!_body.IsOnWall()` or player is not holding horizontal input towards the wall (`Mathf.Sign(inputDir) != -Mathf.Sign(wallNormal.X)`):
      - Start/decrement coyote timer. If coyote timer expires, transition to `_fallingState`.
  - Physics movement:
    - Cap vertical velocity at `_slideTerminalVelocity`.
    - Apply small horizontal push into the wall to maintain contact with `CharacterBody2D.MoveAndSlide()`.

---

### B. `PlayerWallJumping.cs`
Inherits `State`.

#### Export Fields:
- `[Export] private CharacterBody2D _body;`
- `[Export] private Player _player;`
- `[Export] private AnimatedSprite2D _sprite;`
- `[ExportGroup("Wall Jump Parameters")]`
  - `[Export] private float _verticalImpulse = 200f;`
  - `[Export] private float _horizontalImpulse = 130f;`
  - `[Export] private float _lockoutDuration = 0.15f;`
  - `[Export] private AudioStreamPlayer2D _soundEffect;`
- `[ExportGroup("Transitions")]`
  - `[Export] private State _fallingState;`

#### Behavior:
- **`Enter()`**:
  - Fetches cached or active wall normal $N$ from `PlayerWallSliding`.
  - Sets initial impulse:
    - `_body.Velocity = new Vector2(N.X * _horizontalImpulse, -_verticalImpulse);`
  - Plays sound effect (`playerJump.wav` or dedicated wall jump audio).
  - Flips sprite away from the wall (`_sprite.FlipH = N.X < 0`).
  - Sets lockout timer (`_lockoutRemaining = _lockoutDuration`).
  - Transitions immediately to `_fallingState` with lockout metadata passed, or executes lockout phase within `PlayerWallJumping` / `PlayerFalling`.
  - *Clean Pattern:* Pass lockout duration to `PlayerFalling.SetInputLockout(float duration, float lockedDirection)` or let `PlayerFalling` dampen counter-steering during the lockout window.

---

### C. Updates to Existing States

#### `PlayerFalling.cs`:
- Add `[ExportGroup("Wall Sliding")]`:
  - `[Export] private State _wallSlidingState;`
- Add input lockout handling:
  - `private float _inputLockoutTimer;`
  - `private float _lockedDirection;`
  - When lockout active, ignore or clamp horizontal input pushing back towards `_lockedDirection`.
- In `UpdatePhysics(delta)` transition checks:
  - When `_body.IsOnWall() && _body.Velocity.Y >= 0`:
    - Let $N = \_body.GetWallNormal()$.
    - Check if player is holding input into the wall: `Mathf.Sign(horizontalInput) == -Mathf.Sign(N.X)`.
    - If true, transition to `_wallSlidingState`.
