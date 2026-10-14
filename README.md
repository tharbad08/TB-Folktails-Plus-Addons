# Folktails+ Addons 1.2.4

Single Timberborn 1.1 mod with four independent options and persistent settings. Built on Timberborn's native `IBlueprintModifierProvider`, **faction-scoped `BlueprintModifiers`**, and eMka Mod Settings. No runtime file writes or Harmony patches.

## Features

| Setting | Effect | Other required mods |
| --- | --- | --- |
| Charging Station Boost | In **Folktails only**, changes the bot Power need from basic Energy to optional ChargingBoost under Boosts. | Folktails+ |
| Tiny Tubeway Station | Adds Tiny Tubeway Station to Folktails+ if installed and its blueprint exists. | Tiny Tubeway Station |
| Scientific Projects | Enables five Iron Teeth scientific projects in Folktails. | Scientific Projects + Alternative Recipes SPs |
| FT Mixer — Real Costs | Applies material/science costs of four Iron Teeth buildings. | Folktails+ |

All features default to enabled. Dependencies are checked against enabled mods. Settings changes require restarting the game.

## How Charging Boost is implemented

**This is an addon, not a replacement collection.** Versions 1.2.2/1.2.3 had incorrect approaches that allowed basic Power to remain or replaced an entire need collection, respectively. v1.2.4 corrects both problems using existing game APIs and JSON array operators:

1. `AddonBlueprintModifierProvider` contributes a **conditional** `FactionSpec.BlueprintModifiers#append` entry for `Faction.Folktails`, only if the Charging Station Boost checkbox is enabled.
2. The game's built-in `FactionBlueprintModifierProvider` applies `Modifiers/NeedCollection.IronTeeth.FolktailsPlusAddons.blueprint` **only for the selected Folktails faction**, to the normal `NeedCollection.IronTeeth` already attached by Folktails+.
3. That tiny JSON patch uses `Needs#remove: ["Energy"]` and `Needs#append: ["ChargingBoost"]`. It **preserves every other need** in that collection, including needs added by other mods.
4. The original ChargingBoost `NeedSpec` with `NeedGroupId: "Boosts"` remains statically present for save compatibility; the existing native charging-station effect patch provides `ChargingBoost` when enabled.

**Iron Teeth-only games retain the original `Energy` need.** In Folktails, disabling Charging Station Boost stops applying the conditional faction modifier; the original Energy need returns. The mod does not replace the Folktails faction or any need collection wholesale.

### ChargingBoost patch

```json
{
  "NeedCollectionSpec": {
    "Needs#remove": ["Energy"],
    "Needs#append": ["ChargingBoost"]
  }
}
```

## Mixer costs

| Building | Resources | Science |
| --- | --- | ---: |
| Deep Water Pump | 24 Logs | 100 |
| Deep Badwater Pump | 40 Logs + 20 Gears + 20 Metal Parts | 500 |
| Deep Mechanical Fluid Pump | 100 Gears + 50 Treated Planks + 50 Metal Blocks | 5000 |
| Industrial Lumber Mill | 40 Logs | 200 |

## Installation / updating

**Delete the old `FolktailsPlusAddons` folder completely** before extracting this package; old versions wrote files and later versions installed conflicting full need collections. This release includes one mod folder, one manifest, one DLL, two static JSON blueprints (ChargingBoost NeedSpec + faction-scoped modifier) and Workshop artwork.

Required: Folktails+ 11+, Mod Settings 1.1.1+, TimberUi 11+. Optional companions as listed above.

Make a backup of existing saves before testing. In-game confirmation is required; compiling and validating JSON does not prove that selected bot needs are restored in existing saves.

## Implementation references

- `Runtime/AddonRuntime.cs`: native game provider, enabled-mod detection, Mod Settings checkbox and optional feature routing
- `Mod/Payload/ChargingBoost/Factions/Faction.Folktails.blueprint.json.template`: dynamic `BlueprintModifiers#append` on Folktails **only**
- `Mod/Static/version-1.1/Modifiers/NeedCollection.IronTeeth.FolktailsPlusAddons.blueprint.json`: targeted `Energy` removal and `ChargingBoost` append
- `Mod/Static/version-1.1/Needs/Need.Bot.ChargingBoost.blueprint.json`: stable bonus need spec under Boosts
- `.github/workflows/build.yml`: .NET build, syntax/packaging assertions, regression checks
- `WorkshopCover.png` / `WorkshopPreview.jpg`: Workshop media (not an in-game icon)

## Known independent issue

Missing `TWMoreMines` material collection warnings originate in More Mines and are not addressed by this addon.

## Changelog

**1.2.4:** Replace whole-collection/faction swap with conditional, **faction-scoped** blueprint JSON modifiers that remove only `Energy` and append `ChargingBoost`. All unrelated needs are preserved.

**1.2.3:** Copied full 32-need collection and rewired faction (superseded).

**1.2.2:** Appended bonus to a non-existent collection (superseded).

**1.2.1:** Corrected `BlueprintFileBundleLoader.GetBundle` return-type API mismatch.

**1.2.0:** Native blueprint modifiers/settings architecture.

Author: **tharbad08**
