# Tasks: Jump Mechanics Polish

## 1. PlayerFalling Jump & Airborne Polish
- [x] 1.1 Add exported tuning fields to `PlayerFalling.cs` (`_jumpingState`, `_coyoteTime`, `_jumpBufferTime`, `_jumpCutMultiplier`).
- [x] 1.2 Implement `EnableCoyoteTime()` and `NotifyJumpAscent()` helper methods on `PlayerFalling.cs`.
- [x] 1.3 Implement variable jump cut logic in `PlayerFalling.UpdatePhysics` during ascent when `Controller.A` is released.
- [x] 1.4 Implement coyote jump triggering in `PlayerFalling.UpdatePhysics` when `Controller.A` is pressed while `_coyoteTimer > 0`.
- [x] 1.5 Implement jump buffer activation in `PlayerFalling.UpdatePhysics` when `Controller.A` is pressed without air jumps.
- [x] 1.6 Implement immediate jump execution upon landing if `_jumpBufferTimer > 0`.

## 2. State Machine Coordination
- [x] 2.1 Update `PlayerJumping.cs` to notify `PlayerFalling` of jump ascent on transition.
- [x] 2.2 Update `PlayerStanding.cs` to enable coyote time when stepping off ledges into `PlayerFalling`.
- [x] 2.3 Update `PlayerRunning.cs` to enable coyote time when running off ledges into `PlayerFalling`.

## 3. Scene Wiring & Tuning
- [x] 3.1 Update `scenes/entities/Player.tscn` to wire `_jumpingState` on `PlayerFalling` node.
- [x] 3.2 Verify exported parameters (`_coyoteTime = 0.1`, `_jumpBufferTime = 0.1`, `_jumpCutMultiplier = 0.5`) in `Player.tscn`.

## 4. Verification & Testing
- [x] 4.1 Build the C# Godot project to ensure zero compilation errors and warnings.
- [x] 4.2 Verify tap vs. hold jump height behavior (short hop on tap, full height on hold).
- [x] 4.3 Verify ledge coyote jump window allows jumping slightly after stepping off platforms without consuming air jumps.
- [x] 4.4 Verify jump buffering triggers a seamless jump immediately upon touching the ground when pressed just before landing.
