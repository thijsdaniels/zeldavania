## 1. Input & Asset Setup

- [x] 1.1 Configure `Controller.B` input action in `project.godot` (mapping Keyboard `K`, `C` and Gamepad `B`) and add helper properties/methods in `Controller.cs`. Verify input bindings register without errors.
- [x] 1.2 Prepare and configure texture assets for `Arrow` and `Crosshair` in `assets/textures/objects/`. Verify sprites import cleanly in Godot.

## 2. Arrow Projectile Entity

- [x] 2.1 Implement `Arrow.cs` script with TowerFall-style two-phase ballistics (initial zero-gravity flat phase within `StraightDistance`, followed by parabolic gravity drop and rotation alignment to velocity).
- [x] 2.2 Add collision and impact handling to `Arrow.cs` (detecting `Solids` on Layer 1 and `EnemyHurtbox` on Layer 8, applying damage via `Damageable`, embedding into surface, and queuing despawn).
- [x] 2.3 Create `Arrow.tscn` scene with sprite, collision detector, and `Hitbox` component configured for `PlayerHitbox` (Layer 7). Verify node composition and layer masks.

## 3. Player Aiming & Shooting States

- [x] 3.1 Implement `PlayerAiming.cs` state handling hold-to-aim threshold, bullet-time slow motion (`Engine.TimeScale = 0.3`), slow walk-aiming, elevation angle sweep with `Up`/`Down` (-45° to +90°), aim vector memory, and crosshair positioning.
- [x] 3.2 Implement `PlayerShooting.cs` state playing the 5-frame `Shoot` animation from `Zack.png`, instantiating and launching `Arrow.tscn` along the active aim vector, and transitioning cleanly back to standing/running/falling.
- [x] 3.3 Add quick-tap shooting transitions to `PlayerStanding`, `PlayerRunning`, and `PlayerFalling`, and wire all new state nodes, crosshair sprite, and exported references in `Player.tscn`.

## 4. Verification & Polish

- [x] 4.1 Build solution via `dotnet build` to verify C# compilation with zero errors or warnings.
- [x] 4.2 Verify aiming controls (quick-tap vs hold, elevation adjustment, walk-aiming, bullet-time) and arrow projectile behavior (flat trajectory, gravity drop, hitting enemies, and embedding into walls) in gameplay.
