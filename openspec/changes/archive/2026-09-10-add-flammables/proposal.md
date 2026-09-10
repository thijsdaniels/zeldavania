## Why

Interactive environmental gameplay in classic Zelda/Metroidvania games relies on systemic object traits—such as fire, lighting, and destructibles. Currently, torches in the game are static visual scenes with non-interactive point lights, and the player bow only fires standard physical arrows.

This change introduces the systemic `Flammable2D` trait and reusable `LightEmitter2D` component (addressing [Issue #38](https://github.com/thijsdaniels/zeldavania/issues/38)) alongside the Fire Arrow projectile variant and bow debug cycling (addressing [Issue #53](https://github.com/thijsdaniels/zeldavania/issues/53)).

## What Changes

- **Systemic `Flammable2D` Node Component**:
  - Encapsulates burning state (`IsBurning`), ignition, and extinguishing.
  - Supports configurable burn durations (indefinite for standard torches; timed for fuses or consumable entities).
  - Emits `Ignited`, `Extinguished`, and `BurnCompleted` signals.
  - When actively burning, acts as an ignition source that ignites overlapping `Flammable2D` entities on contact.
  - Automatically extinguishes when submerged in water volumes if `ExtinguishInWater` is enabled.
  - Coordinates child or assigned visual emitters (particles, animated sprites) across burning state changes.
- **Reusable `LightEmitter2D` Component**:
  - Encapsulates `PointLight2D` with configurable color, energy, radius scale, shadows, and subtle procedural flame flickering.
  - Connects automatically to sibling/parent `Flammable2D` to sync light emission with burning state.
  - Resolves player shadow casting by ensuring `LightOccluder2D` is active and occluding properly.
- **Interactive `Torch` Entity Refactor**:
  - Updates `Torch.tscn` to use `Flammable2D` and `LightEmitter2D`.
  - Configurable in editor to start unlit by default, or pre-lit for placed scene instances.
- **Fire Arrow Projectile & Bow Variant Cycling**:
  - Creates `FireArrow.tscn` projectile scene with flame trail visuals, dynamic `LightEmitter2D`, and pre-lit `Flammable2D` ignition payload.
  - Extends bow item / shooting pipeline to support arrow variants, enabling cycling between `No Bow`, `Bow (Standard Arrows)`, and `Bow (Fire Arrows)` in inventory/debug mode.
- **Non-Goals / Deferred Scope**:
  - Multi-torch puzzle coordinators (`TorchPuzzleManager` / [Issue #56](https://github.com/thijsdaniels/zeldavania/issues/56)) are deferred to a follow-up change.
  - Dark room ambient modulation (`CanvasModulate` / [Issue #39](https://github.com/thijsdaniels/zeldavania/issues/39)) is deferred to a follow-up change.

## Capabilities

### New Capabilities
- `flammable-objects`: Systemic `Flammable2D` trait, `LightEmitter2D` dynamic lighting, ignition contact propagation, water extinguish triggers, and interactive torch entities.

### Modified Capabilities
- `player-bow-attack`: Adds Fire Arrow variant capability, dynamic light emission on fired fire arrows, ignition payload on projectile impact, and bow item arrow variant selection.

## Impact

- **Entities & Scenes**: `Torch.tscn` refactored; new `FireArrow.tscn` created; `Player.tscn` occluder visibility verified.
- **Combat & Traits**: New `Flammable2D` component in `scripts/combat/` or `scripts/components/`; new `LightEmitter2D` in `scripts/utilities/` or `scripts/objects/`.
- **Inventory & Bow State**: `BowItem.cs`, `PlayerShooting.cs`, and `Inventory.cs` updated to support arrow variants and debug cycling.
