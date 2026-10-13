using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bindito.Core;
using ModSettings.CommonUI;
using ModSettings.Core;
using ModSettings.CoreUI;
using Timberborn.ModManagerScene;
using Timberborn.Modding;
using Timberborn.SettingsSystem;
using TimberUi.CommonUi;
using UnityEngine.UIElements;

namespace FolktailsPlusAddons;

// Mirrors TB Optimizer's startup switches + native Mod Settings factory.
// IMPORTANT: optional features are materialized only if the installed mod
// repository says that ALL their dependencies are enabled.
internal sealed class Feature
{
    public readonly string Key;
    public readonly string Label;
    public readonly string Description;
    public readonly string[] Dependencies;
    public readonly string[] Files;
    public Feature(string key, string label, string description, string[] dependencies, params string[] files)
    {
        Key = key; Label = label; Description = description;
        Dependencies = dependencies; Files = files;
    }
}

internal static class Features
{
    private const string RootName = "FolktailsPlusAddons";
    private static readonly string[] Core = { "FolktailsPlus" };
    internal static readonly Feature[] All = {
        new Feature("ChargingBoost", "Charging station boost",
            "Adds charging boost to Folktails+ bots.\nRequires Folktails+.",
            Core,
            "Needs/Need.Bot.ChargingBoost.blueprint.json",
            "NeedCollections/NeedCollection.FolktailsPlusAddons.ChargingBoost.blueprint.json",
            "Factions/Faction.Folktails.blueprint.json",
            "Buildings/Science/ChargingStation/ChargingStation.IronTeeth.blueprint.json"),
        new Feature("TinyTubeway", "Tiny Tubeway Station",
            "Adds the smaller station to Folktails.\nRequires Tiny Tubeway Station.",
            new[] { "FolktailsPlus", "BobingaboutTinyTubewayStation" },
            "TemplateCollections/TemplateCollection.Buildings.FolktailsPlus.blueprint.json"),
        new Feature("ScientificProjects", "Scientific projects",
            "Unlocks five Iron Teeth projects for Folktails.\nRequires both science mods.",
            new[] { "FolktailsPlus", "ScientificProjects", "AlternativeRecipesSPs" },
            "Blueprints/ScientificProjects/Badwaters/CrudeLogPressExtract.blueprint.json",
            "Blueprints/ScientificProjects/Factions/ItSmelterUpgrade.ScientificProjectSpec.blueprint.json",
            "Blueprints/ScientificProjects/Foods/PrecisionOilYield.blueprint.json",
            "Blueprints/ScientificProjects/Foods/RapidCanolaCrush.blueprint.json",
            "Blueprints/ScientificProjects/Woods/SteelBoundPlanks.blueprint.json")
    };

    private static readonly Dictionary<string, bool> Switches = new(StringComparer.Ordinal);
    private static HashSet<string>? _activeMods;
    private static string _root = "";
    private static readonly object Lock = new();
    public static bool Enabled(Feature f) => !Switches.TryGetValue(f.Key, out var b) || b;
    public static bool DependenciesFound(Feature f) =>
        _activeMods is not null && f.Dependencies.All(_activeMods.Contains);
    public static bool HasSnapshot => _activeMods is not null;

