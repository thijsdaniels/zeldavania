## MODIFIED Requirements

### Requirement: Aerial Physics & Jumping
The player controller SHALL support upward jumping velocity, variable jump height control (jump cut), coyote time, jump buffering, and gravity acceleration.

#### Scenario: Executing a jump
- **GIVEN** the player is in `PlayerStanding` or `PlayerRunning` state on the floor
- **WHEN** the Jump action (Button A) is pressed
- **THEN** the player transitions to `PlayerJumping`
- **AND** plays the jump sound effect
- **AND** sets vertical velocity upward
- **AND** immediately transitions to `PlayerFalling`.

#### Scenario: Executing a variable jump with button release
- **GIVEN** the player has executed a jump and is ascending in `PlayerFalling` (`Velocity.Y < 0`)
- **WHEN** the player releases the Jump button (`Controller.A`) before reaching the jump apex
- **THEN** upward velocity is smoothly dampened/cut (e.g. multiplied by `_jumpCutMultiplier = 0.5f`)
- **AND** the player reaches the apex sooner for a shorter hop.

#### Scenario: Full jump height on held jump button
- **GIVEN** the player has executed a jump and is ascending in `PlayerFalling` (`Velocity.Y < 0`)
- **WHEN** the player continues to hold the Jump button (`Controller.A`)
- **THEN** upward velocity decays naturally under standard gravity
- **AND** the player achieves maximum jump height.

#### Scenario: Ledge jump within coyote time window
- **GIVEN** the player walks or runs off a ledge from `PlayerStanding` or `PlayerRunning` into `PlayerFalling` without jumping
- **AND** elapsed air time is within the configured `_coyoteTime` duration (`0.1s`)
- **WHEN** the Jump button (`Controller.A`) is pressed
- **THEN** the state machine transitions to `PlayerJumping`
- **AND** standard jump impulse is applied
- **AND** the coyote time window is immediately invalidated
- **AND** air jump charges (`_airJumpsRemaining`) are not consumed.

#### Scenario: Coyote time expiration
- **GIVEN** the player has walked off a ledge into `PlayerFalling`
- **AND** the elapsed air time exceeds the configured `_coyoteTime` (`0.1s`)
- **WHEN** the Jump button (`Controller.A`) is pressed
- **THEN** standard airborne rules apply (consuming an air jump if available, or buffering the input).

#### Scenario: Jump buffering before landing
- **GIVEN** the player is airborne in `PlayerFalling` descending toward the ground
- **AND** no air jumps are available or the player is within the jump buffer threshold
- **WHEN** the Jump button (`Controller.A`) is pressed within `_jumpBufferTime` (`0.1s`) before contacting the floor
- **THEN** the jump input is buffered
- **AND** upon contacting the floor (`IsOnFloor() == true` / entering `PlayerLanding`), the player immediately transitions to `PlayerJumping`
- **AND** the jump buffer is consumed.

#### Scenario: Jump buffer expiration
- **GIVEN** the player buffers a jump input in `PlayerFalling`
- **WHEN** the player does not contact the ground within the `_jumpBufferTime` window (`0.1s`)
- **THEN** the buffered jump expires and no jump is executed upon eventual touchdown.

#### Scenario: Falling and landing
- **GIVEN** the player is in `PlayerFalling` state in the air
- **WHEN** the character body contacts a floor (`IsOnFloor() == true`)
- **THEN** the player transitions to `PlayerLanding`
- **AND** subsequently transitions to `PlayerRunning` (if moving) or `PlayerStanding` (if idle).
