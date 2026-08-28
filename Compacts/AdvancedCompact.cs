using BaldiPlusRandomZone.PitStop;
using BaldisBasicsPlusAdvanced.Game.Objects.Plates.KitchenStove;
using BaldisBasicsPlusAdvanced.Game.Objects.Spelling;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Reflection;
using PlusStudioLevelLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaldiPlusRandomZone.Compacts
{
    public static class AdvancedCompact
    {
        public static void LoadCompact()
        {
            SceneObject scene = Plugin.assetMan.Get<SceneObject>("ZonePitStop");

            RoomData classroom = scene.levelAsset.rooms.FirstOrDefault(x => x.category == RoomCategory.Class);
            classroom.wallTex = LevelLoaderPlugin.RoomTextureFromAlias("adv_english_wall");
            classroom.florTex = LevelLoaderPlugin.RoomTextureFromAlias("adv_english_floor");
            classroom.ceilTex = LevelLoaderPlugin.RoomTextureFromAlias("adv_english_ceiling");
            classroom.doorMats = Array.Find(UnityEngine.Object.FindObjectsOfType<StandardDoorMats>(), x => x.name == "EnglishDoorSet");

            classroom.roomFunctionContainer = LevelLoaderPlugin.Instance.roomSettings["adv_english_class_timer"].container;

            classroom.basicObjects.Add(new BasicObjectData
            {
                prefab = LevelLoaderPlugin.Instance.basicObjects["adv_symbol_machine"].transform,
                position = new(125f, 0f, 155f),
                rotation = Direction.North.ToRotation()
            });

            CustomPitStopStove pitStove = new GameObject("Structure_CustomPitStove").gameObject.AddComponent<CustomPitStopStove>();
            pitStove.gameObject.ConvertToPrefab(true);

            PosterObject cookingPoster = ObjectCreators.CreatePosterObject(AssetLoader.TextureFromMod(Plugin.instance, "Compacts", "CookingPoster.png"), []);

            scene.levelAsset.structures.Add(new StructureBuilderData { prefab = pitStove, data = [] });
            scene.levelAsset.posters.Add(new PosterData() { position = new(5, 7), direction = Direction.West, poster = cookingPoster });
        }
    }

    public class CustomPitStopStove : StructureBuilder
    {
        public override void Load(List<StructureData> data)
        {
            base.Load(data);

            StoreRoomFunction storeFunc = FindObjectOfType<StoreRoomFunction>();

            JohnnyKitchenStove stove = Instantiate(Resources.FindObjectsOfTypeAll<JohnnyKitchenStove>().FirstOrDefault(x => x.name == "johnny_kitchen_stove"), ec.cells[5, 7].room.objectObject.transform);
            stove.transform.position = ec.cells[5, 7].FloorWorldPosition;
            stove.Assign(storeFunc);

            GameButton button = (GameButton)GameButton.Build(Plugin.assetPlusMan.Get<GameButtonBase>("GameButton_wBackFace"), ec, new IntVector2(5, 7), Direction.West);
            button.transform.parent = ec.cells[5, 7].room.objectObject.transform;
            button.SetUp(stove);
        }
    }

    [HarmonyPatch(typeof(SymbolMachine), "Start")]
    [ConditionalPatchMod("mrsasha5.baldi.basics.plus.advanced")]
    internal static class AdvancedSybolMachinePatch
    {
        [HarmonyPostfix]
        public static void Patch(SymbolMachine __instance)
        {
            if (Singleton<BaseGameManager>.Instance.InPitstop())
                __instance.ReflectionSetVariable("isPitFloor", true);
        }
    }
}
