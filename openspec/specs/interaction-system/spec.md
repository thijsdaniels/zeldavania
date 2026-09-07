# Interaction System Specification

## Purpose
Provides a generic, modular Godot node-based interaction system enabling the player to detect, prompt, and interact with environmental objects (signposts, chests, doors, NPCs) and locks player movement in a dedicated interaction state.

## Requirements

### Requirement: Interactable2D Node Component
The system SHALL provide a reusable `Interactable2D` (`Area2D`) node component that entities can attach as a child to become interactable.

#### Scenario: Player enters interactable range
- **GIVEN** an entity with an active `Interactable2D` node in the scene
- **WHEN** the player's interaction detector overlaps the `Interactable2D` collision shape
- **THEN** `Interactable2D` emits `PlayerEntered` signal
- **AND** displays its floating world-space prompt indicator (e.g. `[A]` cue) above the object.

#### Scenario: Player exits interactable range
- **GIVEN** the player's interaction detector was overlapping an `Interactable2D`
- **WHEN** the player moves outside the `Interactable2D` collision area
- **THEN** `Interactable2D` emits `PlayerExited` signal
- **AND** hides its floating prompt indicator.

#### Scenario: Interacting with an active object
- **GIVEN** the player is overlapping an active `Interactable2D`
- **WHEN** `Interact(Player player)` is invoked
- **THEN** `Interactable2D` emits `Interacted(player)` signal to notify connected listener scripts or parent nodes.

### Requirement: Player Interaction Detection & State Transition
The player controller SHALL detect nearby `Interactable2D` components and transition into `PlayerInteracting` state upon pressing the interact action.

#### Scenario: Triggering interaction from PlayerStanding
- **GIVEN** the player is in `PlayerStanding` state on the ground
- **AND** `InteractionDetector2D` has an active overlapping `Interactable2D`
- **WHEN** the interact action (Button A) is pressed
- **THEN** the interaction detector triggers `Interact(player)` on the target `Interactable2D`
- **AND** the player state machine transitions immediately to `PlayerInteracting`.

#### Scenario: Player movement lock during interaction
- **GIVEN** the player enters `PlayerInteracting` state
- **THEN** horizontal and vertical velocity are set to `Vector2.Zero`
- **AND** the sprite plays the `Idle` animation
- **AND** directional movement and jump inputs are suspended.

#### Scenario: Completing interaction and restoring control
- **GIVEN** the player is in `PlayerInteracting` state
- **WHEN** the active interaction finishes (e.g. dialogue dismissed or interaction closed)
- **THEN** the state machine transitions back to `PlayerStanding`
- **AND** standard player movement controls are restored.
