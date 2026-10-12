using System;
using System.Collections.Generic;
using System.IO;
namespace Tharbad08.FolktailsPlusAddons;
internal static class BlueprintMaterializer
{
    private static readonly Dictionary<string,string[]> Files = new() {
        ["ChargingBoost"] = new[] {
            "Needs/Need.Bot.ChargingBoost.blueprint.json",
            "NeedCollections/NeedCollection.FolktailsPlusAddons.ChargingBoost.blueprint.json",
            "Factions/Faction.Folktails.blueprint.json",
            "Buildings/Science/ChargingStation/ChargingStation.IronTeeth.blueprint.json"
        },
        ["TinyTubeway"] = new[] {
            "TemplateCollections/TemplateCollection.Buildings.FolktailsPlus.blueprint.json"
        },
        ["ScientificProjects"] = new[] {
            "Blueprints/ScientificProjects/Foods/RapidCanolaCrush.blueprint.json",
            "Blueprints/ScientificProjects/Foods/PrecisionOilYield.blueprint.json",
            "Blueprints/ScientificProjects/Badwaters/CrudeLogPressExtract.blueprint.json",
            "Blueprints/ScientificProjects/Woods/SteelBoundPlanks.blueprint.json",
            "Blueprints/ScientificProjects/Factions/ItSmelterUpgrade.ScientificProjectSpec.blueprint.json"
        }
    };
    public static void Sync(string root,string feature,bool active)
    {
        foreach (var relative in Files[feature])
        {
            var target = Path.Combine(root,"version-1.1",relative);
            if (!active) { if (File.Exists(target)) File.Delete(target); continue; }
            var source = Path.Combine(root,"Payload",feature,relative+".template");
            if (!File.Exists(source)) throw new FileNotFoundException("Missing addon payload",source);
            var data = File.ReadAllText(source);
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            if (!File.Exists(target) || File.ReadAllText(target)!=data)
                File.WriteAllText(target,data);
        }
    }
}
