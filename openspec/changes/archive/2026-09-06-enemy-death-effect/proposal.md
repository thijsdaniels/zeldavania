## Why

Currently, when an enemy without a dedicated death animation (such as the Bat) runs out of health, it gets stuck in its single white hit frame, falling slowly under gravity for ~1 second before despawning. Conversely, enemies with a death animation (like the Goblin) play their animation but do not have a satisfying death impact or visual flourish upon finishing.

A generalized, Zelda-style enemy death effect with immediate hitbox deactivation and crisp explosion audio/visuals provides instant player feedback, cleans up dying enemy interactions, and serves as a universal system for all present and future enemies.

## What Changes

- **Universal Death Effect (`EnemyDeathEffect`)**: Create an auto-cleanup visual/audio effect (Zelda-style smoke poof / particle burst + `explode.wav` with pitch variation) spawned at enemy defeat locations.
- **Immediate Combat Deactivation on Defeat**: When entering `EnemyDying`, immediately disable the enemy's hurtbox and contact hitbox so dying enemies cannot deal damage or absorb additional attacks.
- **Hybrid Death Animation Handling**: Update `EnemyDying.cs` to check if the enemy's `SpriteFrames` contains the configured `_animation` (e.g., `"Dying"`):
  - **Animation present (e.g., Goblin)**: Plays the dying animation with momentum and gravity, then spawns `EnemyDeathEffect` upon completion before `QueueFree()`.
  - **No animation present or empty (e.g., Bat)**: Immediately spawns `EnemyDeathEffect` and frees the enemy with zero delay.

## Capabilities

### Modified Capabilities
- `enemy-behavior`: Update defeat and cleanup requirements to deactivate combat hitboxes/hurtboxes and instantiate a universal death effect on completion (or immediately if no dying animation exists).

## Impact

- Affected scripts: `EnemyDying.cs`, `Bat.tscn`, `Goblin.tscn`.
- New assets & scenes: `scenes/objects/EnemyDeathEffect.tscn`, `scripts/objects/EnemyDeathEffect.cs`, and death effect sprites/particles.
- Audio: Integrates existing `res://assets/sounds/effects/explode.wav`.
