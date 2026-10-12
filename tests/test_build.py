import sys,tempfile,zipfile,json,unittest
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parents[1]/"tools"))
from build import build
class Tests(unittest.TestCase):
    def case(self,mods):
        with tempfile.TemporaryDirectory() as tmp:
            with zipfile.ZipFile(build(Path(tmp)/"addon.zip",mods)) as archive:
                names=archive.namelist()
                for n in names:
                    if n.endswith(".blueprint.json") or n.endswith("manifest.json"):
                        json.loads(archive.read(n))
                return names
    def test_core_only(self):
        names=self.case(["FolktailsPlus"])
        self.assertTrue(any("Need.Bot.ChargingBoost" in n for n in names))
        self.assertFalse(any("/version-1.1/TemplateCollections/" in n for n in names))
    def test_no_scientific_without_dependencies(self):
        names=self.case(["FolktailsPlus","ScientificProjects"])
        self.assertFalse(any("/version-1.1/Blueprints/" in n for n in names))
    def test_all(self):
        names=self.case(["FolktailsPlus","BobingaboutTinyTubewayStation","ScientificProjects","AlternativeRecipesSPs"])
        self.assertEqual(sum(n.startswith("FolktailsPlusAddons/version-1.1/") for n in names),10)
if __name__=="__main__":unittest.main()
