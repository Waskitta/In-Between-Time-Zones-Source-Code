using BaldiPlusRandomZone.EndlessSupport;
using BaldiPlusRandomZone.PitStop;
using HarmonyLib;

namespace BaldiPlusRandomZone.Patches
{
    [HarmonyPatch]
    internal static class StorageLockerPatches
    {
        [HarmonyPatch(typeof(StorageLocker), "Start"), HarmonyPostfix]
        public static void Initialize(StorageLocker __instance)
        {
            if (Singleton<EndlessZoneManager>.Instance == null) return;
            if (__instance.GetComponent<UpgradableGreenLocker>()) return;

            __instance.gameObject.AddComponent<UpgradableGreenLocker>().Initialize(__instance, Singleton<BaseGameManager>.Instance is not ZonePitStopManager);
        }
    }
}
