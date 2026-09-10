## 1. Systemic Traits & Components

- [x] 1.1 Implement `Flammable2D.cs` (`Area2D` component with `IsBurning`, `BurnDuration`, `BurnEndBehavior`, `ExtinguishInWater`, `Ignited`/`Extinguished`/`BurnCompleted` signals, contact ignition propagation, and water volume detection) and verify build succeeds
- [x] 1.2 Implement `LightEmitter2D.cs` (encapsulated `PointLight2D` controller with `LightColor`, `BaseEnergy`, `RadiusScale`, procedural sine/noise flame flicker, and automatic binding to `Flammable2D`) and verify build succeeds

## 2. Torch Object Refactoring

- [x] 2.1 Refactor `Torch.tscn` to use `Flammable2D` (with collision shape and fire visual particles/sprite) and `LightEmitter2D`, defaulting to unlit with pre-lit inspector support, and verify in editor/test scene
- [x] 2.2 Verify `Torch.tscn` dynamically toggles fire visuals and light emission when ignited or extinguished

## 3. Fire Arrow Projectile & Bow Arrow Variants

- [x] 3.1 Create `FireArrow.tscn` (and `FireArrow.cs` / extended `Arrow.cs`) equipped with flame particles, dynamic `LightEmitter2D`, and pre-lit `Flammable2D` ignition payload, and verify projectile instantiates cleanly
- [x] 3.2 Update `BowItem.cs` and `PlayerShooting.cs` to support arrow variants (`Standard` vs `Fire`), spawning `FireArrow.tscn` when Fire Arrow variant is active
- [x] 3.3 Add debug cycling support for Bow item variants (`No Bow` -> `Bow (Standard)` -> `Bow (Fire)`), updating inventory/HUD and active projectile

## 4. Light Occlusion & Integration Verification

- [x] 4.1 Update `Player.tscn` `LightOccluder2D` visibility to ensure dynamic shadow casting from point lights works properly
- [x] 4.2 Test end-to-end interactions: firing a Fire Arrow at an unlit Torch ignites it and casts flickering light; firing a Fire Arrow into water extinguishes it; burning entity in water douses flame
