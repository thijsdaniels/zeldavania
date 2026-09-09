---
trigger: model_decision
description: When designing, implementing, or refactoring gameplay systems, entity traits, interactions, and component architectures in Godot.
---

# Godot Node & Systemic Trait Architecture

Always favor **Godot native Node and Scene composition** over abstract C# interfaces or standalone non-Node class hierarchies.

## 1. Core Principles

- **Concrete Node Components over C# Interfaces**:
  - Do not create parallel C# interfaces (such as `IInteractable`, `IDamageable`, `ISwimmable`) when a dedicated, self-contained Godot Node/Scene component (e.g. `Interactable2D`, `Damageable`, `Hurtbox`, `Hitbox`, `WaterDetector2D`) provides the functionality.
  - Entities gain capabilities by adding reusable child nodes/scenes directly in the Godot inspector.

- **Self-Contained & Inspector-First**:
  - Components should encapsulate their own collision shapes, detection areas, visual cues (e.g. prompt indicators), and internal state.
  - Expose configuration via `[Export]` properties with sensible defaults and tooltips.
  - Communicate outwards using Godot Signals (e.g. `[Signal] public delegate void InteractedEventHandler(Player player);`).

- **Querying & Detecting Components**:
  - Query nodes directly via type checks (`node is Interactable2D`) or standard Godot node APIs (`GetNode<T>()`, `GetOverlappingAreas()`).
  - Avoid creating redundant boilerplate wrapper classes or abstract layers.

---

## 2. Finite State Machine Architecture: Granular States over Monolithic States

Always prefer **multiple focused, single-responsibility `State` nodes** over a single large "monolithic" state with complex internal modes, booleans, and branching.

- **Single Responsibility per State**:
  - Each state class should model a distinct physical, behavioral, or motion phase (e.g. separate `PlayerPaddling`, `PlayerDiving`, `PlayerResurfacing`, `PlayerSwimming`, or `PlayerWallSliding` / `PlayerWallJumping`).
  - Eliminate internal mode flags (such as `_isDiving`, `_isResurfacing`, `_isCharging`, `_isGliding`) by promoting each distinct phase to its own `State` node in the entity's `FiniteStateMachine`.
- **Explicit Transitions & Lifecycle**:
  - Leverage the state lifecycle (`Enter()` and `Exit()`) for animations, sound effects, input lockouts, and property modifications rather than conditional branches inside `UpdatePhysics()`.
  - Let the state machine enforce mutual exclusivity between movement modes to prevent input priority collisions, state contamination, and subtle transition bugs.
- **Inspector Wiring & Composability**:
  - Wire target states explicitly via `[Export] private State _targetState;`. This keeps entity capabilities modular, inspectable, and individually tunable in the Godot editor.
