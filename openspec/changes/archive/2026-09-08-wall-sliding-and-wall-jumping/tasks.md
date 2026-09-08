## 1. Sprite Asset & State Scripts

- [x] 1.1 Add the crafted 32x32 `WallSlide` sprite frame to `assets/textures/characters/` and register the `"WallSlide"` animation in `Player.tscn`'s `SpriteFrames`.
- [x] 1.2 Create `PlayerWallSliding.cs` in `scripts/entities/player/states/` implementing slow descent friction (`_slideTerminalVelocity`), active directional holding check against wall normal, coyote-time buffering, playing `"WallSlide"` animation, and transitions to jumping, falling, and landing.
- [x] 1.3 Create `PlayerWallJumping.cs` in `scripts/entities/player/states/` calculating outward diagonal launch velocity (`_horizontalImpulse`, `_verticalImpulse`), triggering jump sound effect, and applying directional input lockout.

## 2. State Machine Integration & Lockout Logic

- [x] 2.1 Update `PlayerFalling.cs` to add `[Export] private State _wallSlidingState`, wall contact checks (`IsOnWall()`), and transition to wall sliding when falling downwards while pushing into a wall.
- [x] 2.2 Implement input lockout helper/timer in `PlayerFalling.cs` to prevent immediate counter-steering towards the wall during the initial wall-jump launch arc.

## 3. Scene & Inspector Wiring

- [x] 3.1 Instantiate `PlayerWallSliding` and `PlayerWallJumping` nodes under `FiniteStateMachine` in `scenes/entities/Player.tscn` and wire all exported node references and audio stream players.
- [x] 3.2 Configure exported transitions between `PlayerFalling`, `PlayerWallSliding`, `PlayerWallJumping`, and `PlayerLanding`.

## 4. Verification & Playtesting

- [x] 4.1 Build the project with `dotnet build` to ensure zero compilation errors and clean type safety.
- [x] 4.2 Verify in gameplay that the player can smoothly slide down walls by holding into them, detach cleanly by releasing input, and scale opposing parallel wall shafts via wall jumps.

