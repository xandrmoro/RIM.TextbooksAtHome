using HarmonyLib;
using LifeLessons;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using Verse;

namespace TextbooksAtHome
{
    [HarmonyPatch]
    public class LifeLessons_TextbookGenerator_ImpliedThingDefs
    {
        private static MethodBase target;
        private static Assembly assembly;

        public static bool Prepare()
        {
            var llMod = LoadedModManager.RunningMods.FirstOrDefault(m => m.PackageId.EqualsIgnoreCase("GhostData.lifelessons"));

            if (llMod == null)
                return false;

            try
            {
                assembly = llMod.assemblies.loadedAssemblies.FirstOrDefault(a => a.GetName().Name == "LifeLessons");
                var type = assembly.GetType("LifeLessons.TextbookGenerator");

                target = type.GetMethods(BindingFlags.Static | BindingFlags.Public).First(m => m.Name == "ImpliedThingDefs");
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());

                return false;
            }

            return true;
        }

        public static MethodBase TargetMethod()
        {
            return target;
        }

        public static List<ThingDef> Textbooks = new List<ThingDef>();

        public static IEnumerable<ThingDef> Postfix(IEnumerable<ThingDef> values)
        {
            var baseDef = ThingDef.Named("LLTextbook_Recipe");

            foreach (var value in values)
            {
                value.costList = baseDef.costList;
                value.recipeMaker = baseDef.recipeMaker;

                var proficiency = value.GetCompProperties<CompProperties_ProficiencyLearningModifier>().proficiency;

                value.statBases.Add(new StatModifier
                {
                    stat = StatDefOf.WorkToMake,
                    value = (proficiency.costPractical + proficiency.costTheoretical) * 80
                });

                if (value.modExtensions == null)
                    value.modExtensions = new List<DefModExtension>();

                value.modExtensions.Add(new BillProficiencyExtension
                {
                    requirements = new List<ProficiencyDef>
                    {
                        proficiency
                    }
                });

                Textbooks.Add(value);

                yield return value;
            }
        }
    }
}
