## Why

The game currently has ground-based enemies ([`Goblin`](file:///c:/Users/thijs/Repositories/thijsdaniels/zeldavania/scenes/entities/Goblin.tscn)), but lacks aerial flying enemies to populate vertical cavern spaces, dungeons, and ceilings. Implementing the classic Bat enemy addresses **[GitHub Issue #64](https://github.com/thijsdaniels/zeldavania/issues/64)**, introducing aerial dive attacks, ceiling roosting, and sinusoidal air patrol behaviors to challenge the player in vertical platforming sections. Additionally, this change modularizes enemy specifications by extracting archetype-specific behaviors (`goblin-enemy` and `bat-enemy`) into dedicated capabilities.

## What Changes

- **Bat Sprite & Animation Asset**: Export and configure `Bat.png` (64×16 spritesheet containing 16×16 frames for Roosting/Sleeping, Flying flutter, and Hurt/Flinch) in `assets/textures/entities/`.
- **Flying Enemy States**:
  - `BatRoosting`: Hangs upside down on cave ceilings, monitoring proximity/hearing to awaken into aerial flutter.
  - `BatSwooping`: Dives in a smooth curved/parabolic arc toward the player's position after fluttering and cooldown expiration.
  - `BatFluttering`: Hovers and cruises through the air using sinusoidal wave motion (`sin(time * freq) * amp`) while tracking or repositioning.
  - `BatReturning`: Ascends back to the ceiling roost anchor point when the player moves out of detection range.
- **Bat Entity Scene (`Bat.tscn`)**:
  - Built on generic `Enemy.cs` controller (`CharacterBody2D`).
  - Integrated with `Damageable` (health pool) and `Hurtbox` (damage reception with hitstun and flash).
  - Equipped with a continuous contact `Hitbox` (`Area2D`) that inflicts contact damage when colliding with the player.
  - Wired into `EnemyHurt` and `EnemyDying` states for combat feedback and death animation.
- **Spec Modularization**:
  - Moves Goblin-specific melee stabbing attack requirement from `enemy-behavior` into a dedicated `goblin-enemy` capability.
  - Adds dedicated `bat-enemy` capability for flying enemy behaviors.
  - Keeps `enemy-behavior` focused strictly on universal base enemy mechanics (sensory perception, hitstun, falling, and defeat).

## Capabilities

### New Capabilities
- `bat-enemy`: Defines aerial flight behaviors for the Bat enemy archetype including ceiling roosting, sinusoidal aerial flutter, curved swoop attacks, and roost return navigation.
- `goblin-enemy`: Defines combat and navigation behaviors for the Goblin enemy archetype including ground chasing and directional melee spear stabbing attacks.

### Modified Capabilities
- `enemy-behavior`: Removes goblin-specific melee stabbing attack requirement so `enemy-behavior` remains dedicated strictly to universal base enemy mechanics.

## Impact

- **Entities**: New `Bat.tscn` entity scene in `scenes/entities/`.
- **Scripts**: New state scripts in `scripts/entities/enemy/states/` (`BatRoosting.cs`, `BatSwooping.cs`, `BatFluttering.cs`, `BatReturning.cs`).
- **Assets**: New `assets/textures/entities/Bat.png` texture and import metadata.
- **Combat Pipeline**: Reuses existing `Hitbox`, `Hurtbox`, `Damageable`, `EnemyHurt`, and `EnemyDying` without breaking changes.
- **Specs**: Cleanly separates `enemy-behavior`, `goblin-enemy`, and `bat-enemy`.
