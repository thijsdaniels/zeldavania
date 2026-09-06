## ADDED Requirements

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
