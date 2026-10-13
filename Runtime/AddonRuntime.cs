using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bindito.Core;
using ModSettings.Core;
using Timberborn.BlueprintSystem;
using Timberborn.Modding;
using Timberborn.SettingsSystem;

namespace FolktailsPlusAddons;

// Timberborn owns blueprint loading and merging; Mod Settings owns persistence.
// No Harmony, reflection, writable mod folders or custom configuration files.
internal sealed class Feature
{
    public readonly string Key;
    public readonly string Label;
    public readonly string Description;
    public readonly string[] Dependencies;
    public readonly string[] Blueprints;
    public Feature(string key, string label, string description, string[] dependencies, params string[] blueprints)
    {
        Key = key;
        Label = label;
        Description = description;
        Dependencies = dependencies;
        Blueprints = blueprints;
    }
}

internal static class AddonFeatures
{
    internal const string ModId = "tharbad08.FolktailsPlusAddons";
    internal const string TinyStationBlueprint =
        "Buildings/Paths/Tubeway/TubewayStation.Tiny.IronTeeth.blueprint";

    internal static readonly Feature[] All =
    {
        new Feature("ChargingBoost", "Charging station boost",
            "Adds the Folktails+ bot charging boost. Restart to apply.",
            new[] { "FolktailsPlus" },
            "Buildings/Science/ChargingStation/ChargingStation.IronTeeth.blueprint.json"),
        new Feature("TinyTubeway", "Tiny Tubeway Station",
            "Requires Tiny Tubeway Station and its building blueprint. Restart to apply.",
            new[] { "FolktailsPlus", "BobingaboutTinyTubewayStation" },
            "TemplateCollections/TemplateCollection.Buildings.FolktailsPlus.blueprint.json"),
        new Feature("ScientificProjects", "Scientific projects",
            "Unlocks five projects. Requires both Scientific Projects mods. Restart to apply.",
            new[] { "FolktailsPlus", "ScientificProjects", "AlternativeRecipesSPs" },
            "Blueprints/ScientificProjects/Badwaters/CrudeLogPressExtract.blueprint.json",
            "Blueprints/ScientificProjects/Factions/ItSmelterUpgrade.ScientificProjectSpec.blueprint.json",
            "Blueprints/ScientificProjects/Foods/PrecisionOilYield.blueprint.json",
            "Blueprints/ScientificProjects/Foods/RapidCanolaCrush.blueprint.json",
            "Blueprints/ScientificProjects/Woods/SteelBoundPlanks.blueprint.json"),
        new Feature("MixerCosts", "FT Mixer — real costs",
            "Applies the four mixed-faction Iron Teeth building costs. Restart to apply.",
            new[] { "FolktailsPlus" },
            "Buildings/Water/DeepWaterPump/DeepWaterPump.IronTeeth.blueprint.json",
            "Buildings/Water/DeepBadwaterPump/DeepBadwaterPump.IronTeeth.blueprint.json",
            "Buildings/Water/DeepMechanicalFluidPump/DeepMechanicalFluidPump.IronTeeth.blueprint.json",
            "Buildings/Wood/IndustrialLumberMill/IndustrialLumberMill.IronTeeth.blueprint.json")
    };

    // ModSettingsOwner derives keys from the owner type and public property name.
    // Direct reading through ISettings allows provider evaluation at the exact
    // blueprint-deserialization stage, independent of settings-screen timing.
    internal static string SettingKey(string featureKey) =>
        "ModSetting." + ModId + ".AddonSettingsOwner." + featureKey;

    internal static bool Available(ModRepository repository, Feature feature) =>
        feature.Dependencies.All(id => !repository.ModIsNotEnabled(id));
}

// Existing Mod Settings built-in checkboxes & persistence; no bespoke txt file.
public sealed class AddonSettingsOwner : ModSettingsOwner
{
    protected override string ModId => AddonFeatures.ModId;
    public override ModSettingsContext ChangeableOn => ModSettingsContext.MainMenu;

