using BepInEx.Bootstrap;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace BaldiPlusRandomZone.Compacts
{
    public static class LiveStudentReactionCompact
    {
        private static float customStateTimer;
        private static string previousState, currentCustomState;
        private static bool customStateActive;
        private static readonly Dictionary<string, Sprite[]> customStates = new();

        public static void Load()
        {
            AddState("Happy", AssetLoader.SpritesFromSpritesheet(4, 1, 1f, Vector2.one / 2f, AssetLoader.TextureFromMod(Plugin.instance, "Compacts", "Student_Drawing_Happy_Sheet.png")));
            AddState("Mid", AssetLoader.SpritesFromSpritesheet(4, 1, 1f, Vector2.one / 2f, AssetLoader.TextureFromMod(Plugin.instance, "Compacts", "Student_Drawing_Mid_Sheet.png")));
            AddState("Scary", AssetLoader.SpritesFromSpritesheet(4, 1, 1f, Vector2.one / 2f, AssetLoader.TextureFromMod(Plugin.instance, "Compacts", "Student_Drawing_Scared_Sheet.png")));
        }

        public static void AddState(string state, Sprite[] sprites)
        {
            customStates[state] = sprites;
        }

        public static void SetCustomState(string state, float duration)
        {
            if (!Chainloader.PluginInfos.ContainsKey("ganaisthere.plus.livestudentreaction") ||
                !customStates.ContainsKey(state))
                return;

            Type type = AccessTools.TypeByName("LiveStudentReaction.HudManagerUpdatePatch");
            if (type == null) return;

            FieldInfo stateField = AccessTools.Field(type, "StudentState");
            if (stateField == null) return;

            if (!customStateActive)
                previousState = (string)stateField.GetValue(null);

            currentCustomState = state;
            customStateTimer = duration;
            customStateActive = true;
            stateField.SetValue(null, state);

            AccessTools.Field(type, "StudentStateOld")?.SetValue(null, previousState);
            AccessTools.Field(type, "StudentStateKeepTimer")?.SetValue(null, duration);
            AccessTools.Field(type, "StaticTimer")?.SetValue(null, 0.2f);
        }

        public static void Update()
        {
            if (!customStateActive) return;

            customStateTimer -= Time.deltaTime;
            if (customStateTimer > 0f) return;

            customStateTimer = 0f;
            RestoreState();
            customStateActive = false;
            currentCustomState = null;
        }

        private static void RestoreState()
        {
            Type type = AccessTools.TypeByName("LiveStudentReaction.HudManagerUpdatePatch");
            if (type == null) return;

            AccessTools.Field(type, "StudentStateOld")?.SetValue(null, currentCustomState);
            AccessTools.Field(type, "StudentState")?.SetValue(null, previousState);
            AccessTools.Field(type, "StaticTimer")?.SetValue(null, 0.2f);
            AccessTools.Field(type, "StudentStateKeepTimer")?.SetValue(null, 0f);
        }

        public static bool IsActive() => customStateActive;

        public static string GetCurrentState() => currentCustomState;

        public static Sprite[] GetCurrentSprites()
        {
            return customStateActive && customStates.TryGetValue(currentCustomState, out Sprite[] sprites)
                ? sprites : null;
        }
    }

    [ConditionalPatchMod("ganaisthere.plus.livestudentreaction")]
    [HarmonyPatch]
    public static class LiveStudentReactionPatches
    {
        [HarmonyTargetMethod]
        public static MethodBase TargetMethod()
        {
            Type type = AccessTools.TypeByName("LiveStudentReaction.HudManagerUpdatePatch");
            return type == null ? null : AccessTools.Method(type, "ChangeImage");
        }

        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!LiveStudentReactionCompact.IsActive()) return;

            Sprite[] sprites = LiveStudentReactionCompact.GetCurrentSprites();
            if (sprites == null || sprites.Length == 0) return;

            Type type = AccessTools.TypeByName("LiveStudentReaction.HudManagerUpdatePatch");
            if (type == null) return;

            FieldInfo imageField = AccessTools.Field(type, "StudentImage");
            if (imageField == null) return;

            int image = (int)imageField.GetValue(null) % sprites.Length;

            Type awakeType = AccessTools.TypeByName("LiveStudentReaction.HudManagerAwakePatch");
            if (awakeType == null) return;

            FieldInfo studentField = AccessTools.Field(awakeType, "Student");
            if (studentField == null) return;

            Image student = studentField.GetValue(null) as Image;
            if (student == null) return;

            student.sprite = sprites[image];
        }
    }

    [HarmonyPatch(typeof(HudManager), "Update")]
    [ConditionalPatchMod("ganaisthere.plus.livestudentreaction")]
    public static class LiveStudentReactionTimerPatch
    {
        [HarmonyPostfix]
        public static void Postfix() => LiveStudentReactionCompact.Update();
    }
}
