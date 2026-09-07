## ADDED Requirements

### Requirement: Active Item Action Cluster Display
The HUD SHALL display a diamond-arranged action slot widget showing the equipped items and button prompts for slots X, Y, and B.

#### Scenario: Rendering equipped item icons
- **GIVEN** Slot X has Sword, Slot B has Bow, and Slot Y is empty
- **WHEN** the HUD is displayed
- **THEN** Slot X renders the sword icon with an "X" button prompt, Slot B renders the bow icon with a "B" button prompt, and Slot Y renders an empty slot frame with a "Y" button prompt.

#### Scenario: Updating slot icons on inventory change
- **GIVEN** the player changes their slot assignments in the inventory
- **WHEN** an item assignment event is received by the HUD
- **THEN** the corresponding slot widget updates its icon and prompt immediately.

### Requirement: Active Consumable Ammo Counter Widget
The HUD SHALL display current consumable ammo counts for equipped consumable items.

#### Scenario: Displaying arrow count on equipped bow
- **GIVEN** the Bow is assigned to an active slot (e.g. Slot B) and the player has 20 arrows
- **WHEN** the HUD renders the active item slot
- **THEN** an ammo count badge showing "20" is displayed adjacent to the bow icon.

#### Scenario: Updating ammo count on consumption
- **GIVEN** the Bow is equipped and arrow ammo changes from 20 to 19
- **WHEN** an ammo change signal is received
- **THEN** the ammo count badge updates dynamically to "19".

#### Scenario: Non-consumable items display no ammo badge
- **GIVEN** the Sword is assigned to Slot X
- **WHEN** the HUD renders Slot X
- **THEN** no ammo count badge is displayed for the sword.
