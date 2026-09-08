# Proposal: Jump Mechanics Polish (Variable Height, Coyote Time, Jump Buffering)

## Summary
Polish the core jump and aerial platforming feel by implementing three foundational game feel mechanics:
1. **Variable Jump Height (Jump Cut)**: Allows players to control jump apex by releasing the jump button early (short tap vs. full hold).
2. **Coyote Time**: Provides a brief grace window (e.g. 100ms) after walking/running off a ledge where a jump input is still recognized as a ground jump.
3. **Jump Buffering**: Queues jump inputs pressed slightly before touching the ground (e.g. 100ms window) and executes the jump immediately upon landing.

## Problem Statement
Currently, jumping in `PlayerJumping.cs` and `PlayerFalling.cs` feels rigid and unforgiving:
- **Instant Fixed Impulse (Issue #1)**: Jumping applies a single fixed impulse (`-180 px/s`) and immediately falls. Tapping or holding the jump button produces the exact same jump height, making precision vertical platforming difficult.
- **Missed Ledge Jumps (Issue #2)**: As soon as the player walks off a ledge, `IsOnFloor()` becomes `false`. Pressing jump a frame or two after stepping off either expends an air jump (if available) or is ignored entirely, causing missed jumps.
- **Lost Landing Inputs (Issue #3)**: Pressing the jump button while descending shortly before contacting the floor is ignored because the player is still in `PlayerFalling`, requiring frame-perfect timing to jump on touchdown.

## Proposed Solution
- **Variable Jump Height / Jump Cut (`PlayerFalling.cs` & `PlayerJumping.cs`)**:
  - While ascending in `PlayerFalling` (`Velocity.Y < 0`), if the jump action (`Controller.A`) is released before reaching the apex, apply a jump cut deceleration or multiplier (e.g. `_jumpCutMultiplier = 0.5f` or cap upward velocity) to allow fine-grained height control.
- **Coyote Time (`PlayerFalling.cs`)**:
  - When entering `PlayerFalling` from a grounded state (`PlayerStanding` / `PlayerRunning`) without jumping, start a configurable `_coyoteTimer` (default `0.1s`).
  - If the jump action (`Controller.A`) is pressed while `_coyoteTimer > 0`, execute a ground jump (transitioning to `PlayerJumping` or applying jump impulse directly) and consume the coyote window without decrementing air jump charges.
- **Jump Buffering (`PlayerFalling.cs` & `PlayerLanding.cs`)**:
  - When the jump action (`Controller.A`) is pressed in `PlayerFalling` without triggering an air jump (or when air jumps are exhausted / near ground), activate a `_jumpBufferTimer` (default `0.1s`).
  - Upon landing on the floor or entering `PlayerLanding`, if `_jumpBufferTimer > 0`, immediately transition to `PlayerJumping`, consuming the buffer.

## Non-Goals
- Adding new movement abilities like double jump upgrades or dash abilities (tracked separately in #33).
- Modifying wall sliding or wall jumping physics (already handled and validated).

## Related Issues
- Closes: #1 (Implement Variable Jump Height)
- Closes: #2 (Implement Coyote Time for Responsive Platforming)
- Closes: #3 (Implement Jump Buffering for Platforming Landing)
