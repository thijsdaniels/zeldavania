## 1. Asset & Core Collectible Framework

- [x] 1.1 Create 16x16 pixel-art heart texture (`assets/textures/objects/items/Heart.png`) matching project color palette and verify image file exists
- [x] 1.2 Implement abstract `Collectible2D` base class (`scripts/objects/Collectible2D.cs`) with spawn pop velocity, settling physics, float bobbing, optional lifetime timeout/blinking, and `collect.wav` audio playback
- [x] 1.3 Implement `ArrowCollectible2D` (`scripts/objects/ArrowCollectible2D.cs`) with 10/20/30 ammo quantity tiers mapped to `AmmoArrows.png` frames and `Inventory` replenishment logic
- [x] 1.4 Implement `HeartCollectible2D` (`scripts/objects/HeartCollectible2D.cs`) with `Damageable.Heal(4)` restoration logic

## 2. Collectible Scenes & Lootable Trait

- [x] 2.1 Create `ArrowCollectible.tscn` and `HeartCollectible.tscn` scenes with `CircleShape2D` collision and sprite configuration
- [x] 2.2 Implement `LootEntry` resource/struct and `Lootable2D` component (`scripts/combat/Lootable2D.cs`) with weighted drop rolling, `DropOnDestroy`, and `LootOnInteract` modes

## 3. Breakable Jar Entity

- [x] 3.1 Implement `Jar.cs` controller (`scripts/objects/Jar.cs`) handling `Damageable.OnDepleted`, destruction puff effect spawning (`EnemyDeathPuff`), and cleanup
- [x] 3.2 Create `Jar.tscn` scene composed of `Sprite2D` (`Jar.png`), `Damageable`, `Hurtbox`, and `Lootable2D` with configured arrow and heart drop table

## 4. World Integration & Verification

- [x] 4.1 Place breakable jars with drop tables across accessible ground surfaces in `World.tscn`
- [x] 4.2 Run C# build (`dotnet build`) to verify clean compilation with zero warnings/errors
- [x] 4.3 Verify in gameplay that sword slashes and arrows smash jars, spawn collectibles with arc pops, and replenish ammo and health upon collection
