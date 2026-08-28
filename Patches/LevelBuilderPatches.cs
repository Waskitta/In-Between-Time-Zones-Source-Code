using BaldiPlusRandomZone.EndlessSupport;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace BaldiPlusRandomZone.Patches
{
    [HarmonyPatch(typeof(LevelBuilder))]
    internal static class LevelBuilderPatches
    {
        private static int previousNpcCount;

        [HarmonyPrefix]
        [HarmonyPatch("AddNpcsFromPreviousLevels")]
        private static void AddNpcsFromPreviousLevels_Prefix(LevelBuilder __instance)
        {
            if (EndlessZoneManager.Instance == null || !EndlessZoneManager.Instance.zoneRules.Any(x => x.category == ZoneRules.ZoneRuleCategory.ClonedNPCs))
                return;

            previousNpcCount = 0;

            foreach (SceneObject previousLevel in __instance.scene.previousLevels)
            {
                previousNpcCount += previousLevel.forcedNpcs.Length;
                previousNpcCount += previousLevel.additionalNPCs;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("AddNpcsFromPreviousLevels")]
        private static void AddNpcsFromPreviousLevels_Postfix(LevelBuilder __instance)
        {
            if (EndlessZoneManager.Instance == null || !EndlessZoneManager.Instance.zoneRules.Any(x => x.category == ZoneRules.ZoneRuleCategory.ClonedNPCs))
                return;

            int currentLevelStart = previousNpcCount;

            if (currentLevelStart >= __instance.Ec.npcsToSpawn.Count)
                return;

            List<NPC> clonedNpcs = new List<NPC>();

            for (int i = currentLevelStart; i < __instance.Ec.npcsToSpawn.Count; i++)
            {
                clonedNpcs.Add(__instance.Ec.npcsToSpawn[i]);
            }

            __instance.Ec.npcsToSpawn.AddRange(clonedNpcs);
        }
    }
}