using HarmonyLib;

namespace BaldiPlusRandomZone.CustomCharacters.Patches
{
    [HarmonyPatch]
    internal static class PrincipalPatches
    {
        [HarmonyPatch(typeof(Principal), nameof(Principal.SendToDetention)), HarmonyPostfix]
        public static void OnSendDetention(Principal __instance, bool validCollision)
        {
            if (__instance.TryGetComponent(out PrincipalReskin reskin))
                reskin.SendToDetention(validCollision);
        }
    }
}
