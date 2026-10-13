# Folktails+ Addons — single mod

The release has exactly **one** mod directory, **one** manifest and **one** mod ID (`tharbad08.FolktailsPlusAddons`). It combines three features:

- Charging Boost for bots using the Iron Teeth charging station
- Tiny Tubeway Station available to Folktails
- Five Scientific Projects available to Folktails

## Install

Extract `FolktailsPlusAddons` from the all-in-one release ZIP directly to `Documents/Timberborn/Mods/`. Remove the previously installed `FolktailsPlusAddons-ChargingBoost`, `FolktailsPlusAddons-TinyTubeway`, and `FolktailsPlusAddons-ScientificProjects` folders to avoid duplicate blueprint changes. Enable the new mod and restart Timberborn.

## Dependencies and important limitation

This **single-mod, data-only** build requires all four dependencies: Folktails+ >= 11.0.0, Bobingabout Tiny Tubeway Station >= 1.1.1.0, Scientific Projects and Alternative Recipes Scientific Projects. If any dependency is missing or disabled, Timberborn disables the whole bundle. This is intentional: a JSON-only mod cannot safely conditionally include the Tiny Tubeway collection reference when its target mod is absent.

**The previous source-only settings prototype is not shipped as a DLL** and this release does *not* implement independent per-feature toggles or automatic optional-dependency deactivation. Those features require separately compiled and validated runtime integration. Do not mistake this for a completed TB Optimizer-style settings build.

## Blueprint safety

No original Folktails+ manifest or building list is replaced. Charging Boost uses a distinct need collection and adds its collection ID to Folktails with an append operation. The Tiny Tubeway reference and five science patches are applied only after their required mods have been loaded.

## Verification

ZIP: 11 JSON files; one mod folder; one manifest. All JSON parsed; ZIP integrity checked. This is packaging-level verification only; in-game validation has not been possible.
