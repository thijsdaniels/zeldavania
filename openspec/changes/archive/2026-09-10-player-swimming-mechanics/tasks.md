## 1. Passive Items & Inventory System

- [x] 1.1 Create `PassiveItem.cs` in `scripts/inventory/` inheriting from `Node` with exported properties: `ItemId`, `ItemName`, `Description`, `TierIcons`, `DefaultIcon`, `Tier`, and `IsUnlocked`, supporting dynamic tiered naming and descriptions.
- [x] 1.2 Update `Inventory.cs` to add `GetPassiveTier(string itemId)` and `HasPassive(string itemId, int minTier = 1)` helper methods.
- [x] 1.3 Generate custom 16x16 pixel art icons for all 4 tiers: Silver Scale, Golden Scale, Zora's Flippers, and Golden Flippers.

## 2. Player Swimming & Diving States
- [x] 2.1 Create `PlayerSwimming.cs` for unified surface floating, horizontal swimming (`_surfaceSwimSpeed = 65f`), dynamic spring-damper buoyancy, timed downward diving (`scale` Tier 1 vs Tier 2), max depth hold, and buoyant resurfacing.
- [x] 2.2 Implement active breach jumping in `PlayerSwimming.cs` (`Controller.A` without `Down`, `_leapVelocity = 190f`) with jump ascent notification in `PlayerFalling.cs`.
- [x] 2.3 Create `PlayerDiving.cs` for 360° free diving (`scale` Tier 3), swim dash, and waterfall climbing (`scale` Tier 4).

## 3. Scene & Inspector Wiring

- [x] 3.1 Instantiate `Scale` unified 4-tier passive item node under `Player/Inventory` in `scenes/entities/Player.tscn` with 4 tier icon textures.
- [x] 3.2 Wire `Swimming` (surface) and `Diving` (underwater) state nodes and references in `Player.tscn`.
- [x] 3.3 Hand-stamp `0:1` interior water tiles under solid ceilings/walls in `World.tscn` where applicable.
- [x] 3.4 Configure dedicated `Waterfalls` 2D physics layer 11 in `project.godot` and `TileSet.tres`, instantiate `WaterfallDetector2D` on Player, and wire to `PlayerDiving` and `PlayerSwimming`.
- [x] 3.5 Wire `WaterDetector2D` and `_swimmingState` into `PlayerWallSliding.cs` and `Player.tscn` to handle wallsliding into water.

## 4. Verification & Testing

- [x] 4.1 Build the project with `dotnet build` to ensure zero compilation errors.
- [x] 4.2 Test surface paddling, breaching jumps onto 1-2 tile ledges, diving & resurfacing, and free 360° swim / waterfall ascent.
