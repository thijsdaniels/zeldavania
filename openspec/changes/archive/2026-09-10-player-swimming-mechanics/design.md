# Design Document: Player Swimming Mechanics & Ability Progression

### Overview
This design structures player aquatic movement into two focused `State` nodes in the `FiniteStateMachine`:
1. **`PlayerSwimming`**: Surface waterline floating, horizontal swimming, buoyant spring-damper dynamics, shallow/deep diving (Scale Tier 1 & 2), and breach jumping.
2. **`PlayerDiving`**: Free 360° underwater navigation, swim dashing, and waterfall climbing with Flippers (Scale Tier 3 & 4).

---

## State Machine Architecture

```
                 +-------------------+
                 |   PlayerFalling   |
                 +-------------------+
                   | (enters water)
                   v
          +------------------+ (dive with flippers) +--------------------+
          |  PlayerSwimming  | -------------------> |    PlayerDiving    |
          | (surface float)  | <------------------- | (360° / Free Swim) |
          +------------------+  (swim to surface)   +--------------------+
           | (dive with scale) ^
           v                   |
     +------------------------------+
     | Physical Buoyant Dive & Hold |
     +------------------------------+
```

---

## Class Definitions & Responsibilities

### 1. `PlayerSwimming.cs` (Surface Mode)
* **Description**: Default state on water entry. Floats at waterline with natural buoyancy damping.
* **Movement**: Horizontal left/right swimming at `_surfaceSwimSpeed` (65 px/s).
* **Breach Leap**: `Controller.A` without `Down` launches upward (`Velocity.Y = -190 px/s`), plays splash, and transitions to `PlayerFalling`.
* **Scale Dive**: Pressing `Down` with `Scale Tier 1 or 2` initiates timed buoyant dive down with max-depth hover and auto-resurface.
* **Flipper Dive**: Pressing `Down` with `Scale Tier >= 3` transitions into `PlayerDiving` for free 360° underwater exploration.

### 2. `PlayerDiving.cs` (360° Free Diving / Flippers)
* **Description**: Active when `GetPassiveTier("scale") >= 3`. Unrestricted 360° omnidirectional movement at `_freeSwimSpeed` (110 px/s) with Rayman-style direct stick steering.
* **Heading Rotation**: Sprite smoothly interpolates visual angle to match movement heading with a +90° sprite frame offset.
* **Golden Flippers (Tier 4)**: Holding `A` activates swim dash (`_flipperDashSpeed = 210 px/s`). Holding `Up` / `A` inside waterfalls overcomes current force (`_waterfallCurrentForce = 600 px/s²`) to ascend up waterfalls.
* **Dolphin Leap**: Surfacing at the waterline with upward momentum launches the player in a hydrodynamic arc (`_dolphinVerticalVelocity = 195 px/s`, `_dolphinHorizontalVelocity = 125 px/s`, `1.35x` dash multiplier) without jump-cut velocity damping.
* **Surface Transition**: Gently floating or swimming horizontally to the surface returns to `PlayerSwimming`.

---

## Node & Scene Composition

### `Player.tscn` Hierarchy Updates
```
Player (CharacterBody2D)
├── WaterDetector2D (Area2D)       [Collision Mask: Layer 3 - Liquids (4)]
├── WaterfallDetector2D (Area2D)   [Collision Mask: Layer 11 - Waterfalls (1024)]
├── LadderDetector2D (Area2D)      [Collision Mask: Layer 4 - Climbables (8)]
├── State (FiniteStateMachine)
│    ├── Swimming (PlayerSwimming)      [Surface floating, diving, buoyancy, breach jump]
│    └── Diving (PlayerDiving)          [360° diving, flipper dash, waterfall ascent, dolphin leap]
```

```
Player/Inventory/
    └── Scale (PassiveItem)        [ItemId = "scale", Tier = 1, MaxTier = 4, IsUnlocked = true]
```

---

## Physics Layers & Collision Mapping

| Layer Index | Name | Bit Mask | Purpose |
| :--- | :--- | :--- | :--- |
| Layer 3 | `Liquids` | `4` | Water tiles (pools, basins, channels) detected by `WaterDetector2D` |
| Layer 11 | `Waterfalls` | `1024` | Waterfall tiles detected by `WaterfallDetector2D` for currents/scaling |

---

## Tuning Values & Configuration

| Parameter | Default Value | Purpose |
| :--- | :--- | :--- |
| `_surfaceSwimSpeed` | `65 px/s` | Horizontal surface swimming velocity |
| `_leapVelocity` | `190 px/s` | Surface breach jump upward impulse (clears 1-2 tile ledges) |
| `_buoyancySpringStiffness` | `80` | Spring constant for surface bobbing & buoyant return |
| `_buoyancyDamping` | `6.5` | Viscous water drag damping oscillations |
| `_waterlineOffset` | `8 px` | Submergence resting depth offset |
| `_maxBuoyancyAscentSpeed` | `90 px/s` | Maximum passive buoyant rise velocity |
| `_shallowDiveDownwardForce` | `2800 px/s²` | Scale Tier 1 downward swim thrust fighting buoyancy |
| `_deepDiveDownwardForce` | `5600 px/s²` | Scale Tier 2 downward swim thrust fighting buoyancy |
| `_shallowDiveDuration` | `0.65 s` | Scale Tier 1 shallow dive duration (~1.75 tiles / ~28 px) |
| `_deepDiveDuration`| `1.3 s` | Scale Tier 2 deep dive duration (~4 tiles / ~64 px) |
| `_maxDepthHoldDuration`| `0.5 s` | Hover duration at max depth while holding Down |
| `_diveCooldown` | `0.5 s` | Breathing cooldown at surface between consecutive dives |
| `_freeSwimSpeed` | `110 px/s` | Scale Tier 3 (Zora's Flippers) 360° swim speed |
| `_flipperDashSpeed`| `210 px/s` | Scale Tier 4 (Golden Flippers) swim dash speed |
| `_dolphinVerticalVelocity` | `195 px/s` | Dolphin leap upward launch impulse at waterline |
| `_dolphinHorizontalVelocity`| `125 px/s` | Dolphin leap horizontal momentum scale |
| `_dolphinDashMultiplier` | `1.35` | Velocity multiplier for dashing dolphin leap |
| `_waterfallCurrentForce` | `600 px/s²` | Downward current pull inside waterfalls |
| `_waterfallDisperseSpeed` | `20 px/s` | Outward lateral current at waterfall plunge zone (48 px / 3 tiles radius) |
