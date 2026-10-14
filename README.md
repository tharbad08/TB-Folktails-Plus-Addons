# Folktails+ Addons v1.2.6

One addon for Timberborn 1.1, with four independently configurable features, using the native blueprint modifier API and eMka Mod Settings. No Harmony startup hooks or runtime filesystem writes.

## Features

- **Charging Station Boost** — attempts to remove basic bot Energy and append ChargingBoost under Boosts **in the Folktails need collection only**.
- **Tiny Tubeway Station** — adds the small station when its optional companion mod and building blueprint are available.
- **Scientific Projects** — makes five projects available when Scientific Projects and Alternative Recipes SPs are enabled.
- **FT Mixer — Real Costs** — applies four mixed-faction building material/science cost adjustments.

Required: Folktails+ 11+, Mod Settings 1.1.1+, TimberUi 11+. The other companion mods are optional. All four switches default to enabled; restart to apply settings changes.

## v1.2.6 — precisely scoped need JSON

After feedback, the ChargingBoost feature uses **exactly one** need-collection overlay, activated by the Charging Station Boost checkbox:

```json
{
  "NeedCollectionSpec": {
    "CollectionId": "Folktails",
    "Needs#remove": ["Energy"],
    "Needs#append": ["ChargingBoost"]
  }
}
```

It modifies only `NeedCollection.Folktails`. Removed the previous:
- `Faction.Folktails` `BlueprintModifiers#append` injection,
- `NeedCollection.IronTeeth.FolktailsPlusAddons` modifier,
- entire copied/replaced FolktailsPlus need collection.

The original ChargingBoost `NeedSpec` still exists under **Boosts**, always registered for compatibility with old saves. The charging-station effect remains controlled by the same checkbox. No IronTeeth need blueprints are touched.

### Important behavior limitation

**This change follows the requested JSON contract; it is not yet a proven fix for the UI.** Timberborn 1.1's `FactionNeedService.Load` uses the **union of the Needs lists from every collection attached to the faction**, rather than copying IronTeeth needs into Folktails beforehand. Folktails+ attaches the `IronTeeth` need collection to Folktails, so if `Energy` exists *only* in that separate collection, a `Needs#remove` on `CollectionId: Folktails` cannot remove it. Building successfully does not establish that basic Power disappears. For a genuinely guaranteed Folktails-only removal while preserving the Iron Teeth faction, we may need a different faction-specific mechanism or a post-collection filtering API. We are intentionally not applying those alternatives in this version.

## Source files

- `Mod/Payload/ChargingBoost/NeedCollections/NeedCollection.Folktails.blueprint.json.template` — conditional Folktails-only array operations
- `Mod/Static/version-1.1/Needs/Need.Bot.ChargingBoost.blueprint.json` — stable boost need definition
- `Runtime/AddonRuntime.cs` — native provider and Mod Settings integration
- `Mod/Payload/` — optional feature JSON resources embedded into one DLL

## FT Mixer costs

| Building | Materials | Science |
| --- | --- | ---: |
| Deep Water Pump | 24 Logs | 100 |
| Deep Badwater Pump | 40 Logs, 20 Gears, 20 Metal Parts | 500 |
| Deep Mechanical Fluid Pump | 100 Gears, 50 Treated Planks, 50 Metal Blocks | 5000 |
| Industrial Lumber Mill | 40 Logs | 200 |

## Install

**Delete the old FolktailsPlusAddons directory before extracting v1.2.6**, because old versions packaged additional JSONs that must not remain. The package contains a single mod folder with DLL, manifest, static ChargingBoost definition, README, and Steam Workshop cover. Back up affected saves before testing.

The separate `TWMoreMines` validation issue originates from More Mines, not this addon.

## Changelog

**1.2.6** — Only `CollectionId: Folktails`, `Needs#remove: Energy`, and `Needs#append: ChargingBoost`. Removed faction/IT modifiers; documented the engine's collection-union caveat.

**1.2.5** — Two collection modifiers: Folktails append and a faction-scoped IronTeeth removal (superseded).
**1.2.4** — Original attempted faction-scoped modification (superseded).
**1.2.3** — Incorrect replacement with fixed need list (superseded).

Author: **tharbad08**.
