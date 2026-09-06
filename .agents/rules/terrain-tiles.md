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
* `0:0`: **Left Endcap** — `→` (`right_side`)
* `1:0`: **Looping Middle** — `←→` (`left_side`, `right_side`)
* `2:0`: **Right Endcap** — `←` (`left_side`)

### 1D Vertical Climbables (`Match Sides`)
Standard 3-tile vertical climbing spritesheets ($16 \times 48$):
* `0:0`: **Top Endcap / Exit** — `↓` (`bottom_side`)
* `0:1`: **Repeating Shaft / Rungs** — `↑↓` (`top_side`, `bottom_side`)
* `0:2`: **Bottom Endcap** — `↑` (`top_side`)

### 2D Solid Ground: Standard 34-Tile Layout (`Match Corners and Sides`)
Every 2D solid terrain sheet uses the standard $7 \times 7$ grid layout. Each cell at row $Y$ and column $X$ (atlas coordinate `X:Y`) connects to neighboring tiles in the indicated arrow directions (`↖ ↑ ↗ ← → ↙ ↓ ↘`):

| Row \ Col | 0 | 1 | 2 | 3 | 4 | 5 | 6 |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **0** | - | → | ←→ | ← | ↖↑↗←→↙↓↘ | ←→↙↓↘ | ↖↑↗←→ |
| **1** | →↘↓ | ←↙↓ | ↓ | ↖↑↗←→↙↓ | ↖↑↗←→↓ | ↖↑↗←→↓↘ | ↑→↓ |
| **2** | ↑↗→↘↓ | ↖↑←↙↓ | ↑↓ | ↖↑←↙↓→ | ↑↓←→ | ↑↗→↘↓← | ↑↓← |
| **3** | ↑↗→ | ↖↑← | ↑ | ↖↑←↙↓↘→ | ↑←↙↓↘→ | ↖↑↗→↘↓↙← | ←→↓ |
| **4** | →↓ | ←↓ | ↑↗→↓ | ↖↑←↓ | ←↙↓→ | ←↘↓→ | ←↑→ |
| **5** | ↑→ | ↑← | ↑→↘↓ | ↑←↙↓ | ↖↑←→ | ↗↑←→ | ↑↗→↓↙← |
| **6** | ↖↑↗→↘↓ | ↑←↙↓→ | ↑→↘↓← | ↖↑←↓→ | ↗↑→↓← | - | - |