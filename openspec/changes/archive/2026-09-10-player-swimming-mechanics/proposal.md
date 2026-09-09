# Change Proposal: Player Swimming Mechanics & Ability Progression

## Summary
Overhaul the player swimming mechanics to feature surface paddling with buoyancy, an active breaching jump to easily clear ledges, timed diving with buoyant automatic resurfacing, current pushback and waterfall scaling via swim dashing, and a tiered passive item upgrade system in the inventory.

## Problem Statement
The current swimming implementation has several limitations:
1. **Unresponsive & Float-like Movement**: Swimming acts like omnidirectional 2D flight with no surface floating or distinct paddling feel.
2. **Ledge Exit Failure**: The player cannot jump out of water onto ledges. There is no active jump action in water, and the exit velocity formula contains a typo referencing horizontal velocity (`Mathf.Min(_body.Velocity.X, -_leapVelocity)`) with an underpowered impulse (`110 px/s` vs standard jump `180 px/s`).
3. **Unrestricted Waterfall Climbing**: Waterfall tiles share the water physics layer without downward current, allowing the player to freely fly up waterfalls without any ability gating.
4. **Lack of Progression & Ability Unlocks**: No modular system exists for tiered diving depth, free-form underwater navigation, or current/waterfall dash abilities.

## Proposed Solution
1. **Surface Floating & Horizontal Swimming**:
   - By default on water entry, buoyancy keeps the player afloat at the surface waterline with smooth horizontal swimming controls (`Left` / `Right`) in `PlayerSwimming`.
2. **Active Dolphin Breach & Leaping**:
   - Surface Breach Jump: Pressing Jump (`Controller.A`) without `Down` launches an upward breach leap (`Velocity.Y = -190 px/s`) to clear 1–2 tile ledges onto dry land.
   - Underwater Dolphin Leap: When surfacing from `PlayerDiving` with upward momentum, launches the player in a full hydrodynamic arc into the air without jumping velocity cuts.
3. **Unified 4-Tier Scale Progression (`scale`)**:
   - **Tier 1 (Silver Scale)**: Shallow timed dive ($\approx 1.75$ tiles / $28\text{ px}$) with hover hold and buoyant auto-resurface.
   - **Tier 2 (Golden Scale)**: Deep timed dive ($\approx 4$ tiles / $64\text{ px}$) with hover hold and buoyant auto-resurface.
   - **Tier 3 (Zora's Flippers)**: Pressing `Down` while swimming dives into `PlayerDiving` for free 360° omnidirectional swimming with smooth heading rotation.
   - **Tier 4 (Golden Flippers)**: Unlocks swim dashing (Hold `A`) and ascending vertical waterfall columns.
4. **Tiered Passive Item Architecture**:
   - Extend `PassiveItem` in `Zeldavania.Inventory` with dynamic `TierIcons`, `DisplayName`, and `DisplayDescription` properties (`GetPassiveTier("scale")`), configuring the progressive item under `Player/Inventory`.
5. **Waterfall Separation & Physics Layering**:
   - Dedicated `Waterfalls` layer 11 in `TileSet.tres` and `WaterfallDetector2D` on Player for pushback and plunge dispersal.
6. **Water Tile Autotiling / Hand-Stamping**:
   - Hand-stamp interior `0:1` water tiles under ceilings and solid overhangs to maintain clean tile boundaries without surface foam.

## Non-Goals
- Dedicated underwater HUD breath/oxygen gauge bar (tracked separately in future UI pass).
- Procedural water shader waves (existing TileMap tile animations and modulation are preserved).

## References
- Specs: `openspec/specs/player-movement/spec.md`
- Architecture Rules: `.agents/rules/terrain-tiles.md`, `.agents/rules/godot-node-traits.md`
