## MODIFIED Requirements

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

## ADDED Requirements

### Requirement: Passive Ability Item Inventory Integration
The player inventory system SHALL support registering, querying, and managing tiered passive ability items that modify gameplay capabilities without occupying active action slots (X, Y, B).

#### Scenario: Querying passive item tier levels
- **GIVEN** passive items are registered under `Player/Inventory`
- **WHEN** gameplay states query `_player.Inventory.GetPassiveTier(itemId)`
- **THEN** the inventory returns the item's configured `Tier` integer if unlocked, or `0` if not present/locked.