    public ModSetting<bool> ChargingBoost { get; } =
        new(true, ModSettingDescriptor.Create("Charging station boost")
            .SetTooltip("Bot charging boost for Folktails+. Takes effect after restart."));
    public ModSetting<bool> TinyTubeway { get; } =
        new(true, ModSettingDescriptor.Create("Tiny Tubeway Station")
            .SetTooltip("Adds the station only when its mod AND blueprint are available. Restart required."));
    public ModSetting<bool> ScientificProjects { get; } =
        new(true, ModSettingDescriptor.Create("Scientific projects")
            .SetTooltip("Unlock five projects if Scientific Projects and Alternative Recipes SPs are enabled."));
    public ModSetting<bool> MixerCosts { get; } =
        new(true, ModSettingDescriptor.Create("FT Mixer — real costs")
            .SetTooltip("Rebalances four Iron Teeth building prices in Folktails+. Restart required."));

    public AddonSettingsOwner(ISettings settings, ModSettingsOwnerRegistry owners, ModRepository repository)
        : base(settings, owners, repository)
    {
        ChargingBoost.Descriptor.SetEnableCondition(
            () => AddonFeatures.Available(repository, AddonFeatures.All[0]));
        TinyTubeway.Descriptor.SetEnableCondition(
            () => AddonFeatures.Available(repository, AddonFeatures.All[1]));
        ScientificProjects.Descriptor.SetEnableCondition(
            () => AddonFeatures.Available(repository, AddonFeatures.All[2]));
        MixerCosts.Descriptor.SetEnableCondition(
            () => AddonFeatures.Available(repository, AddonFeatures.All[3]));
    }
}

// Native game API: SpecService asks this provider for each EXISTING blueprint.
// Never adds unresolvable paths to collections, or touches the filesystem.
public sealed class AddonBlueprintModifierProvider : IBlueprintModifierProvider
{
    private readonly ModRepository _mods;
    private readonly ISettings _settings;
    private readonly BlueprintFileBundleLoader _bundles;
    private bool? _tinyStationPresent;

    public string ModifierName => "Folktails+ Addons";

    public AddonBlueprintModifierProvider(
        ModRepository mods, ISettings settings, BlueprintFileBundleLoader bundles)
    {
        _mods = mods;
        _settings = settings;
        _bundles = bundles;
    }

    public IEnumerable<string> GetModifiers(string blueprintPath)
    {
        if (_mods.ModIsNotEnabled("FolktailsPlus"))
            yield break;

        foreach (Feature feature in AddonFeatures.All)
        {
            if (!AddonFeatures.Available(_mods, feature) ||
                !_settings.GetBool(AddonFeatures.SettingKey(feature.Key), true))
                continue;

            foreach (string relative in feature.Blueprints)
            {
                if (!string.Equals(Normalize(blueprintPath), Normalize(StripJson(relative)),
                        StringComparison.OrdinalIgnoreCase))
                    continue;

                // The companion mod can be installed but provide a different
                // template path or lack the asset required by a loaded save.
                // Never add a dangling reference to Folktails+'s collection.
                if (feature.Key == "TinyTubeway" && !TinyStationPresent())
                    continue;

                string? json = GetEmbeddedBlueprint(feature.Key, relative);
                if (json != null)
                    yield return json;
            }
        }
    }

    private bool TinyStationPresent()
    {
        if (_tinyStationPresent.HasValue)
            return _tinyStationPresent.Value;

        try
        {
            _tinyStationPresent =
                _bundles.GetBundle(AddonFeatures.TinyStationBlueprint) != null;
        }
        catch (Exception)
        {
            _tinyStationPresent = false;
        }
        return _tinyStationPresent.Value;
    }

    private static string Normalize(string path) => path.Replace('\\', '/').TrimStart('/');
    private static string StripJson(string path) =>
        path.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            ? path.Substring(0, path.Length - ".json".Length) : path;

    private static string? GetEmbeddedBlueprint(string feature, string relative)
    {
        string resource = "AddonPayload/" + feature + "/" + relative + ".template";
        using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource);
        if (stream == null)
            return null;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}

[Context("Game")]
[Context("MainMenu")]
public sealed class AddonConfigurator : Configurator
{
    public override void Configure() =>
        this.BindSingleton<AddonSettingsOwner>()
            .MultiBindSingleton<IBlueprintModifierProvider, AddonBlueprintModifierProvider>();
}
