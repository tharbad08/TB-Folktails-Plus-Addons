# TB Folktails+ Addons

Combines the three user-supplied Folktails+ patches without replacing Folktails+'s manifests, collections, or building definitions.

## Ready-to-extract, dependency-safe version

The `Installable/` directory has three **individually gated Timberborn addon mods**, bundled in a **single ZIP**. Each uses Timberborn's `RequiredMods` checks, so the game will not activate an addon whose dependency is disabled or missing. You can enable or disable them separately in Timberborn's built-in mod manager. The three components are Charging Boost (Folktails+), Tiny Tubeway (Folktails+ and BobingaboutTinyTubewayStation), and Scientific Projects (Folktails+, ScientificProjects, AlternativeRecipesSPs).

Extract the contents of the release ZIP **directly to** `Documents/Timberborn/Mods/` and restart the game. Do not extract all addons into one extra parent directory.

The original Charging Boost input had a full Folktails+ replacement manifest and building collection; those are intentionally excluded. The Charging Boost patch now appends its need to Folktails+'s existing need collection.

## Settings and detection

The *working, data-only* release relies on the game's built-in mod-manager checkboxes and dependency checks, **not** a custom in-game settings panel. The `src/` code under this repository was a prototype and is not part of that release. It has not been successfully built or game-tested. The requested TB Optimizer-style custom settings UI requires a separately compiled and runtime-validated DLL.

The original source archive is preserved in the conversation; no assemblies from optional mods are redistributed here.

## Validation

- All 12 files in the data-only package were checked as valid JSON.
- Each of the three addon manifests contains explicit dependency requirements.
- No direct replacement of Folktails+'s building collections or full faction manifest.
- Packaging validated locally. The game itself was not run in this environment.

## Original projects
- [Folktails+](https://github.com/datvm/TimberbornMods/tree/master/FolktailsPlus)
- [Scientific Projects](https://github.com/datvm/TimberbornMods/tree/master/ScientificProjects)
