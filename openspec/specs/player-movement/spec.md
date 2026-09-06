# Player Movement Specification

## Purpose
Controls 2D side-scrolling platforming mechanics for the player character, including grounded movement with inertia, jumping, falling, crouching, rolling, ladder climbing, swimming, and one-way platform traversal.

## Requirements

### Requirement: Grounded Movement & Inertia
The player controller SHALL smoothly accelerate and decelerate horizontally using inertia calculations.

#### Scenario: Standing to running
- **GIVEN** the player is in `PlayerStanding` state on the floor
- **WHEN** horizontal input direction is non-zero
- **THEN** the player transitions to `PlayerRunning` state
- **AND** the player accelerates toward maximum speed limit using horizontal inertia
- **AND** the animated sprite synchronizes its animation speed and flips horizontally based on movement direction.

#### Scenario: Running to standing
- **GIVEN** the player is in `PlayerRunning` state
- **WHEN** horizontal input is released (direction is 0)
- **THEN** the player decelerates to 0
- **AND** transitions to `PlayerStanding` state when stopped.

### Requirement: Aerial Physics & Jumping
The player controller SHALL support upward jumping velocity and gravity acceleration.

#### Scenario: Executing a jump
- **GIVEN** the player is in `PlayerStanding` or `PlayerRunning` state on the floor
- **WHEN** the Jump action (Button A) is pressed
- **THEN** the player transitions to `PlayerJumping`
- **AND** plays the jump sound effect
- **AND** sets vertical velocity upward
- **AND** immediately transitions to `PlayerFalling`.

#### Scenario: Falling and landing
- **GIVEN** the player is in `PlayerFalling` state in the air
- **WHEN** the character body contacts a floor (`IsOnFloor() == true`)
- **THEN** the player transitions to `PlayerLanding`
- **AND** subsequently transitions to `PlayerRunning` (if moving) or `PlayerStanding` (if idle).

### Requirement: Crouch, Roll & One-Way Platform Traversal
The player controller SHALL support crouching, rolling under obstacles, and dropping through one-way collision floors.

#### Scenario: Crouching
- **GIVEN** the player is standing on the floor
- **WHEN** the Down action is pressed
- **THEN** the player transitions to `PlayerCrouching`
- **WHEN** the Down action is released
- **THEN** the player transitions back to `PlayerStanding`.

#### Scenario: Rolling
- **GIVEN** the player is running or crouching
- **WHEN** the roll action is triggered while maintaining velocity
- **THEN** the player enters `PlayerRolling` with adjusted collision height
- **AND** returns to standing/running upon completion.

#### Scenario: Dropping through one-way platforms
- **GIVEN** the player is standing on a one-way collision floor (Collision Mask 2)
- **WHEN** the Down action is just pressed
- **THEN** Collision Mask 2 is disabled and player Y-position is offset by +1 pixel to cross the one-way boundary
- **WHEN** the Down action is released
- **THEN** Collision Mask 2 is re-enabled.

### Requirement: Environmental Climbing & Swimming Transitions
The player controller SHALL transition to specialized movement states when interacting with ladders or water.

#### Scenario: Grabbing a ladder
- **GIVEN** the ladder detector is overlapping a ladder tile
- **WHEN** the Up action is pressed (or Down from above)
- **THEN** the player transitions to `PlayerClimbing`
- **AND** vertical velocity is driven by Up/Down inputs while disabling standard gravity.

#### Scenario: Entering water
- **GIVEN** the player enters an area detected by the water detector
- **WHEN** the player submerges
- **THEN** the player transitions to `PlayerSwimming`
- **AND** movement velocity is scaled with water drag and buoyancy rules.

### Requirement: Offensive Combat State Transitions
The system SHALL support transitioning from standard movement states to `PlayerAttacking` when the attack action is pressed.

#### Scenario: Attack input from standing or running
- **GIVEN** the player is in `PlayerStanding` or `PlayerRunning` state
- **WHEN** the Attack action (`Controller.X`) is just pressed
- **THEN** the state machine transitions immediately to `PlayerAttacking`.

#### Scenario: Attack input from falling
- **GIVEN** the player is in `PlayerFalling` state
- **WHEN** the Attack action (`Controller.X`) is just pressed
- **THEN** the state machine transitions immediately to `PlayerAttacking`.

