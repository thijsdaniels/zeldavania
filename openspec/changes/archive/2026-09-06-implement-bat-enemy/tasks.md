## 1. Asset Export & Texture Setup

- [x] 1.1 Export `Bat.png` (64×16 spritesheet, 4 frames: Sleeping, Flying 1-2, Hurt) from `..\metroidvania-unity\Assets\Sprites\Enemies\Bat.psd` into `assets/textures/entities/Bat.png` and verify file creation.

## 2. Flying Enemy State Scripts

- [x] 2.1 Implement `BatRoosting.cs` in `scripts/entities/enemy/states/` (captures initial roost anchor, plays sleeping animation, and transitions to swoop/wake on alert/target spotted) and verify compilation with `dotnet build`.
- [x] 2.2 Implement `BatSwooping.cs` in `scripts/entities/enemy/states/` (executes downward dive arc toward player position and transitions to fluttering upon completion) and verify compilation with `dotnet build`.
- [x] 2.3 Implement `BatFluttering.cs` in `scripts/entities/enemy/states/` (applies sinusoidal wave flight `sin(time * freq) * amp`, tracks player, and handles swoop cooldown / target lost transitions) and verify compilation with `dotnet build`.
- [x] 2.4 Implement `BatReturning.cs` in `scripts/entities/enemy/states/` (navigates back to the recorded ceiling roost anchor and transitions to `BatRoosting`) and verify compilation with `dotnet build`.

## 3. Bat Scene Assembly & Inspector Wiring

- [x] 3.1 Create `scenes/entities/Bat.tscn` configuring `CharacterBody2D` with `Enemy.cs`, `AnimatedSprite2D` (`SpriteFrames` for `Sleeping`, `Flying`, `Hurt`), `CollisionShape2D`, `ContactHitbox`, `Hurtbox`, `Damageable`, `HearingArea`, `VisionArea`, `SoundEffects`, and `FiniteStateMachine` connecting all states and transitions.

## 4. Verification & Code Formatting

- [x] 4.1 Format C# codebase with `dotnet csharpier .` and verify clean build with `dotnet build`.
- [x] 4.2 Validate full change schema compliance using `openspec validate implement-bat-enemy`.
