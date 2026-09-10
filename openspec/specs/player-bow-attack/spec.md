# Player Bow Attack Specification

## Purpose
Provides ranged combat mechanics allowing the player to aim and shoot arrows with hybrid controls, bullet-time slowdown, ballistics physics, and impact damage.

## Requirements

### Requirement: Hybrid Bow Aiming Input & State Transitions
The player system SHALL support quick-firing and hold-to-aim shooting mechanics via any action button (`Controller.X`, `Controller.Y`, or `Controller.B`) currently assigned to the Bow.

#### Scenario: Quick tap fires immediately
- **WHEN** the player presses and releases the action button assigned to the Bow in less than the aim threshold (0.15s) while standing, running, or falling
- **THEN** the player transitions to the shooting state and fires an arrow in the current facing direction at the remembered elevation angle

#### Scenario: Holding enters aim stance with bullet-time
- **WHEN** the player holds the action button assigned to the Bow for longer than the aim threshold (0.15s)
- **THEN** the player transitions to the aiming state, engine time scale is reduced to 0.3x, and the aiming crosshair becomes visible

#### Scenario: Releasing aim fires along aim vector
- **WHEN** the player releases the action button assigned to the Bow while in the aiming state
- **THEN** the engine time scale restores to 1.0x, the crosshair hides, and the player transitions to the shooting state, firing an arrow along the active aim direction


### Requirement: Aim Angle Adjustment & Memory
The aiming system SHALL allow continuous elevation angle adjustment and remember the angle across consecutive shots.

#### Scenario: Adjusting elevation with vertical input
- **WHEN** the player presses or holds Up or Down while in the aiming state
- **THEN** the elevation angle sweeps smoothly between -45 degrees (downward) and +90 degrees (straight up)

#### Scenario: Reversing horizontal facing preserves elevation
- **WHEN** the player inputs horizontal direction opposite to current facing while aiming
- **THEN** the character flips facing direction and mirrors the horizontal component of the aim vector while preserving the vertical elevation angle

#### Scenario: Consecutive shots reuse last elevation angle
- **WHEN** the player fires an arrow after adjusting the elevation angle and subsequently quick-taps the shoot button
- **THEN** the new arrow is fired at the previously adjusted elevation angle in the active facing direction

### Requirement: Walk-Aiming
The player SHALL be capable of slow horizontal movement while maintaining an aiming stance.

#### Scenario: Moving horizontally while aiming
- **WHEN** the player holds a horizontal movement input while grounded in the aiming state
- **THEN** the player moves at 50% of standard run speed while maintaining the active aim angle and crosshair position

### Requirement: Crosshair Visual Indicator
The aiming system SHALL display a visual crosshair indicating the current trajectory vector.

#### Scenario: Crosshair positioning
- **WHEN** the player is in the aiming state
- **THEN** the crosshair node is visible and rendered at a fixed radius from the player's center in the normalized aim direction

### Requirement: Arrow Projectile Flight Physics
The arrow projectile SHALL follow a two-phase trajectory: an initial high-speed flat flight phase followed by a ballistic gravity drop.

#### Scenario: Straight-line flat phase
- **WHEN** the arrow is instantiated and travels less than the straight distance threshold (e.g. 128 pixels)
- **THEN** the arrow travels at launch velocity along the aim vector with zero gravity acceleration

#### Scenario: Ballistic gravity phase
- **WHEN** the arrow travels beyond the straight distance threshold
- **THEN** downward gravity acceleration is applied to its vertical velocity, and the projectile rotation smoothly aligns with its velocity vector (`Atan2(vy, vx)`)

### Requirement: Arrow Impact and Despawn Lifecycle
The arrow projectile SHALL deal damage to enemies and embed into surfaces on collision before despawning.

#### Scenario: Arrow strikes enemy hurtbox
- **WHEN** the arrow hitbox collides with an enemy hurtbox (Layer 8)
- **THEN** damage is applied to the enemy's damageable component, the arrow stops physics movement, disables its hitbox, and despawns after an embed delay

#### Scenario: Arrow strikes solid terrain
- **WHEN** the arrow collides with a solid terrain collider (Layer 1)
- **THEN** the arrow embeds into the surface at its impact angle, stops physics movement, disables its hitbox, and despawns after a timeout (e.g. 1.5 seconds)

### Requirement: Fire Arrow Projectile Variant
The bow shooting system SHALL support firing a `FireArrow` projectile variant equipped with visual flame trail effects, dynamic light emission, and a pre-lit `Flammable2D` ignition payload.

#### Scenario: Firing a Fire Arrow
- **WHEN** the player shoots with a bow configured for the Fire Arrow variant
- **THEN** a `FireArrow` projectile is instantiated with active flame trail particles and a dynamic `LightEmitter2D` casting light as it travels

#### Scenario: Fire Arrow impacts flammable object
- **WHEN** an in-flight or embedded `FireArrow` contacts an unlit `Flammable2D` entity (such as an unlit `Torch`)
- **THEN** the fire arrow ignites the `Flammable2D` entity before embedding or despawning

#### Scenario: Fire Arrow enters water
- **WHEN** an in-flight `FireArrow` enters a `WaterVolume` area
- **THEN** its flame is extinguished and its `LightEmitter2D` turns off

### Requirement: Bow Arrow Variant Selection and Cycling
The inventory and bow item system SHALL support selecting and cycling arrow variants for gameplay and debugging.

#### Scenario: Cycling bow arrow variants
- **WHEN** the player cycles through bow equipment variants in inventory/debug mode
- **THEN** the active bow cycles through `No Bow` -> `Bow (Standard Arrows)` -> `Bow (Fire Arrows)` -> `No Bow`, updating the projectile scene spawned during shooting

