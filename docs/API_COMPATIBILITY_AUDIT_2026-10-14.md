# Timberborn 1.1 runtime/API audit — 2026-10-14

## Evidence

Checked the user-provided `timberborn-build-refs-20261014-013425(1).zip` archive (502 game, 603 Workshop and 13 local-mod DLLs) against the compiled `FolktailsPlusAddons_1.2.1.zip` package.

The affected Player log identifies game **1.1.2.7** and the addon **v1.2.0**. The archive itself contains that older v1.2.0 addon binary, not v1.2.1. The *separately obtained* v1.2.1 release DLL was inspected for the actual API verification.

No proprietary Timberborn or third-party DLLs are checked into this repository.

## Verified direct external API method references

All **11** directly referenced external methods in the v1.2.1 compiled assembly were resolved against the matching installed DLLs. Return types and argument types were compared in the .NET metadata, not just method names.

| Declaring API | Method | Verified signature |
| --- | --- | --- |
| Bindito.Core.ContextAttribute | .ctor | void(string) |
| ModSettings.Core.ModSettingDescriptor | Create | ModSettingDescriptor(string) |
| ModSettings.Core.ModSettingDescriptor | SetTooltip | ModSettingDescriptor(string) |
| ModSettings.Core.ModSettingsOwner | .ctor | void(ISettings, ModSettingsOwnerRegistry, ModRepository) |
| ModSettings.Core.ModSettingDescriptor | SetEnableCondition | ModSettingDescriptor(Func<bool>) |
| Timberborn.BlueprintSystem.BlueprintFileBundleLoader | GetBundle | **BlueprintFileBundle(string)** |
| Bindito.Core.UiBuilderExtensions (TimberUi.dll) | BindSingleton | Configurator(Configurator) |
| Bindito.Core.UiBuilderExtensions (TimberUi.dll) | MultiBindSingleton | Configurator(Configurator) |
| Bindito.Core.Configurator | .ctor | void() |
| Timberborn.Modding.ModRepository | ModIsNotEnabled | bool(string) |
| Timberborn.SettingsSystem.ISettings | GetBool | bool(string, bool) |

The implemented `IBlueprintModifierProvider.GetModifiers(string)` contract also matches the game: `IEnumerable<string>(string)`. The inherited `ModSetting<T>`/descriptor operations were separately checked against Mod Settings 1.1.

**Conclusion:** No further direct method signature mismatch was identified in the v1.2.1 compiled assembly for the tested DLL versions. This does not constitute proof of in-game feature behavior.

## Feature payload validation

Extracted embedded managed resources from the actual v1.2.1 DLL, JSON-decoded every one, and compared the intended changes with the user-supplied original ZIPs:

| Feature | Resources embedded | Validation |
| --- | ---: | --- |
| Charging Boost | 4 | Original need/charging effect preserved; runtime uses one effect patch and three static compatibility blueprints |
| Tiny Tubeway | 1 | Exact original append patch preserved |
| Scientific Projects | 5 | Exact original project faction patches preserved |
| FT Mixer costs | 4 | All four original material/science cost tables preserved; unrelated building attributes intentionally not copied |

The installed v1.2.1 ZIP also contains three static ChargingBoost compatibility blueprints and a single mod manifest and DLL.

## Remaining concerns

1. **v1.2.0 is what the supplied references archive captured.** Replace that older installation with the compiled v1.2.1 package before gathering a fresh test log.
2. The references collector does **not** include all optional mods' blueprint JSON and asset bundles. It cannot confirm the Tiny Tubeway mod's exact template path, other blueprint ownership, or whether a save has already lost entities. For this, supply a complete installed Tiny Tubeway mod folder and an affected save.
3. The previous Player.log independently showed `FactionSpecMaterialsValidator` missing material collection `TWMoreMines`. The addon API fix does **not** resolve the More Mines material registration fault; it can still prevent a successful startup.
4. These checks validate binary signatures, bundled resources, manifests, and source payload equivalence; gameplay and saved-world loading still require an actual Timberborn test.

## Prevention

Do not use invented return types in compiler stubs. Specifically, `BlueprintFileBundleLoader.GetBundle(string)` returns **`BlueprintFileBundle`**, *not* `object`. CI already contains a regression guard for that known method. Future changes involving new game or mod APIs must compare all external references against the current game's real binaries before release.
