using BaldiPlusRandomZone.EndlessSupport;
using HarmonyLib;
using System.Collections;
using System.Globalization;
using UnityEngine;

namespace BaldiPlusRandomZone.Patches
{
    [HarmonyPatch]
    internal static class ElevatorScreenPatches
    {
        public static float YtpMultiplier()
        {
            EndlessZoneManager manager = Singleton<EndlessZoneManager>.Instance;

            if (manager == null)
                return 3 - Singleton<CoreGameManager>.Instance.Attempts;

            if (manager.zoneRules == null || manager.zoneRules.Count == 0)
                return 1f;

            return manager.zoneRules[0].powerBonus;
        }


        [HarmonyPatch(typeof(ElevatorScreen), "Results")]
        [HarmonyPostfix]
        private static void ResultsPostfix(ElevatorScreen __instance, int stickerBonus)
        {
            if (Singleton<EndlessZoneManager>.Instance != null)
                __instance.StartCoroutine(ForceMultiplierText(__instance, stickerBonus));
        }

        private static IEnumerator ForceMultiplierText(ElevatorScreen screenInstance, int stickerBonus)
        {
            BigScreen screen = (BigScreen)AccessTools.Field(typeof(ElevatorScreen), "bigScreen").GetValue(screenInstance);

            while (!screen.resultsText.activeSelf)
                yield return null;

            while (screen.resultsText.activeSelf)
            {
                float multiplier = YtpMultiplier();
                int points = Singleton<CoreGameManager>.Instance.GetPointsThisLevel(0);
                int total = Mathf.RoundToInt(points * multiplier + stickerBonus);

                screen.multiplier.text = multiplier.ToString("0.##", CultureInfo.InvariantCulture) + "x";;
                screen.total.text = total.ToString();

                yield return null;
            }
        }
    }
}