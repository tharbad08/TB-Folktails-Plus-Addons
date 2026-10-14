# Folktails+ Addons 1.2.2

One mod, four independently configurable features, **native mod detection** and **no runtime writes to mod folders**.

## What's changed

Replaced the custom Harmony startup patch, reflection-based dependency detection, file copying/deleting and .txt preferences. Instead, the mod now uses Timberborn's built-in `IBlueprintModifierProvider` to contribute overlay data at blueprint deserialization, `ModRepository.ModIsNotEnabled` to detect enabled mods, and eMka Mod Settings' `ModSetting<bool>` with Timberborn's `ISettings` persistence.

## Features

| Feature | Dependency | Description |
| --- | --- | --- |
| Charging station boost | Folktails+ | Charging station applies ChargingBoost to bots |
| Tiny Tubeway | Tiny Tubeway Station | Adds the tiny station to Folktails+ only if its blueprint path is present |
| Scientific projects | Scientific Projects and Alternative Recipes SPs | Adds five projects to Folktails |
| FT Mixer — real costs | Folktails+ | Adjusts four Iron Teeth building costs |

The ChargingBoost need spec and a small overlay on **Folktails+'s own `NeedCollection.FolktailsPlus`** are always present, as in the original ChargingBoost addon. The checkbox controls only the charging station's boost effect. Existing saves can therefore still deserialize ChargingBoost when the effect is disabled. The previous v1.2.1 implementation incorrectly registered a separate `FolktailsPlusAddons.ChargingBoost` collection, which could leave the bonus need absent from bots.

**FT Mixer costs:** Deep Water Pump = 24 logs, 100 science; Deep Badwater Pump = 40 logs, 20 gears, 20 metal parts, 500 science; Deep Mechanical Fluid Pump = 100 gears, 50 treated planks, 50 metal blocks, 5000 science; Industrial Lumber Mill = 40 logs, 200 science.

## Install

**Completely delete the old FolktailsPlusAddons folder** before extracting this ZIP. Older versions wrote generated blueprint files that can otherwise conflict. The release contains one folder, one manifest, one DLL, two static charging-need blueprints and the Workshop cover.

Required mods: Folktails+ 11+, Mod Settings 1.1.1+, TimberUi 11+. Tiny Tubeway Station, Scientific Projects and Alternative Recipes SPs are optional. Change switches under Mods → Folktails+ Addons → Settings. Restart to apply.

Back up any saved colony showing missing Tiny Tubeway buildings before testing. This build will not automatically reconstruct already-lost save entities.

## Developer layout

- `Runtime/AddonRuntime.cs`: native `IBlueprintModifierProvider` and built-in Mod Settings booleans
- `Mod/Static/version-1.1/`: the ChargingBoost need blueprint and the Folktails+ need-collection append patch
- `Mod/Payload/`: 14 JSON overlays embedded directly in DLL as resources (not installed as files)
- `Mod/manifest.json`: one mod ID with only core required dependencies
- `.github/workflows/build.yml`: GitHub Actions build, tests and single ZIP packaging

The mod does not write or delete files at runtime and does not use Harmony. Compilation and packaging alone are not a substitute for in-game testing.

## Workshop

`WorkshopCover.png` and `WorkshopPreview.jpg` are promotional assets (no in-game icon).

## Diagnostics

**Startup crash fixed in v1.2.1:** v1.2.0 was compiled against a stub that incorrectly described `BlueprintFileBundleLoader.GetBundle(string)` as returning `object`. Timberborn 1.1 returns `BlueprintFileBundle`. The CLR method signature includes its return type, so the old DLL threw `MissingMethodException` when Tiny Tubeway detection ran. v1.2.1 now uses the correct return type and verifies the API contract during the build. The source of this signature was checked against Timberborn's game-reflection API.

**Separate outstanding error:** If `FactionSpecMaterialsValidator` reports missing `TWMoreMines`, this comes from the More Mines material collection and is not fixed by the addon. Backup the save and test with More Mines disabled or updated if startup still fails.

## Charging Boost: Power versus Energy

Timberborn's original bot **Energy** requirement is a `BasicNeeds` need and remains essential. ChargingBoost is a **second**, optional wellbeing need under `Boosts`, using the in-game **Power** display name, supplied by the original ChargingBoost compatibility mod. The addon must append `ChargingBoost` to Folktails+'s own `NeedCollection.FolktailsPlus`, not introduce an unrelated collection. **v1.2.2 restores that original wiring.** It does not move or disable the essential Energy requirement.

## Changelog

**1.2.2** — Restored original ChargingBoost registration in Folktails+'s existing `NeedCollection.FolktailsPlus`; removed redundant faction attachment and separate need collection; regression tests verify the exact merge and Boosts group.


**1.2.1** — Corrected the return type for the built-in `BlueprintFileBundleLoader.GetBundle` call and added a CI regression guard against the incorrect stub signature.


**1.2.1** — Native engine blueprint modifiers and persisted settings, no runtime file access, retained saved charging need, tiny-station blueprint presence check.

**1.1.5** — Early activation hook (superseded).
