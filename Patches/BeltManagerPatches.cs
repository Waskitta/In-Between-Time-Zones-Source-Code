using HarmonyLib;
using UnityEngine;

namespace BaldiPlusRandomZone.Patches
{
    [HarmonyPatch(typeof(BeltManager))]
    internal static class BeltManagerPatches
    {
        [HarmonyPatch("OnTriggerEnter"), HarmonyPrefix]
        public static bool Prefix(BeltManager __instance, Collider other)
        {
            NPC npc = other.GetComponent<NPC>();
            if (npc != null)
            {
                if (npc.Navigator.Speed > __instance.Speed)
                    return false;
            }

            return true;
        }
    }
}
