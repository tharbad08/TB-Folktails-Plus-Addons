# Folktails+ Addons 1.2.3

Four independent features in one Timberborn **1.1** addon, with enabled-mod detection and persistent checkboxes through the native Timberborn and eMka Mod Settings APIs. No runtime file writing or Harmony hook is used.

## Features

| Toggle | What it does | Optional dependencies |
| --- | --- | --- |
| Charging Station Boost | Makes Charging Stations provide `ChargingBoost`, displayed as **Power** under **Boosts**, instead of the Iron Teeth basic Energy need | None |
| Tiny Tubeway Station | Integrates the station when its mod and blueprint are available | Bobingabout Tiny Tubeway Station |
| Scientific Projects | Enables five Iron Teeth science projects for Folktails | Scientific Projects and Alternative Recipes Scientific Projects |
| FT Mixer — Real Costs | Changes four Iron Teeth building costs for mixed-faction Folktails colonies | None |

**All four checkboxes are enabled by default.** Feature-specific dependencies must be enabled, and changes require a game restart.

## Power / ChargingBoost: important compatibility detail

**v1.2.3 corrects the Basic Needs regression in v1.2.1 and v1.2.2.** The original user-supplied `FolktailsPlus_11.0.1_ChargingBoost` package is *not* merely a bonus need appended to an existing collection. It changes the Folktails faction's need-collection wiring:

- The upstream **Folktails+** appends `IronTeeth` to `Faction.Folktails.NeedCollectionIds`. The Iron Teeth need collection contains `Energy` (shown as **Power** under **Basic Needs**).
- The original ChargingBoost package instead selects a dedicated **FolktailsPlus** need collection containing 32 specific wellbeing needs, **including `ChargingBoost` and excluding `Energy`**. Its `ChargingBoost` NeedSpec has `NeedGroupId: Boosts` and `DisplayNameLocKey: Need.Power.DisplayName`.
- This addon now restores that *actual original behavior* with a **minimal faction-specific overlay**: remove `IronTeeth` from Folktails' need collection IDs, append `FolktailsPlus`, and supply the complete original 32-need collection. Iron Teeth's own faction/need collection is not touched.
- The bot ChargingBoost definition is always installed so existing saves can recognize it; the checkbox controls only the Charging Station effect. Disabling the checkbox does **not** restore the basic Energy need.

**Gameplay caveat:** Unlike a purely cosmetic UI change, the original package replaces the standard Iron Teeth bot Energy need in Folktails with a custom ChargingBoost need. This may affect bot need mechanics in existing saves. **Back up your save** before testing and do not overwrite the last known-good copy until you confirm the behavior.

## FT Mixer costs

| Iron Teeth building | Materials | Science |
| --- | --- | ---: |
| Deep Water Pump | 24 Logs | 100 |
| Deep Badwater Pump | 40 Logs, 20 Gears, 20 Metal Parts | 500 |
| Deep Mechanical Fluid Pump | 100 Gears, 50 Treated Planks, 50 Metal Blocks | 5000 |
| Industrial Lumber Mill | 40 Logs | 200 |

## Installation

1. Delete the entire **old** `FolktailsPlusAddons` mod folder. Versions before 1.2.0 could leave dynamically generated blueprint files behind.
2. Extract this ZIP so the *single* `FolktailsPlusAddons` folder resides in your local `Documents/Timberborn/Mods/` directory (or corresponding Linux/Proton directory).
3. Keep **Folktails+ 11+**, **Mod Settings 1.1.1+**, and **TimberUi 11+** enabled. The other mods listed under features are optional.
4. Restart Timberborn, configure features under **Mods → Folktails+ Addons → Settings**, and restart again after changes.

Existing saves: create a separate copy before opening a colony previously affected by missing ChargingBoost or Tiny Tubeway templates.

## Design and source

- `Mod/Static/version-1.1/NeedCollections/NeedCollection.FolktailsPlus.blueprint.json` — complete 32-item collection copied from the original package (not an append to a nonexistent blueprint)
- `Mod/Static/version-1.1/Factions/Faction.Folktails.blueprint.json` — targeted `NeedCollectionIds#remove` / `#append` overlay; unrelated faction fields are untouched
- `Mod/Static/version-1.1/Needs/Need.Bot.ChargingBoost.blueprint.json` — original bonus NeedSpec in Boosts
- `Runtime/AddonRuntime.cs` — `IBlueprintModifierProvider`, optional-mod checks, and Mod Settings booleans
- `Mod/Payload/` — supporting feature patches embedded in the DLL
- `.github/workflows/build.yml` — compile and validate the ZIP, including a SHA-256 check of the original full need collection

**Known separate issue:** `TWMoreMines` missing material collection errors originate from More Mines. This addon does not repair them.

**Compatibility testing:** GitHub Actions verifies compilation and packaging. It does not establish whether a saved game loads correctly or whether a bot’s wellbeing UI behaves as intended.

## Changelog

**1.2.3** — Restored the original complete FolktailsPlus need collection, removed IronTeeth needs only from Folktails faction, fixed Power reappearing under Basic Needs, regression-tested original 32 needs.

**1.2.2** — Tried a need append on a nonexistent FolktailsPlus collection (incorrect; superseded).

**1.2.1** — Fixed the Timberborn 1.1 `BlueprintFileBundleLoader.GetBundle` return-type API mismatch.

**1.2.0** — Switched to native blueprint modifiers and Mod Settings, replacing the custom file-copying/Harmony startup mechanism.

**1.1.4** — Added FT Mixer real costs.

## Workshop

`WorkshopCover.png` and `WorkshopPreview.jpg` are Workshop-only promotional assets. Author: **tharbad08**.