    public static void Startup(string modPath)
    {
        lock (Lock)
        {
            _root = Path.GetFileName(modPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                    .Equals("version-1.1", StringComparison.OrdinalIgnoreCase)
                ? Directory.GetParent(modPath)!.FullName : modPath;
            _activeMods = null;
            Switches.Clear();
            try
            {
                var file = Path.Combine(_root, "folktails-addons-options.txt");
                if (File.Exists(file))
                    foreach (var line in File.ReadAllLines(file))
                    {
                        var parts = line.Split('=');
                        if (parts.Length == 2 && (parts[1] == "0" || parts[1] == "1"))
                            Switches[parts[0]] = parts[1] == "1";
                    }
            }
            catch (Exception e) { Warn("Could not read settings: " + e.Message); }
        }
    }

    public static void Capture(ModRepository repository)
    {
        lock (Lock)
        {
            var found = new HashSet<string>(StringComparer.Ordinal);
            try
            {
                // Use the actual enabled ModRepository, NOT filesystem presence,
                // loaded assemblies, or unvalidated guesswork about mod names.
                if (repository.GetType().GetProperty("Mods")?.GetValue(repository) is IEnumerable mods)
                    foreach (var mod in mods)
                    {
                        if (mod is null) continue;
                        var t = mod.GetType();
                        if (t.GetProperty("IsEnabled")?.GetValue(mod) is not bool active || !active) continue;
                        var manifest = t.GetProperty("Manifest")?.GetValue(mod);
                        if (manifest?.GetType().GetProperty("Id")?.GetValue(manifest) is string id)
                            found.Add(id);
                    }
                // Empty snapshot is a safe failure: dependent features are removed.
            }
            catch (Exception e) { Warn("Cannot inspect enabled mods: " + e.Message); }
            _activeMods = found;
            Synchronize();
        }
    }

    public static void Set(string key, bool value)
    {
        lock (Lock)
        {
            Switches[key] = value;
            try
            {
                if (_root.Length > 0)
                    File.WriteAllLines(Path.Combine(_root, "folktails-addons-options.txt"),
                        All.Select(f => f.Key + "=" + (Enabled(f) ? "1" : "0")));
            }
            catch (Exception e) { Warn("Cannot save settings: " + e.Message); }
            Synchronize();
        }
    }

    private static void Synchronize()
    {
        // This executes during MainMenu settings owner creation, so patches
        // exist before the NEXT game load. Options are restart-required.
        if (_root.Length == 0 || _activeMods is null) return;
        var version = Path.Combine(_root, "version-1.1");
        foreach (var f in All)
        {
            var shouldEnable = Enabled(f) && DependenciesFound(f);
            foreach (var relative in f.Files)
            {
                var dest = Path.Combine(version, relative);
                var source = Path.Combine(_root, "Payload", f.Key, relative + ".template");
                try
                {
                    if (!shouldEnable)
                    {
                        // Only remove files we manage, never dependency files.
                        if (File.Exists(dest)) File.Delete(dest);
                        continue;
                    }
                    if (!File.Exists(source))
                    {
                        Warn("Feature payload missing: " + source);
                        continue;
                    }
                    var bytes = File.ReadAllBytes(source);
                    Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
                    if (!File.Exists(dest) || !File.ReadAllBytes(dest).SequenceEqual(bytes))
                        File.WriteAllBytes(dest, bytes);
                }
                catch (Exception e) { Warn(f.Key + ": " + e.Message); }
            }
        }
    }
    private static void Warn(string message) => System.Diagnostics.Trace.TraceWarning("[Folktails+ Addons] " + message);
}

public sealed class ModStarter : IModStarter
{
    public void StartMod(IModEnvironment environment) => Features.Startup(environment.ModPath);
}

[Context("MainMenu")]
public sealed class MainMenuConfig : Configurator
{
    public override void Configure() => this.BindSingleton<AddonSettingsOwner>()
        .MultiBindSingleton<IModSettingElementFactory, AddonSettingsElementFactory>();
}

[Context("Game")]
public sealed class GameConfig : Configurator
{
    public override void Configure() => this.BindSingleton<AddonSettingsOwner>()
        .MultiBindSingleton<IModSettingElementFactory, AddonSettingsElementFactory>();
}

public sealed class AddonSettingsOwner : ModSettingsOwner
{
    public override string ModId => "tharbad08.FolktailsPlusAddons";
    public override ModSettingsContext ChangeableOn => ModSettingsContext.All;
    public AddonSettingsUiSetting Features { get; } = new();
    public AddonSettingsOwner(ISettings settings, ModSettingsOwnerRegistry registry, ModRepository repo)
        : base(settings, registry, repo) => FolktailsPlusAddons.Features.Capture(repo);
}

public sealed class AddonSettingsUiSetting : NonPersistentSetting
{
    public AddonSettingsUiSetting() : base(ModSettingDescriptor.Create("Folktails+ Addons")) { }
    public override void Reset() { }
}

public sealed class AddonSettingsPanel : VisualElement
{
    public AddonSettingsPanel()
    {
        this.SetPadding(8);
        this.AddGameLabel("Folktails+ Addons", bold: true).SetMarginBottom(7);
        this.AddGameLabel("Changes take effect after restarting Timberborn.").SetMarginBottom(10);
        foreach (var f in FolktailsPlusAddons.Features.All)
        {
            var group = this.AddChild().SetMarginBottom(11);
            var header = group.AddRow().AlignItems(Align.Center);
            header.AddGameLabel(f.Label).SetMarginRight(8);
            var selected = f.Key;
            var checkbox = header.AddToggle("", onValueChanged: value => FolktailsPlusAddons.Features.Set(selected, value));
            checkbox.SetValueWithoutNotify(FolktailsPlusAddons.Features.Enabled(f));
            var note = f.Description;
            if (!FolktailsPlusAddons.Features.DependenciesFound(f))
                note += "\nNot active: required mod missing or disabled.";
            group.AddGameLabel(note).SetMarginBottom(2);
        }
    }
}

public sealed class AddonSettingsElementFactory : IModSettingElementFactory
{
    public int Priority => 0;
    public bool TryCreateElement(ModSetting setting, out IModSettingElement? element)
    {
        if (setting is AddonSettingsUiSetting)
        {
            element = new ModSettingElement(new AddonSettingsPanel(), setting);
            return true;
        }
        element = null;
        return false;
    }
}
