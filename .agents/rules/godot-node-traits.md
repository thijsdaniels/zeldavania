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
