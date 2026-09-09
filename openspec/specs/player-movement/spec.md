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
The player controller SHALL transition to specialized movement states when interacting with ladders or water bodies, supporting ladder climbing, surface swimming, timed diving, buoyant resurfacing, and free diving with waterfall traversal.

#### Scenario: Grabbing a ladder
- **GIVEN** the ladder detector is overlapping a ladder tile
- **WHEN** the Up action is pressed (or Down from above)
- **THEN** the player transitions to `PlayerClimbing`
- **AND** vertical velocity is driven by Up/Down inputs while disabling standard gravity.

#### Scenario: Entering water
- **GIVEN** the player enters an area detected by `WaterDetector2D` from airborne or grounded states
- **WHEN** the player enters water
- **THEN** the state machine transitions to `PlayerSwimming` (surface swimming)
- **AND** natural buoyancy holds the player at the surface waterline when idle.

#### Scenario: Surface swimming
- **GIVEN** the player is in `PlayerSwimming` at the surface
- **WHEN** horizontal directional input (`Left` / `Right`) is applied
- **THEN** the player accelerates horizontally using water inertia and surface swimming speed
- **AND** the animated sprite plays the swimming animation flipped in the movement direction.

#### Scenario: Breaching jump out of water onto ledges
- **GIVEN** the player is in `PlayerSwimming` (or `PlayerDiving`) at the water surface
- **AND** the Down action is NOT pressed
- **WHEN** the Jump action (`Controller.A`) is pressed
- **THEN** an upward breach launch impulse is applied (`Velocity.Y = -190 px/s`)
- **AND** the splash sound effect is played
- **AND** the state machine transitions to `PlayerFalling` with jump ascent notified
- **AND** the player trajectory clears 1–2 tile ledges adjacent to the water.

#### Scenario: Shallow timed dive with Scale Tier 1 (Silver Scale)
- **GIVEN** the player has `scale` at Tier 1 equipped in inventory
- **WHEN** the Down action is pressed in `PlayerSwimming`
- **THEN** an active dive force pushes the player down to shallow depth ($\approx 1.75$ tiles / $28\text{ px}$)
- **AND** holding Down maintains maximum depth until descent timer expires
- **WHEN** the dive timer expires OR the Down action is released
- **THEN** buoyant upward velocity is applied until the player reaches the waterline, returning to normal surface swimming.

#### Scenario: Deep timed dive with Scale Tier 2 (Golden Scale)
- **GIVEN** the player has `scale` at Tier 2 equipped in inventory
- **WHEN** the Down action is pressed in `PlayerSwimming`
- **THEN** an active dive force pushes the player down to deep depth ($\approx 4$ tiles / $64\text{ px}$) before buoyant resurfacing.

#### Scenario: Free 360-degree diving with Scale Tier 3 (Zora's Flippers)
- **GIVEN** the player has `scale` at Tier 3 equipped in inventory
- **WHEN** the player is swimming at the surface and presses Down
- **THEN** the state machine transitions into `PlayerDiving` (360° free diving)
- **AND** directional inputs steer the player directly with responsive speed (`_freeSwimSpeed = 110 px/s`)
- **AND** the sprite smoothly rotates towards the movement heading angle
- **AND** swim dashing (<kbd>A</kbd>) propels in the input direction or the last heading direction
- **WHEN** the player swims to the surface waterline with upward heading
- **THEN** the state machine smoothly transitions back to `PlayerSwimming`.

#### Scenario: Waterfall current resistance and waterfall climbing with Scale Tier 4 (Golden Flippers)
- **GIVEN** the player is inside a waterfall column
- **WHEN** the player does NOT possess `scale` at Tier 4
- **THEN** a downward current force pushes the player downward into the plunge basin below
- **WHEN** the player possesses `scale` at Tier 4 and holds Up in `PlayerDiving`
- **THEN** upward propulsion overcomes the waterfall current, enabling vertical ascent to the waterfall peak.

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

### Requirement: Environmental Interaction State Transitions
The system SHALL support transitioning from `PlayerStanding` to `PlayerInteracting` when interacting with an in-world object.

#### Scenario: Interacting with an object from standing
- **GIVEN** the player is in `PlayerStanding` state
- **AND** the player's interaction detector overlaps an active `Interactable2D`
- **WHEN** the Interact action (`Controller.A`) is just pressed
- **THEN** the state machine transitions immediately to `PlayerInteracting`
- **AND** player movement is locked until the interaction concludes.

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

### Requirement: Passive Ability Item Inventory Integration
The player inventory system SHALL support registering, querying, and managing tiered passive ability items that modify gameplay capabilities without occupying active action slots (X, Y, B).

#### Scenario: Querying passive item tier levels
- **GIVEN** passive items are registered under `Player/Inventory`
- **WHEN** gameplay states query `_player.Inventory.GetPassiveTier(itemId)`
- **THEN** the inventory returns the item's configured `Tier` integer if unlocked, or `0` if not present/locked.
