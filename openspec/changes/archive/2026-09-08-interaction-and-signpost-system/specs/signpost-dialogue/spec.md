# Signpost and Dialogue System Specification

## Purpose
Implements readable environmental signposts throughout the world and a bottom-anchored in-game dialogue banner with typewriter text streaming, fast-forward skip, and dismiss capabilities.

## Requirements

### Requirement: Signpost Entity
The system SHALL provide a `Signpost.tscn` world entity that level designers can place in overworld and dungeon rooms with custom text.

#### Scenario: Inspecting a signpost
- **GIVEN** a `Signpost` instance placed in a room with a configured multiline `Message` property
- **WHEN** the player interacts with the signpost
- **THEN** the signpost invokes the dialogue UI with its message content.

### Requirement: Dialogue Banner Presentation & Lifecycle
The system SHALL provide a `DialogueBox.tscn` UI component inside `Hud.tscn` anchored to the bottom of the screen.

#### Scenario: Displaying a dialogue message
- **GIVEN** a dialogue message string is passed to `DialogueBox.Open(string text)`
- **WHEN** the dialogue box opens
- **THEN** the banner frame appears anchored at the bottom of the viewport
- **AND** text reveals progressively using a typewriter effect
- **AND** a prompt indicator indicates the confirm/continue button.

#### Scenario: Fast-forwarding text reveal
- **GIVEN** the dialogue box is actively streaming characters via typewriter effect
- **WHEN** the player presses the confirm action (Button A)
- **THEN** all remaining characters in the current message are revealed immediately.

#### Scenario: Dismissing dialogue and restoring control
- **GIVEN** the dialogue box text has completed its reveal
- **WHEN** the player presses the confirm action (Button A)
- **THEN** the dialogue box closes
- **AND** emits `DialogueClosed` signal
- **AND** the player transitions out of `PlayerInteracting` back to `PlayerStanding`.
