# Folktails+ Addons 1.1.3 — one mod with optional features

One install folder `FolktailsPlusAddons`, one manifest, one compiled C# DLL, and **three independent checkboxes** within the Mod Settings menu. The settings UI follows the [TB Optimizer](https://github.com/tharbad08/TimberbornMods/tree/TB-Optimizer/BenchmarkAndOptimizerV11Port) pattern (ModSettingsOwner, custom setting element factory, short labels and explanation text).

## Features
| Feature | Required enabled mods |
| --- | --- |
| Charging station boost | Folktails+ |
| Tiny Tubeway Station | Folktails+; `BobingaboutTinyTubewayStation` |
| Scientific projects | Folktails+; `ScientificProjects`; `AlternativeRecipesSPs` |

**Mod-wide requirements:** Folktails+ v11+, eMka Mod Settings, TimberUi. Others are *optional* and declared `OptionalMods` to load before this addon if present.

## How dependency detection works
The runtime reads **enabled** mod IDs from Timberborn's `ModRepository` (not merely installed folders). When the main-menu settings owner initializes, it reconciles the selected features with the available dependencies, installing/removing this addon's `.blueprint.json` patches from its own `version-1.1/` folder. Feature payload templates remain in `Payload/` outside the blueprint search path. Missing prerequisites disable that feature (and its checkbox) without preventing other features from loading. Settings are saved to `folktails-addons-options.txt`; changes need a game restart.

**Limitations:** This uses writable local mod-directory materialization to activate blueprints; Steam Workshop installations may be read-only. It is a compile-targeted adaptation of TB Optimizer's settings, not verified against an actual game runtime. Direct-to-save launch and blueprint scanning before main-menu settings initialization are not verified. The GitHub Actions build compiles against the TB Optimizer reference stubs, which validates API syntax but is not a substitute for integration testing against the game binaries.

## Release
Go to [Build Folktails+ Addons](https://github.com/tharbad08/TB-Folktails-Plus-Addons/actions/workflows/build.yml) and download the latest successful artifact `FolktailsPlusAddons_1.1.3`. It contains a single mod folder (not three mods). Extract it under your Timberborn Mods directory and remove old addons.

## File ownership
- `Runtime/AddonRuntime.cs`: dependency detection, options, menu UI and blueprint materialization
- `Mod/manifest.json`: core and optional mod requirements
- `Mod/Payload/<feature>/`: blueprint templates; no replacement Folktails+ manifest
- `.github/workflows/build.yml`: compile C# and package one mod folder, validate contents
