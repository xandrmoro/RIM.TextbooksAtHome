using HarmonyLib;
using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace TextbooksAtHome
{
    [HarmonyPatch(typeof(DefGenerator), nameof(DefGenerator.GenerateImpliedDefs_PreResolve))]
    public class DefGenerator_GenerateImpliedDefs_PreResolve
    {
        public static bool Prepare()
        {
            return LoadedModManager.RunningMods.Any(m => m.PackageId.EqualsIgnoreCase("GhostData.lifelessons"));
        }

        public static void Postfix()
        {
            DefDatabase<RecipeDef>.Remove(DefDatabase<RecipeDef>.GetNamed("Make_LLTextbook_Recipe"));

            foreach (var textbook in LifeLessons_TextbookGenerator_ImpliedThingDefs.Textbooks.OrderBy(t => t.label))
            {
                var recipe = RecipeDefGenerator.CreateRecipeDefFromMaker(textbook);
                recipe.uiIconThing = textbook;

                DefGenerator.AddImpliedDef(recipe);
            }
        }
    }
}
