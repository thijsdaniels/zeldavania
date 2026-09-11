# Breakable Jars Specification

## Purpose

Provides destructible decorative pottery props that shatter when attacked and spawn loot items or debris effects.

## Requirements

### Requirement: Smashable Damage Interaction
The breakable jar SHALL detect player attacks (sword slashes, arrows) via its damageable and hurtbox components and shatter upon taking 1 point of damage.

#### Scenario: Striking jar with sword attack
- **GIVEN** an intact jar placed in the world
- **WHEN** the player performs a sword attack that overlaps the jar's hurtbox
- **THEN** the jar is depleted of hit points and triggers its destruction sequence.

#### Scenario: Striking jar with arrow projectile
- **GIVEN** an intact jar placed in the world
- **WHEN** an arrow projectile collides with the jar's hurtbox
- **THEN** the jar is depleted of hit points, the arrow embeds or shatters, and destruction begins.

### Requirement: Jar Destruction Sequence
The breakable jar SHALL play a destruction particle effect (poof/debris), trigger its attached loot component to roll drops, and remove itself from the active scene.

#### Scenario: Shattering sequence
- **WHEN** a jar's hit points are depleted
- **THEN** a destruction puff/debris effect is spawned at the jar's position, the lootable drop roll is executed, and the jar entity is freed.

### Requirement: Non-Solid Physical Presence
The breakable jar SHALL be an intangible prop that does not block player locomotion.

#### Scenario: Player walking past intact jar
- **WHEN** the player walks through or past an intact jar
- **THEN** player movement is not obstructed by solid collision.
