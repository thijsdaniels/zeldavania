## Context

The game relies on Godot 4 Node and Area2D composition rather than C# interfaces ([.agents/rules/godot-node-traits.md](file:///Users/home/Repositories/thijsdaniels/zeldavania/.agents/rules/godot-node-traits.md)). Existing systems include combat traits (`Damageable`, `Hitbox`, `Hurtbox`), projectile physics (`Arrow`), environmental detectors (`WaterDetector2D`), and inventory item slot assignment (`Inventory`).

## Goals / Non-Goals

**Goals:**
- Implement `Flammable2D` as a self-contained, inspector-first `Area2D` component that handles burning state, timed burnouts, water extinguishment, and contact ignition.
- Implement `LightEmitter2D` as an encapsulated `PointLight2D` controller supporting dynamic procedural flicker and automatic binding to parent/sibling `Flammable2D` state.
- Refactor `Torch.tscn` to use `Flammable2D` + `LightEmitter2D` (unlit by default with pre-lit inspector toggle).
- Create `FireArrow.tscn` with flame visuals, dynamic light, and pre-lit `Flammable2D` payload.
- Update `BowItem` and `PlayerShooting` to support cycling between standard arrows and fire arrows in debug/inventory.
- Fix player shadow occlusion by enabling `visible = true` on `LightOccluder2D` in `Player.tscn`.

**Non-Goals:**
- Multi-torch puzzle logic (`TorchPuzzleManager`), timed door triggers, or puzzle coordination ([Issue #56](https://github.com/thijsdaniels/zeldavania/issues/56)).
- Dark room full-screen ambient modulation (`CanvasModulate` / [Issue #39](https://github.com/thijsdaniels/zeldavania/issues/39)).
- Elemental projectile variants beyond Fire (Ice, Light are reserved for future changes).

## Decisions

### 1. `Flammable2D` Component Structure & Contact Ignition
- **Architecture**: `Flammable2D` is an `Area2D` component placed in `scripts/combat/` or `scripts/components/`.
- **Properties**:
  - `[Export] public bool IsBurning { get; set; } = false;`
  - `[Export] public float BurnDuration { get; set; } = 0f;` (0 = infinite)
  - `[Export] public BurnEndBehavior OnBurnComplete { get; set; } = BurnEndBehavior.Extinguish;` (Enum: `Extinguish`, `DestroyEntity`, `TriggerSignalOnly`)
  - `[Export] public bool ExtinguishInWater { get; set; } = true;`
  - `[Export] private Node2D _flameVisuals;` (Optional direct reference to particle emitter / animated sprite)
- **Contact Propagation**:
  - When `IsBurning == true`, `Flammable2D` checks overlapping `Flammable2D` areas or processes `AreaEntered` to call `other.Ignite()`.
- **Water Extinguishment**:
  - When overlapping an area in the Water layer / `WaterVolume`, if `ExtinguishInWater == true`, `Extinguish()` is called.
- *Alternative Considered*: Dedicated separate `Igniter2D` component. *Rationale*: Making `Flammable2D` itself an ignition source when burning simplifies scene trees and keeps ignition systemic.

### 2. `LightEmitter2D` Component & Flame Flicker
- **Architecture**: `LightEmitter2D` extends `PointLight2D` (or wraps one as a child `Node2D`) in `scripts/utilities/` or `scripts/objects/`.
- **Properties**:
  - `[Export] public Color LightColor { get; set; } = new Color(0.82f, 0.51f, 0.14f);`
  - `[Export] public float BaseEnergy { get; set; } = 0.6f;`
  - `[Export] public float RadiusScale { get; set; } = 0.6f;`
  - `[Export] public bool Flicker { get; set; } = true;`
  - `[Export] public float FlickerIntensity { get; set; } = 0.08f;`
  - `[Export] public float FlickerSpeed { get; set; } = 12f;`
- **Flammable Binding**:
  - In `_Ready()`, finds sibling or parent `Flammable2D`. Automatically subscribes to `Ignited` (enables light) and `Extinguished` (disables light).
- **Procedural Flicker**:
  - Modulates `Energy` and `TextureScale` via smooth sine/noise variation during `_Process()` when active.

### 3. Torch Scene Refactor
- **Node Hierarchy (`Torch.tscn`)**:
  ```
  Torch (Node2D)
    +-- Sprite2D (Torch base sprite)
    +-- Flammable2D (Area2D with CollisionShape2D)
    |     +-- GPUParticles2D / AnimatedSprite2D (Flame at offset (0, -25))
    +-- LightEmitter2D (PointLight2D with flicker, radius 0.6)
  ```
- Defaults to `IsBurning = false`. In inspector, `[Export] bool IsLit` or `Flammable2D.IsBurning` can be checked for pre-lit placement.

### 4. Fire Arrow Projectile & Bow Cycling
- **`FireArrow.tscn`**:
  - Inherits from `Arrow` (`Arrow.cs` or subclass `FireArrow.cs`).
  - Child nodes:
    - `LightEmitter2D` (smaller radius, e.g. `RadiusScale = 0.3`, `BaseEnergy = 0.4`).
    - `GPUParticles2D` (trailing flame particles).
    - `Flammable2D` (configured `IsBurning = true`, ignition payload).
- **Bow Cycling**:
  - `BowItem.cs` maintains `[Export] public ArrowVariant Variant { get; set; } = ArrowVariant.Standard;` (Enum: `Standard`, `Fire`).
  - `PlayerShooting.cs` has `[Export] private PackedScene _fireArrowScene;` and selects scene based on `_bowItem.Variant`.
  - Debug cycle key or inventory cycle advances bow state: `No Bow` -> `Standard Bow` -> `Fire Bow`.

## Risks / Trade-offs

- **[Performance of point light flicker]** $\rightarrow$ Simple math sine/noise evaluation on single floats per frame in `_Process()` has negligible CPU cost compared to custom shaders.
- **[Light Occlusion in 2D]** $\rightarrow$ Godot 4 requires `LightOccluder2D.visible = true` and `shadow_enabled = true` on `PointLight2D`. Verified and addressed in `Player.tscn`.
- **[Area2D Collision Layer Conflicts]** $\rightarrow$ Ensure `Flammable2D` uses an appropriate collision layer/mask (e.g. dedicated interactable/hazard layer) so it cleanly detects other `Flammable2D` areas and `WaterVolume` without breaking enemy hurtboxes.
