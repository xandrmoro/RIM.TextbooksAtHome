using HarmonyLib;
using Verse;

namespace TextbooksAtHome
{
    public class TextbooksAtHome : Mod
    {
        public TextbooksAtHome(ModContentPack contentPack) : base(contentPack)
        {
            new Harmony(Content.PackageIdPlayerFacing).PatchAll();
        }
    }
}
