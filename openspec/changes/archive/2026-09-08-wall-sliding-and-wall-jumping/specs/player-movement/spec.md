## ADDED Requirements

### Requirement: Wall Sliding State & Friction Traversal
The player controller SHALL support sliding down vertical walls with reduced descent velocity when actively pressing movement inputs into a solid wall while airborne.

#### Scenario: Entering wall slide while falling against a wall
- **GIVEN** the player is in `PlayerFalling` with downward velocity (`Velocity.Y >= 0`)
- **AND** the character body contacts a solid wall (`IsOnWall() == true`)
- **WHEN** the player actively presses the horizontal movement input towards the wall
- **THEN** the state machine transitions to `PlayerWallSliding`
- **AND** vertical descent speed is capped at the wall slide terminal velocity (`45 px/s`).

#### Scenario: Exiting wall slide by releasing directional input or wall separation
- **GIVEN** the player is in `PlayerWallSliding`
- **WHEN** the player releases horizontal input, presses away from the wall, or slides past the bottom edge of the wall
- **THEN** the player transitions back to `PlayerFalling` with standard gravity restored.

#### Scenario: Landing on the floor from a wall slide
- **GIVEN** the player is in `PlayerWallSliding`
- **WHEN** the character body contacts the ground (`IsOnFloor() == true`)
- **THEN** the player transitions to `PlayerLanding` (or `PlayerStanding` / `PlayerRunning`).

### Requirement: Wall Jumping Launch Mechanics & Opposing Wall Traversal
The player controller SHALL launch diagonally upward and away from a wall when pressing the Jump action while wall sliding.

#### Scenario: Executing a wall jump from wall sliding
- **GIVEN** the player is in `PlayerWallSliding` against a wall with normal vector $N$
- **WHEN** the Jump action (`Controller.A`) is pressed
- **THEN** the state machine transitions to `PlayerWallJumping`
- **AND** the player's velocity is set to a diagonal vector away from the wall (`Velocity.X = N.X * JumpKickSpeed`, `Velocity.Y = -JumpVerticalSpeed`)
- **AND** horizontal directional input towards the old wall is locked out for a short duration (`0.15s`) to preserve outward trajectory arc
- **AND** the wall jump sound effect is played
- **AND** the player transitions to `PlayerFalling` upon completing the launch impulse.

#### Scenario: Wall jump coyote time
- **GIVEN** the player slips off a wall edge or releases the wall direction within the configured coyote threshold (`0.08s`)
- **WHEN** the Jump action is pressed
- **THEN** the wall jump is accepted and executes normally using the cached wall normal vector.
