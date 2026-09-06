---
trigger: model_decision
description: When creating, configuring, or painting terrain tiles and tilesets.
---

# Terrain & TileSet Architecture

The game structures all environmental tiles into four dedicated **Terrain Sets** to enforce clean layer separation, optimize autotiling modes, and avoid composite asset explosions.

## 1. The Four Terrain Sets

| Terrain Set | Autotile Match Mode | Categories & Terrains | Target TileMapLayer | Purpose |
| :--- | :--- | :--- | :--- | :--- |
| **Set 0: Solids** | `Match Corners and Sides` | `Dirt`, `Rock`, `Ice`, `Dirt - Ledge` | `Solids` / `Platforms` | Primary 2D collidable ground geometry. |
| **Set 1: Surfaces** | `Match Sides` (1D Horizontal) | `Grassy`, `Icy` *(future: `Moss`, `Snow`, `Slime`)* | `Decorations` / `Surfaces` | Visual and physical overlays on top of solid ground. |
| **Set 2: Liquids** | `Match Corners and Sides` / `Match Sides` | `Water`, `Waterfall` *(future: `Lava`, `Acid`)* | `Water` / `Liquids` | Fluid bodies and surface-to-waterfall transitions. |
| **Set 3: Climbables** | `Match Sides` (1D Vertical) | `Ladder` *(future: `Ropes`, `Chains`)* | `Platforms` / `Climbables` | Vertical traversal navigation. |

---

## 2. Asset & Layering Rules

- **Never bake surface toppers into solid terrain spritesheets** (e.g. do not create `DirtGrassy.png` or `RockIcy.png`).
- Solid terrains must remain pure base materials.
- Surface toppers (grass, moss, ice) must be created as standalone 3-tile $48 \times 16$ spritesheets and painted on the overlay layer.
- Solid collision shapes and light occluders belong on the `Solids` layer; surface physics/interactions (like ice slipperiness) are handled via separate detector layers or custom tile data.

---

## 3. Autotile Peering Layouts

### 1D Horizontal Surfaces (`Match Sides`)
Standard 3-tile topper spritesheets ($48 \times 16$):
* `0:0`: **Left Endcap** — Peering bit on `Right Side` only.
* `1:0`: **Looping Middle** — Peering bits on `Left Side` and `Right Side`.
* `2:0`: **Right Endcap** — Peering bit on `Left Side` only.

### 1D Vertical Climbables (`Match Sides`)
Standard 3-tile vertical climbing spritesheets ($16 \times 48$):
* `0:0`: **Top Endcap / Exit** — Peering bit on `Bottom Side` only.
* `0:1`: **Repeating Shaft / Rungs** — Peering bits on `Top Side` and `Bottom Side`.
* `0:2`: **Bottom Endcap** — Peering bit on `Top Side` only.

### 2D Solid Ground (34-Tile `Match Corners and Sides`)
Every 2D solid terrain spritesheet contains 34 sprites for specific neighbor combinations:

| Coordinate | Adjacent Tiles |
| :--- | :--- |
| 0,0 | |
| 0,1 | R |
| 0,2 | L R |
| 0,3 | L |
| 0,4 | L TL T TR R BR B BL |
| 0,5 | L R BR B BL |
| 0,6 | L TL T TR R |
| 1,0 | R BR B |
| 1,1 | L B BL |
| 1,2 | B |
| 1,3 | L TL T TR R B BL |
| 1,4 | L TL T TR R B |
| 1,5 | L TL T TR R BR B |
| 1,6 | T R B |
| 2,0 | |
| 2,1 | |
| 2,2 | T B |
| 2,3 | L TL T  R B BL |
| 2,4 | L T R B |
| 2,5 | L T TR R BR B |
| 2,6 | L T B |
| 3,0 | |
| 3,1 | |
| 3,2 | T |
| 3,3 | L TL T R BR B BL |
| 3,4 | L T R BR B BL |
| 3,5 | L T TR R BR B BL |
| 3,6 | L R B |
| 4,0 | |
| 4,1 | |
| 4,2 | |
| 4,3 | |
| 4,4 | |
| 4,5 | |
| 4,6 | L T R |
| 5,0 | |
| 5,1 | |
| 5,2 | |
| 5,3 | |
| 5,4 | |
| 5,5 | |
| 5,6 | |
| 6,0 | |
| 6,1 | |
| 6,2 | |
| 6,3 | |
| 6,4 | |