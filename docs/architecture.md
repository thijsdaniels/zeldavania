# Zeldavania - Architecture & Coding Guidelines

This document outlines the architectural patterns, code conventions, and design principles for the Zeldavania codebase.

---

## 1. Technology Stack & Tooling

- **Engine:** Godot Engine 4.7+ (.NET / C# edition)
- **Runtime:** .NET 8.0 (`Zeldavania.csproj`)
- **Code Formatter:** [CSharpier](https://csharpier.com/) (config in `.csharpierrc.yaml`)
  - `printWidth: 80`
  - `tabWidth: 4`
  - `useTabs: false`
- **Spec Framework:** OpenSpec (`openspec/` and `.agents/`)

---

## 2. 2D Physics & Collision Architecture

Collision layers in `project.godot` are partitioned into **World Terrain**, **Combat Areas**, and **Entity Bodies**:

| Layer # | Layer Name | Bit Value | Type | Purpose / Usage |
| :--- | :--- | :--- | :--- | :--- |
| **1** | `Solids` | 1 | Terrain | Static terrain, solid tiles, boundaries |
| **2** | `Platforms` | 2 | Terrain | One-way drop-through platforms |
| **3** | `Liquids` | 4 | Environmental | Water and fluid detection areas |
| **4** | `Climbables` | 8 | Environmental | Ladders and climbable zones |
| **5** | `PlayerHurtbox` | 16 | Combat Area | Player damage receiver (`Hurtbox` Area2D) |
| **6** | `EnemyHitbox` | 32 | Combat Area | Enemy damage dealer (`Hitbox` Area2D) |
| **7** | `PlayerHitbox` | 64 | Combat Area | Player attack area (`Hitbox` Area2D) |
| **8** | `EnemyHurtbox` | 128 | Combat Area | Enemy damage receiver (`Hurtbox` Area2D) |
| **9** | `Player` | 256 | Entity Body | Player `CharacterBody2D` |
| **10** | `Enemy` | 512 | Entity Body | Enemy `CharacterBody2D` |

### Key Invariants:
1. **Actors Are Never Solid Terrain**:
   - `CharacterBody2D` nodes for Players (Layer 9) and Enemies (Layer 10) mask against Layers 1 & 2 (`Solids` + `Platforms`).
   - They must **never** set `collision_layer = 1` (`Solids`), ensuring actors collide with the environment without physically blocking, pinning, or getting stuck on other actors.
2. **Sensory Areas Mask Entities Directly**:
   - AI sensory detectors (`HearingArea`, `VisionArea`) set `collision_mask = 256` (Layer 9: `Player`) to detect the player body via `BodyEntered`.
3. **Combat Areas Are Decoupled from Kinematic Bodies**:
   - Combat damage, invulnerability, and overlaps are handled purely through dedicated `Area2D` nodes (`Hitbox` / `Hurtbox`) on Layers 5–8.
   - Both `Hitbox` and `Hurtbox` connect to `AreaEntered` for bidirectional collision triggering.

---

## 3. Core Architectural Patterns

### 3.1 Node-Based Finite State Machine (FSM)
Entity behaviors (Player, Enemies, etc.) are managed by a modular, node-based state machine in `scripts/utilities/`:

- **`FiniteStateMachine` (Coordinator):**
  - Manages active state and transitions.
  - Automatically registers all child `State` nodes on `_Ready()`.
  - Routes `_Process` $\rightarrow$ `_state.UpdateGraphics()` and `_PhysicsProcess` $\rightarrow$ `_state.UpdatePhysics()`.
- **`State` (Base Node):**
  - Base class inheriting from `Godot.Node`.
  - Base `_Process` and `_PhysicsProcess` are sealed to prevent accidental overriding.
  - Exposes virtual lifecycle methods: `Enter()`, `Exit()`, `UpdatePhysics(double delta)`, `UpdateGraphics(double delta)`.
  - Triggers transitions via `Transition(State nextState)`.
- **State Wiring via Inspector:**
  - States declare dependencies (bodies, sprites, audio players, target states) using `[Export]` and `[ExportGroup("...")]`.

### 3.2 Combat Traits & Structs (`scripts/combat/`)
Universal combat building blocks applicable to both characters and props:

- **`Damageable` (Node):**
  - Logic node tracking `MaxHitPoints` and `CurrentHitPoints`.
  - Exposes `TakeDamage(Hit)`, `Heal(int)`, and `Reset()`.
  - Emits `OnDamaged(int amount, Vector2 origin)`, `OnHealthChanged(int current, int max)`, and `OnDepleted()`.
- **`Hurtbox` (Area2D):**
  - Exports a reference to `Damageable` and manages `IsInvulnerable`.
  - Emits `OnHurt(int damage, Vector2 origin, float force)` and forwards damage payloads to `_damageable.TakeDamage(hit)`.
- **`Hitbox` (Area2D):**
  - Exports `Damage` and `Force`.
  - On `AreaEntered` with a `Hurtbox`, constructs a `Hit` and calls `hurtbox.ReceiveHit(hit)`.
- **`Hit` (Readonly Struct):**
  - Immutable payload containing `Damage` (int), `Origin` (Vector2), and `Force` (float).

### 3.3 Extension Methods for Engine Types
To keep entity scripts clean and avoid deeply nested inheritance hierarchies:
- Reusable math and physics helpers are written as C# static extension methods under `scripts/extensions/<GodotType>/`.
- **Examples:**
  - `CharacterBody2D.MoveWithInertia(...)`, `CharacterBody2D.Accelerate(...)`, `CharacterBody2D.Decelerate(...)`
  - `AnimatedSprite2D.SynchronizeAnimation(...)` (handles frame scaling and `FlipH` based on direction)

### 3.4 Tile & Environmental Detection
- Dedicated detector nodes (`TileDetector2D` inheriting from `Area2D`) emit signals (`OnTileEntered`, `OnTileExited`) when overlapping environmental layers (ladders, water, triggers).

---

## 4. C# Coding Conventions

### 4.1 Naming Conventions
- **Classes, Enums, Structs, Interfaces:** `PascalCase` (e.g., `FiniteStateMachine`, `PlayerStanding`, `Damageable`).
- **Public Methods & Properties:** `PascalCase` (e.g., `EnterState()`, `IsOverlapping`, `CurrentHitPoints`).
- **Private / Protected Fields:** `_camelCase` with a leading underscore (e.g., `_state`, `_body`, `_damageable`).
- **Local Variables & Parameters:** `camelCase` (e.g., `delta`, `normalizedDirection`).
- **Signals / Events:** `PascalCase` matching Godot naming conventions (e.g., `OnTransition`, `OnDamaged`, `OnDepleted`).

### 4.2 Fail-Fast Inspector Dependencies
- Use `[Export]` for inspector-assignable node references and tunable parameters.
- Group related parameters using `[ExportGroup("GroupName")]`.
- **Validation:** In `_Ready()`, nodes must validate required exported node references and log descriptive errors via `GD.PushError($"[...] has no ... assigned!")` rather than guessing or silently searching parent/sibling nodes.

### 4.3 Audio Organization
- Sound effect players are grouped as `AudioStreamPlayer2D` children under a dedicated `SoundEffects` node in scene trees.

### 4.4 Clean Code & Technical Debt
- Annotate pending improvements or known edge-cases with `/// @todo <description>` comments.
- Format code with CSharpier before committing changes.
