# Technical Design: Breakable Jars and Collectibles System

## Context

See `proposal.md` for motivation and background. The project utilizes Godot 4.7+ (.NET / C#) with a node-and-scene trait composition architecture (`.agents/rules/godot-node-traits.md`). Currently, `Player` has a local `Inventory` node managing `EquipmentItem` nodes (such as `BowItem`) and a `Damageable` node managing hit points. Enemies and weapons use `Hitbox` and `Hurtbox` on distinct collision layers.

## Goals / Non-Goals

**Goals:**
- Provide an extensible, abstract `Collectible2D` base class for all in-world pickups with pop physics, gentle bobbing, optional lifetime blinking, and audio feedback.
- Implement `ArrowCollectible2D` (supporting 10, 20, and 30 arrow bundle tiers) and `HeartCollectible2D` (heals 4 HP).
- Implement a reusable `Lootable2D` component handling weighted drop tables with both `DropOnDestroy` and `LootOnInteract` trigger pathways.
- Implement a smashable `Jar.tscn` scene using `Damageable`, `Hurtbox`, and `Lootable2D`.
- Create a 16x16 pixel-art heart icon (`Heart.png`) matching the game's palette.

**Non-Goals:**
- Full treasure chest fanfare cutscenes / hold-above-head states (tracked under Issue #42).
- Magnetism/suction towards player (deferred for future upgrade/accessory items).
- Dynamic situational loot weighting (e.g. adaptive difficulty drop rate adjustments).

## Decisions

### 1. Polymorphic `Collectible2D` Base Class
- **Decision**: `Collectible2D` is an abstract `Area2D` node with `protected abstract bool OnCollect(Node2D collector)`. It handles common physics (initial velocity impulse `Vector2(rand_x, -120)`, gravity, surface settling), bobbing, optional blinking/timeout, and audio playback (`collect.wav`).
- **Rationale**: Keeps common pickup lifecycle logic in one place while allowing specific item types (`ArrowCollectible2D`, `HeartCollectible2D`, and future `BombCollectible2D` or `KeyCollectible2D`) to write clean, type-safe resolution logic.
- **Alternatives Considered**:
  - *Unified enum switch statement in Collectible.cs*: Violates Open-Closed principle and requires modifying core files for each new pickup.
  - *Pure C# non-Node interfaces*: Conflicts with Godot node-trait guidelines.

### 2. Node Resolution on Collector
- **Decision**: When `OnCollect(Node2D collector)` runs, concrete collectibles query the collector directly:
  - `ArrowCollectible2D`: queries `collector.GetNodeOrNull<Inventory>("Inventory")` -> finds `BowItem` -> calls `inventory.AddAmmo(bow, Amount)`.
  - `HeartCollectible2D`: queries `collector.GetNodeOrNull<Damageable>("Damageable")` -> calls `damageable.Heal(HealAmount)`.
- **Rationale**: Clean, decoupled, and multiplayer-friendly since it operates on the specific `collector` instance that collided with the pickup without relying on global singletons.

### 3. Unified `Lootable2D` Component
- **Decision**: `Lootable2D` manages an exported array of `LootEntry` (or `DropTableEntry` resource/struct with `PackedScene Scene` and `int Weight`). It supports two trigger modes:
  - `DropOnDestroy`: connects to `Damageable.OnDepleted` on its parent or assigned target. On trigger, rolls a drop, instantiates the `Collectible2D` in world space, and applies pop velocity.
  - `LootOnInteract`: connects to `Interactable2D.Interacted`. On trigger, rolls a drop and calls `Collect(player)` directly.
- **Rationale**: Reuses identical loot resolution logic for breakable objects (jars, pots, crates, grass, enemies) and direct containers (chests, drawers, search points).

### 4. Non-Solid Breakable Jar Entity
- **Decision**: `Jar.tscn` is a `Node2D` containing a `Sprite2D` (`Jar.png`), a `Damageable` (1 HP), an `Area2D` `Hurtbox` (monitoring Layer 4 / combat hurtbox), and `Lootable2D`. It has no solid `StaticBody2D` collision.
- **Rationale**: Player can walk in front of/past jars smoothly without getting stuck or snagged, matching initial gameplay goals.

### 5. Node Hierarchy & Structure

```
Jar (Node2D) [Jar.cs]
├── Sprite2D (Jar.png)
├── Damageable (MaxHitPoints = 1, CurrentHitPoints = 1)
├── Hurtbox (Area2D) -> CollisionShape2D
└── Lootable2D (DropOnDestroy = true)

ArrowCollectible (Area2D) [ArrowCollectible2D.cs]
├── Sprite2D (AmmoArrows.png, hframes = 3, frame = 0/1/2)
├── CollisionShape2D (CircleShape2D radius = 7)
└── AudioStreamPlayer2D (collect.wav)
(Variants: ArrowCollectible.tscn [10x], ArrowCollectible20.tscn [20x], ArrowCollectible30.tscn [30x])

HeartCollectible (Area2D) [HeartCollectible2D.cs]
├── AnimatedSprite2D (Heart.png, 4-frame glint animation)
├── CollisionShape2D (CircleShape2D radius = 7)
└── AudioStreamPlayer2D (collect.wav)
```

## Risks / Trade-offs

- **[Risk] Collectible spawning inside solid tiles** → *Mitigation*: Pop impulse launches upward and forward with simple RayCast2D/body collision or Area2D floor check so it settles above ground.
- **[Risk] Multiple hits destroying jar twice** → *Mitigation*: `Damageable.OnDepleted` fires once and immediately disables `Hurtbox.Monitoring`/`Monitorable` before queueing free.
