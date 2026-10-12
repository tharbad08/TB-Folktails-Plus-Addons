# Folktails+ Addons

Combines three separate Timberborn 1.1 Folktails+ enhancements with dependency-aware packaging and a C# Mod Settings integration (pending compilation/in-game validation).

## Features
- **Charging Boost** — independent of other optional mods; defines an isolated bot need collection, registers the need only for Folktails, and updates the Iron Teeth Charging Station attraction.
- **Tiny Tubeway Station** — requires `BobingaboutTinyTubewayStation`.
- **Scientific Projects** — requires `ScientificProjects` and `AlternativeRecipesSPs`; grants Folktails five previously Iron-Teeth-only projects.

**Required**: Folktails+ v11.0.0+, Timberborn v1.1.1+, eMka Mod Settings for the native settings window. Optional dependencies are for load order; assets must still be gated.

## Design
Do not ship the full replacement Folktails+ manifest/collection from the ChargingBoost input. Only append/add minimal blueprints. Dependent patches must not load unless their required mods are **enabled** (not merely installed). Native in-game checkboxes use Mod Settings; blueprint changes take effect after game restart. The optional materializer only changes this addon's own blueprint files.

## Validation and release status
Packaging tests pass. The native C# DLL has **not** been compiled or game-validated because Timberborn/Mod Settings runtime assemblies are not accessible in this environment. No full release with verified in-game settings is claimed. The Python packager can create a dependency-gated, one-folder ZIP without the DLL; in that case selecting features happens during packaging, not through an in-game settings window.

Use `python -m unittest discover -s tests -v` and `python tools/build.py --help`.

See the source ZIP supplied in the chat for the entire snapshot and local tests.
