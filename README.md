# Folktails+ Addons v1.2.7

Four independently configurable Folktails+ features: Charging Station Boost, Tiny Tubeway Station integration, five Scientific Projects, and FT Mixer real building costs. Requires Folktails+ 11+, Mod Settings 1.1.1+, and TimberUi 11+.

## ChargingBoost

For the Folktails+ gameplay configuration, Timberborn combines Folktails and IronTeeth need collections. Version 1.2.6 removed Energy from the wrong collection, so Power appeared in Basic Needs and Boosts.

v1.2.7 uses the game's native blueprint modifier API. When the Charging Station Boost option is enabled, it applies these **two minimal, conditional** JSON modifications:

- `NeedCollection.Folktails`: `{"NeedCollectionSpec":{"CollectionId":"Folktails","Needs#append":["ChargingBoost"]}}`
- `NeedCollection.IronTeeth`: `{"NeedCollectionSpec":{"CollectionId":"IronTeeth","Needs#remove":["Energy"]}}`

The original `ChargingBoost` definition stays in the `Boosts` group, including when the feature is turned off, to avoid missing-need errors in old saves. No entire need collections are overwritten; no faction-specific modifier or Harmony patch is used.

**Scope:** Energy is removed from the shared IronTeeth collection while the feature is on. To play the vanilla Iron Teeth faction, **disable both Folktails+ and Folktails+ Addons and restart Timberborn**. This follows the intended use of the two mods.

## Other features

Tiny Tubeway Station is enabled only when its companion mod/blueprint is available. Scientific Projects requires Scientific Projects and Alternative Recipes SPs. FT Mixer real costs sets Deep Water Pump to 24 Logs/100 Science, Deep Badwater Pump to 40 Logs + 20 Gears + 20 Metal Parts/500 Science, Deep Mechanical Fluid Pump to 100 Gears + 50 Treated Planks + 50 Metal Blocks/5000 Science, and Industrial Lumber Mill to 40 Logs/200 Science.

## Installation

Delete the existing `FolktailsPlusAddons` folder completely before installing the updated single-folder ZIP. Earlier builds contained incompatible static need modifiers. Keep a backup of an affected save before loading and saving it.

The mod's four toggles are enabled by default under Mods → Folktails+ Addons → Settings. Changes require a game restart.

## Verification

The CI build compiles the DLL and checks the two JSON overlays plus the packaging and other features. Compilation alone does not validate in-game wellbeing behavior.

Author: **tharbad08**. Workshop cover included.
