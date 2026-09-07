## MODIFIED Requirements

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
