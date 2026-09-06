## 1. Visual & Audio Assets

- [x] 1.1 Create the 4-frame smoke poof texture sheet `assets/textures/effects/EnemyDeathPuff.png` and verify file creation.
- [x] 1.2 Create `EnemyDeathEffect.cs` and `EnemyDeathEffect.tscn` configured with `AnimatedSprite2D`, `AudioStreamPlayer2D` (`explode.wav`), and auto-cleanup.

## 2. Enemy Dying State Updates

- [x] 2.1 Update `EnemyDying.cs` with dependency exports for `_hurtbox`, `_contactHitbox`, and `_deathEffectScene`.
- [x] 2.2 In `EnemyDying.Enter()`, disable hurtbox/contact hitbox monitoring and check if `_animation` exists on `SpriteFrames`. If missing, spawn death effect and free immediately; if present, play animation and spawn effect on `AnimationFinished`.
- [x] 2.3 Wire the dependencies and `_deathEffectScene` in `Bat.tscn` and `Goblin.tscn`.

## 3. Verification & Polish

- [x] 3.1 Build project using `dotnet build` and ensure 0 warnings and 0 errors.
- [x] 3.2 Launch game and test bat defeat (instant smoke poof + explode sound) and goblin defeat (plays collapse animation, then poof + explode sound).
