## ADDED Requirements

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
