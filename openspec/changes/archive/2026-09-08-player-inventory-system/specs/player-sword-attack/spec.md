## MODIFIED Requirements

### Requirement: Attack Execution & State Entry
The system SHALL transition the player into the `PlayerAttacking` state when any action button (`Controller.X`, `Controller.Y`, or `Controller.B`) assigned to a melee weapon is triggered from valid states, playing the weapon's configured attack animation.

#### Scenario: Attacking while standing
- **GIVEN** the player is in `PlayerStanding` state on the floor
- **WHEN** an action button assigned to the Sword is pressed
- **THEN** the player transitions to `PlayerAttacking` state
- **AND** the configured attack animation (e.g. `"Sword"`) plays on the animated sprite.

#### Scenario: Attacking while running
- **GIVEN** the player is in `PlayerRunning` state on the floor
- **WHEN** an action button assigned to the Sword is pressed
- **THEN** the player transitions to `PlayerAttacking` state
- **AND** the configured attack animation plays on the animated sprite.

#### Scenario: Attacking while airborne
- **GIVEN** the player is in `PlayerFalling` state in the air
- **WHEN** an action button assigned to the Sword is pressed
- **THEN** the player transitions to `PlayerAttacking` state
- **AND** the configured attack animation plays on the animated sprite.
