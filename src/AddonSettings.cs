using System;
using System.Linq;
using Bindito.Core;
using ModSettings.Common;
using ModSettings.Core;
using Timberborn.Modding;
namespace Tharbad08.FolktailsPlusAddons;

[Context("MainMenu")]
[Context("Game")]
public sealed class AddonSettingsConfigurator : IConfigurator
{
    public void Configure(IContainerDefinition containerDefinition) =>
        containerDefinition.Bind<AddonSettings>().AsSingleton();
}
public sealed class AddonSettings : ModSettingsOwner
{
    private const string AddonId = "tharbad08.FolktailsPlusAddons";
    private readonly ModRepository repository;
    private readonly ModSetting<bool> charging = new(true, ModSettingDescriptor.Create("Charging station boost")
        .SetTooltip("Boosts Folktails+ bots after a charging-station visit. Restart required."));
    private readonly ModSetting<bool> tiny = new(true, ModSettingDescriptor.Create("Tiny Tubeway Station")
        .SetTooltip("Requires the Tiny Tubeway Station mod. Restart required."));
    private readonly ModSetting<bool> scientific = new(true, ModSettingDescriptor.Create("Scientific projects")
        .SetTooltip("Requires Scientific Projects and Alternative Recipes Scientific Projects. Restart required."));
    public override string ModId => AddonId;
    public override ModSettingsContext ChangeableOn => ModSettingsContext.All;
    public AddonSettings(ISettings settings, ModSettingsOwnerRegistry owners, ModRepository repository)
        : base(settings, owners, repository) { this.repository = repository; }
    private bool Enabled(string id) => repository.Mods.Any(x => x.IsEnabled &&
        string.Equals(x.Manifest.Id, id, StringComparison.Ordinal));
    public override void OnBeforeLoad()
    {
        base.OnBeforeLoad();
        tiny.Descriptor.SetEnableCondition(() => Enabled("BobingaboutTinyTubewayStation"));
        scientific.Descriptor.SetEnableCondition(() => Enabled("ScientificProjects") && Enabled("AlternativeRecipesSPs"));
        AddCustomModSetting(charging, nameof(charging));
        AddCustomModSetting(tiny, nameof(tiny));
        AddCustomModSetting(scientific, nameof(scientific));
    }
    public override void OnAfterLoad()
    {
        base.OnAfterLoad();
        ModSettingChanged += (_, _) => Sync();
        Sync();
    }
    private void Sync()
    {
        var self = repository.Mods.FirstOrDefault(x => x.Manifest.Id == AddonId);
        if (self == null) return;
        var path = self.ModDirectory.Directory.FullName;
        foreach (var feature in new[] { "ChargingBoost", "TinyTubeway", "ScientificProjects" })
        {
            var enabled = feature switch {
                "ChargingBoost" => charging.Value,
                "TinyTubeway" => tiny.Value && Enabled("BobingaboutTinyTubewayStation"),
                _ => scientific.Value && Enabled("ScientificProjects") && Enabled("AlternativeRecipesSPs")
            };
            try { BlueprintMaterializer.Sync(path, feature, enabled); }
            catch (Exception e) { UnityEngine.Debug.LogError($"Folktails+ Addons: {feature}: {e}"); }
        }
    }
}
