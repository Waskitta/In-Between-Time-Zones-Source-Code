using BaldiPlusRandomZone.EndlessSupport;
using BaldiPlusRandomZone.RoomFunctions;
using BaldiPlusRandomZone.ZoneRules;
using HarmonyLib;
using MTM101BaldAPI.Reflection;
using System;

namespace BaldiPlusRandomZone.Patches
{
    [HarmonyPatch(typeof(RoomFunctionContainer))]
    internal static class RoomFunctionContainerPatches
    {
        [HarmonyPatch(nameof(RoomFunctionContainer.Initialize)), HarmonyPrefix]
        public static void OnInitialize(RoomFunctionContainer __instance, RoomController room)
        {
            if (!__instance.gameObject.GetComponent<DummyRoomFunction>())
                __instance.AddFunction(__instance.gameObject.AddComponent<DummyRoomFunction>());

            __instance.AddFunction(__instance.gameObject.AddComponent<WoodRoomFunction>());
        }

        [HarmonyPatch(nameof(RoomFunctionContainer.AfterAllRoomsPlaced)), HarmonyPrefix]
        public static void AfterAllRoomsPlaced(RoomFunctionContainer __instance, LevelBuilder builder, Random rng)
        {
            if (Singleton<EndlessZoneManager>.Instance == null) return;

            foreach (ZoneRule rule in Singleton<EndlessZoneManager>.Instance.zoneRules)
                rule.AfterAllRoomsPlaced((RoomController)__instance.ReflectionGetVariable("room"), builder, rng);
        }
    }

    public class DummyRoomFunction : RoomFunction
    {
        public override void AfterAllRoomsPlaced(LevelBuilder builder, Random rng)
        {
            base.AfterAllRoomsPlaced(builder, rng);
        }
    }

}
