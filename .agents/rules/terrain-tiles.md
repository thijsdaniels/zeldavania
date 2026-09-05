---
trigger: model_decision
description: When drawing terrain tiles in a tileset layer.
---

Every terrain tile spritesheet contains 34 sprites, each one for a specific combination of adjacent tiles. For example, the tile at coordinate 0,3 should be used only when is an adjacent tile from the same spritesheet on its left. In other words, it is the right end-cap of a single-height group.

| Coordinate | Adjacent Tiles |
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
| 2,0 |
| 2,1 |
| 2,2 | T B |
| 2,3 | L TL T  R B BL |
| 2,4 | L T R B |
| 2,5 | L T TR R BR B |
| 2,6 | L T B |
| 3,0 |
| 3,1 |
| 3,2 | T |
| 3,3 | L TL T R BR B BL |
| 3,4 | L T R BR B BL |
| 3,5 | L T TR R BR B BL |
| 3,6 | L R B |
| 4,0 |
| 4,1 |
| 4,2 |
| 4,3 |
| 4,4 |
| 4,5 |
| 4,6 | L T R |
| 5,0 |
| 5,1 |
| 5,2 |
| 5,3 |
| 5,4 |
| 5,5 |
| 5,6 |
| 6,0 |
| 6,1 |
| 6,2 |
| 6,3 |
| 6,4 |