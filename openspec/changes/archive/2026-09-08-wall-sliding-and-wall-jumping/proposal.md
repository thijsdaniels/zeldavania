# Change Proposal: Wall Sliding and Wall Jumping

## Summary
Implement wall sliding and wall jumping mechanics for the player character to enable vertical shaft climbing, acrobatic traversal, and ability-gated Metroidvania level progression.

## Problem Statement
The player currently possesses standard grounded jumping and air jumping, but cannot interact with vertical wall surfaces. When encountering vertical cliffs or shafts, the player falls helplessly down walls at full terminal velocity. Vertical traversal in shafts requires ladders or moving platforms rather than responsive, acrobatic platforming.

## Proposed Solution
Introduce two new player states within the `FiniteStateMachine`:
1. **`PlayerWallSliding`**:
   - Triggered when the player is airborne, falling downwards (`Velocity.Y >= 0`), colliding with a wall (`_body.IsOnWall()`), and actively holding the horizontal input **towards the wall**.
   - Reduces downward terminal velocity to a gentle slide speed (e.g. `45 px/s`).
   - Supports wall coyote-time buffer when detaching from the wall.
   - Emits wall friction dust particles.
2. **`PlayerWallJumping`**:
   - Triggered when pressing the Jump action (`Controller.A`) while wall sliding or within the coyote buffer.
   - Launches the player diagonally upward and away from the wall (`Vy = -200`, `Vx = wallNormal.X * 130`).
   - Applies a brief horizontal input lockout / damping window (~`0.15s`) to ensure a clean launch arc and prevent single-wall climbing (requiring two opposing walls to gain height).
   - Plays a distinct jump sound effect and particle puff.

## Non-Goals
- Single-wall vertical scaling / infinite single-wall climb (wall jump arc is specifically designed to require two opposing walls to gain height).
- Ledge grabbing / mantling (tracked separately in Issue #32).
- Air dashing (tracked separately in Issue #33).


## References
- GitHub Issue: Refs #31 (`Implement Wall Sliding and Wall Jumping States`)
