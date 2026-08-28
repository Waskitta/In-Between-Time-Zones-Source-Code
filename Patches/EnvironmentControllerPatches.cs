using HarmonyLib;

namespace BaldiPlusRandomZone.Patches
{
    [HarmonyPatch(typeof(EnvironmentController))]
    internal static class EnvironmentControllerPatches
    {
        [HarmonyPatch(nameof(EnvironmentController.BuildPoster), new[] { typeof(PosterObject), typeof(Cell), typeof(Direction), typeof(bool) })]
        [HarmonyPrefix]
        public static bool Prefix(PosterObject poster, Cell tile, Direction dir, bool allowMultiPoster, ref bool __result)
        {
            if (poster == null)
            {
                __result = false;
                return false;
            }

            if (poster.multiPosterArray == null)
            {
                __result = false;
                return false;
            }

            if (tile == null)
            {
                __result = false;
                return false;
            }

            return true;
        }
    }
    }
