using HarmonyLib;

namespace BaldiPlusRandomZone.CustomCharacters.Patches
{
    [HarmonyPatch]
    internal static class MainNPCPatches
    {
        [HarmonyPatch(typeof(NPC), nameof(NPC.Initialize)), HarmonyPrefix]
        public static void OnEnterEntityTrigger(NPC __instance)
        {
            if (__instance.TryGetComponent(out NpcReskinBase reskin))
                reskin.OnNpcInitialize();
        }

        [HarmonyPatch(typeof(NPC), nameof(NPC.EntityTriggerEnter)), HarmonyPrefix]
        public static void OnEnterEntityTrigger(NPC __instance, Entity otherEntity, bool validCollision)
        {
            if (__instance.TryGetComponent(out NpcReskinBase reskin))
                reskin.OnEntityEnter(otherEntity, validCollision);
        }

        [HarmonyPatch(typeof(NPC), nameof(NPC.EntityTriggerStay)), HarmonyPrefix]
        public static void OnStayEntityTrigger(NPC __instance, Entity otherEntity, bool validCollision)
        {
            if (__instance.TryGetComponent(out NpcReskinBase reskin))
                reskin.OnEntityStay(otherEntity, validCollision);
        }

        [HarmonyPatch(typeof(NPC), nameof(NPC.EntityTriggerExit)), HarmonyPrefix]
        public static void OnExitEntityTrigger(NPC __instance, Entity otherEntity, bool validCollision)
        {
            if (__instance.TryGetComponent(out NpcReskinBase reskin))
                reskin.OnEntityExit(otherEntity, validCollision);
        }
    }
}
