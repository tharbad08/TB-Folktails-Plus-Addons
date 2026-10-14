# Folktails+ Addons v1.2.5

One mod with four independently enabled features. Uses Timberborn's built-in blueprint modifier API and eMka Mod Settings; no Harmony startup patch, reflection-based dependency scanning or runtime writes.

## Features

| Feature | Effect | Optional companion |
| --- | --- | --- |
| Charging Station Boost | Folktails' Power changes from basic `Energy` to bonus `ChargingBoost` under Boosts | None |
| Tiny Tubeway | Adds Tiny Tubeway Station to Folktails+ when both mod and blueprint are available | Tiny Tubeway Station |
| Scientific Projects | Adds five Iron Teeth scientific projects for Folktails | Scientific Projects and Alternative Recipes SPs |
| FT Mixer — Real Costs | Adjusts material/science costs of four Iron Teeth buildings | None |

Required: Folktails+ 11+, Mod Settings 1.1.1+, TimberUi 11+. All four features default on; disabled/missing optional companions affect only their feature. **Restart the game after changing settings.**

## ChargingBoost — v1.2.5 fix

**Do not replace or copy full need collections.** Timberborn 1.1 `FactionNeedService.Load` uses the **union of every NeedCollectionId** attached to a faction. Folktails+ adds the `IronTeeth` collection to Folktails, but does *not* merge that collection's `Needs` array into the `Folktails` collection. Consequently, a `Needs#remove: ["Energy"]` against the *Folktails* collection alone has no effect: Energy originates in the separately attached *IronTeeth* collection.

This release uses two small, **settings-conditional and Folktails-scoped** native JSON modifications:

**1. Add the bonus to Folktails' own need collection**, using the requested explicit CollectionId:

```json
{"NeedCollectionSpec":{"CollectionId":"Folktails","Needs#append":["ChargingBoost"]}}
```

The `AddonBlueprintModifierProvider` supplies this JSON when Timberborn deserializes `NeedCollection.Folktails`, and only if Charging Station Boost is enabled.

**2. Remove inherited basic Energy only in Folktails' context.** The same enabled feature appends one `FactionSpec.BlueprintModifiers` entry to `Faction.Folktails`; the native `FactionBlueprintModifierProvider` then applies this modifier to `NeedCollection.IronTeeth` *only when the current faction is Folktails*:

```json
{"NeedCollectionSpec":{"CollectionId":"IronTeeth","Needs#remove":["Energy"]}}
```

This does **not** globally change Iron Teeth's need collection or overwrite other Iron Teeth needs. It simply removes one inherited item for Folktails. It does not alter Folktails+ itself. All other needs added by Folktails+, Iron Teeth or unrelated mods are retained.

The original ChargingBoost `NeedSpec` is always installed (for save compatibility); it uses `NeedGroupId: "Boosts"` and the game's Power display-name key. The existing ChargingStation effect patch is controlled by the same checkbox.

### Source locations

- `Mod/Payload/ChargingBoost/NeedCollections/NeedCollection.Folktails.blueprint.json.template` — *Folktails* CollectionId and `Needs#append` only
- `Mod/Payload/ChargingBoost/Factions/Faction.Folktails.blueprint.json.template` — adds the faction-scoped reference, enabled only via native setting
- `Mod/Static/version-1.1/Modifiers/NeedCollection.IronTeeth.FolktailsPlusAddons.blueprint.json` — one-item `Needs#remove` for that faction only
- `Mod/Static/version-1.1/Needs/Need.Bot.ChargingBoost.blueprint.json` — stable bonus need specification
- `Runtime/AddonRuntime.cs` — four feature switches, native provider, optional dependency checks

## FT Mixer real costs

| Building | Materials | Science |
| --- | --- | ---: |
| Deep Water Pump | 24 Logs | 100 |
| Deep Badwater Pump | 40 Logs, 20 Gears, 20 Metal Parts | 500 |
| Deep Mechanical Fluid Pump | 100 Gears, 50 Treated Planks, 50 Metal Blocks | 5000 |
| Industrial Lumber Mill | 40 Logs | 200 |

## Install

**Delete the entire previous `FolktailsPlusAddons` directory** before extracting the updated single-folder ZIP. Earlier versions created and packaged incompatible need collection assets, which can remain if the ZIP is simply overwritten. Extract to your local Timberborn Mods directory, restart the game and enable Charging Station Boost in Mod Settings. Back up affected saved worlds before loading or saving.

## Validation and limitations

GitHub Actions builds against API reference stubs and asserts the exact JSON for both scoped need changes, valid manifest, compiled DLL, dependency/settings code, Workshop cover and all four feature payloads. This catches regression to replacing a full collection, to a global modification of IronTeeth, or to omitting `CollectionId: "Folktails"`.

**Still requires in-game confirmation** on Timberborn 1.1: actual parsing, provider invocation and bot wellbeing categories cannot be established by compilation alone. Send a fresh `Player.log` from **v1.2.5** if Power remains a Basic Need. The independent `TWMoreMines` missing material collection warning is unrelated.

## Changelog

**1.2.5** — Explicit Folktails CollectionId, conditional boost append, minimal faction-scoped inherited Energy removal and extra regression coverage.
**1.2.4** — First attempted faction-scoped modifier; ChargingBoost append was attached to inherited IronTeeth instead of Folktails.
**1.2.3** — Superseded full collection replacement.
**1.2.2** — Superseded nonexistent collection append.
**1.2.1** — Fixed Timberborn blueprint-loader return type.
**1.2.0** — Native blueprint provider architecture.

Author: **tharbad08**. Workshop promotional cover is included; no in-game icon.
