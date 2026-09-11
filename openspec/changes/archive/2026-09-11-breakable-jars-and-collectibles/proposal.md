# Change Proposal: Breakable Jars and Collectibles System

## Why

Destructible environmental props (like ceramic jars) and restorative pickups (ammo, health) are core Zelda and Metroidvania staples that reward combat action and exploration. Currently, players have no in-game means of replenishing expended arrow ammunition or health, and the world lacks interactable breakable objects. Introducing a systemic `Lootable2D` trait, an extensible `Collectible2D` pipeline, and smashable `Jar` entities establishes a scalable foundation for all current and future item drops and container looting (related to GitHub Issues #17 and #41).

## What Changes

- **Breakable Ceramic Jar (`Jar.tscn`)**:
  - Implements the breakable jar prop using `Damageable` (1 HP), `Hurtbox`, and `Lootable2D`.
  - Non-solid/intangible collision so players can walk past it freely.
  - Smashes when struck by player attacks (Sword slashes, Arrows), playing a smash puff effect, triggering its drop roll, and freeing itself.
- **Generic Collectible Framework (`Collectible2D`)**:
  - Abstract `Area2D` base class handling spawn arc pop physics, ground settling, gentle floating bob animation, optional lifetime timeout with blinking, and `collect.wav` audio feedback.
  - Polymorphic `OnCollect(Node2D collector)` execution allowing custom pickup payloads.
- **Concrete Collectibles**:
  - `ArrowCollectible2D`: Supports 3 distinct visual tiers (10, 20, 30 arrows) mapped to frames of `AmmoArrows.png`, directly replenishing the collector's `BowItem` ammo in `Inventory`.
  - `HeartCollectible2D`: Heals the collector's `Damageable` by 4 HP (1 full heart container), utilizing a new 16x16 pixel-art heart texture.
- **Unified Lootable Trait (`Lootable2D`)**:
  - Reusable component holding weighted drop tables / loot definitions.
  - Supports `DropOnDestroy` (spawning physics collectibles in world space when destroyed) and `LootOnInteract` (direct acquisition for future chests or searching).
- **World Placement & Testing**:
  - Places jars with configured drop tables in `World.tscn` alongside testing instances.

## Capabilities

### New Capabilities
- `lootable-objects`: Modular trait for configuring weighted loot tables and triggering item drops on destruction (`DropOnDestroy`) or direct retrieval on interaction (`LootOnInteract`).
- `collectible-items`: World collectible entity lifecycle (`Collectible2D`), physics spawn arc, lifetime blinking, audio cues, and polymorphic collection hooks (`ArrowCollectible2D`, `HeartCollectible2D`).
- `breakable-jars`: Smashable decorative pottery entity (`Jar.tscn`) composed of `Damageable`, `Hurtbox`, and `Lootable2D`.

### Modified Capabilities
<!-- None -->

## Impact

- **New Scripts**: `scripts/combat/Lootable2D.cs`, `scripts/objects/Collectible2D.cs`, `scripts/objects/ArrowCollectible2D.cs`, `scripts/objects/HeartCollectible2D.cs`, `scripts/objects/Jar.cs`.
- **New Scenes**: `scenes/objects/Jar.tscn`, `scenes/objects/ArrowCollectible.tscn`, `scenes/objects/HeartCollectible.tscn`.
- **New Textures**: `assets/textures/objects/items/Heart.png` (16x16 pixel art).
- **Affected Systems**: `Inventory` (interacted with by `ArrowCollectible2D`), `Damageable` (interacted with by `HeartCollectible2D` and `Jar.tscn`), `World.tscn` (adds test jar placements).
