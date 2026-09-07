# Tasks: Interaction and Signpost System

## 1. Core Interaction Components
- [x] 1.1 Create `scripts/utilities/Interactable2D.cs` with `Interacted`, `PlayerEntered`, `PlayerExited` signals, exported prompt settings, and prompt cue toggle logic.
- [x] 1.2 Create `scenes/utilities/Interactable2D.tscn` with `CollisionShape2D` on layer 10 and a floating `PromptCue` indicator with smooth floating/bounce animation.
- [x] 1.3 Create `scripts/utilities/InteractionDetector2D.cs` monitoring layer 10 and tracking the closest active `Interactable2D`.
- [x] 1.4 Create `scenes/utilities/InteractionDetector2D.tscn` and attach it to `scenes/entities/Player.tscn`.

## 2. Player State Machine & Interaction Lifecycle
- [x] 2.1 Create `scripts/entities/player/states/PlayerInteracting.cs` extending `State` to freeze velocity, play `Idle`, and transition back to `PlayerStanding` on interaction close.
- [x] 2.2 Wire `PlayerInteracting` into `scenes/entities/Player.tscn` under `State/Interacting`.
- [x] 2.3 Update `scripts/entities/player/states/PlayerStanding.cs` to check `_interactionDetector.CanInteract` on `Input.IsActionJustPressed(Controller.A)` and trigger `Interact(player)` before transitioning to `PlayerInteracting`.

## 3. Dialogue Box UI
- [x] 3.1 Create `scripts/userInterface/DialogueBox.cs` with typewriter text reveal, skip-to-end, and close action on `Controller.A`.
- [x] 3.2 Create `scenes/userInterface/DialogueBox.tscn` with bottom-anchored styling matching HUD / PauseMenu aesthetics.
- [x] 3.3 Embed `DialogueBox.tscn` into `scenes/userInterface/Hud.tscn` and expose helper methods / signals in `Hud.cs`.

## 4. Signpost Entity
- [x] 4.1 Create `scripts/objects/Signpost.cs` with exported multiline `Message` property and connection to open dialogue on `Interacted`.
- [x] 4.2 Create `scenes/objects/Signpost.tscn` using `assets/textures/objects/Signpost.png`, `StaticBody2D` base collision, and child `Interactable2D`.

## 5. Level Integration & Verification
- [x] 5.1 Place a test `Signpost` instance in `scenes/environments/World.tscn` with a descriptive test message.
- [x] 5.2 Build the C# project with `dotnet build` and ensure 0 errors and 0 warnings.
- [x] 5.3 Verify interaction prompt visibility when approaching the signpost, dialogue opening on `(A)`, typewriter fast-forward, and clean player control restoration on dismissal.
