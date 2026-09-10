## Purpose

Defines systemic flammable entity traits (`Flammable2D`), dynamic point light emission (`LightEmitter2D`), ignition propagation, extinguish conditions, and interactive torch objects.

## ADDED Requirements

### Requirement: Flammable State and Lifecycle Management
The `Flammable2D` trait SHALL track whether an entity is actively burning, manage ignition/extinguish state changes, support configurable burn durations, and emit lifecycle signals.

#### Scenario: Unlit entity is ignited
- **WHEN** an unlit `Flammable2D` receives an ignition trigger (via `Ignite()` or contact with an active ignition source)
- **THEN** its `IsBurning` state transitions to `true`, and it emits the `Ignited` signal

#### Scenario: Burning entity is extinguished
- **WHEN** an actively burning `Flammable2D` receives an extinguish trigger (via `Extinguish()` or contact with water)
- **THEN** its `IsBurning` state transitions to `false`, and it emits the `Extinguished` signal

#### Scenario: Timed burn duration elapses
- **WHEN** a `Flammable2D` with `BurnDuration > 0` remains burning for its full duration
- **THEN** it emits the `BurnCompleted` signal and executes its configured completion action (`Extinguish`, `DestroyEntity`, or `TriggerSignal`)

### Requirement: Contact Ignition Propagation
Actively burning `Flammable2D` entities SHALL act as ignition sources that propagate fire to overlapping or contacted unlit `Flammable2D` entities.

#### Scenario: Burning entity contacts unlit flammable entity
- **WHEN** an actively burning `Flammable2D` collides with or enters the detection area of an unlit `Flammable2D`
- **THEN** the unlit entity is ignited and transitions its `IsBurning` state to `true`

### Requirement: Water Extinguishment
`Flammable2D` entities with `ExtinguishInWater` enabled SHALL automatically extinguish when contacting water volumes.

#### Scenario: Burning entity enters water volume
- **WHEN** a burning `Flammable2D` with `ExtinguishInWater` enabled enters a `WaterVolume` area
- **THEN** it immediately calls `Extinguish()`, setting `IsBurning` to `false` and emitting `Extinguished`

### Requirement: Dynamic Point Light Emission and Flicker
The `LightEmitter2D` component SHALL encapsulate a `PointLight2D` with configurable color, energy, radius scale, shadow support, and subtle procedural flame flickering, and SHALL synchronize with the parent/sibling `Flammable2D` burning state.

#### Scenario: Flammable state toggles light emission
- **WHEN** a `Flammable2D` attached to or referencing a `LightEmitter2D` transitions between burning and unlit
- **THEN** the `LightEmitter2D` enables point light visibility and energy when burning, and disables light visibility when unlit

#### Scenario: Procedural flicker modulation
- **WHEN** the `LightEmitter2D` is actively emitting with flicker enabled
- **THEN** its energy and texture scale subtly modulate continuously using smooth procedural noise/variation

### Requirement: Interactive Torch Object
The `Torch` scene SHALL integrate `Flammable2D` and `LightEmitter2D` to provide an in-world torch that defaults to unlit, can be configured pre-lit in inspector, and updates its visual flame and light on ignition/extinguish.

#### Scenario: Unlit torch struck by fire arrow or flame
- **WHEN** an unlit `Torch` scene is contacted by an active ignition payload
- **THEN** the torch's `Flammable2D` component transitions to burning, activating its flame visual particles and `LightEmitter2D`
