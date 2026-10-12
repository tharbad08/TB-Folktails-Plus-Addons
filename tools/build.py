#!/usr/bin/env python3
"""Package selected addons without assuming optional mods are enabled."""
import argparse, zipfile
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
FEATURES={
"ChargingBoost":{"FolktailsPlus"},
"TinyTubeway":{"FolktailsPlus","BobingaboutTinyTubewayStation"},
"ScientificProjects":{"FolktailsPlus","ScientificProjects","AlternativeRecipesSPs"}
}
def build(out,enabled,disabled=(),dll=None):
    enabled=set(enabled)
    if "FolktailsPlus" not in enabled:
        raise ValueError("FolktailsPlus must be enabled")
    base=ROOT/"Mod"
    out=Path(out)
    out.parent.mkdir(parents=True,exist_ok=True)
    with zipfile.ZipFile(out,"w",zipfile.ZIP_DEFLATED) as zip:
        for path in sorted(base.rglob("*")):
            if path.is_file():
                zip.write(path,"FolktailsPlusAddons/"+path.relative_to(base).as_posix())
        for feature,required in FEATURES.items():
            if required<=enabled and feature not in disabled:
                for path in (base/"Payload"/feature).rglob("*.template"):
                    rel=path.relative_to(base/"Payload"/feature).as_posix()[:-9]
                    zip.writestr("FolktailsPlusAddons/version-1.1/"+rel,path.read_bytes())
        if dll: zip.write(dll,"FolktailsPlusAddons/Scripts/"+Path(dll).name)
    return out
if __name__=="__main__":
    ap=argparse.ArgumentParser()
    ap.add_argument("--enabled-mod",action="append",default=[])
    ap.add_argument("--disable",choices=list(FEATURES),action="append",default=[])
    ap.add_argument("--dll",type=Path)
    ap.add_argument("--out",type=Path,default=Path("FolktailsPlusAddons.zip"))
    args=ap.parse_args()
    print(build(args.out,args.enabled_mod,args.disable,args.dll))
