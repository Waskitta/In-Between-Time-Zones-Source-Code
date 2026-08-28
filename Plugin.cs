using BaldiPlusRandomZone.Compacts;
using BaldiPlusRandomZone.CustomCharacters;
using BaldiPlusRandomZone.EndlessSupport;
using BaldiPlusRandomZone.Extensions;
using BaldiPlusRandomZone.Menu;
using BaldiPlusRandomZone.PitStop;
using BaldiPlusRandomZone.ZoneRules;
using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using ModdedModesAPI.ModesAPI;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Reflection;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.SaveSystem;
using MTM101BaldAPI.UI;
using PlusStudioLevelFormat;
using PlusStudioLevelLoader;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace BaldiPlusRandomZone
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi", MTM101BaldiDevAPI.VersionNumber)]
    [BepInDependency("mtm101.rulerp.baldiplus.levelstudioloader", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("pixelguy.pixelmodding.baldiplus.moddedmodesapi", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("ganaisthere.plus.livestudentreaction", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("mtm101.rulerp.baldiplus.levelstudio", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("mrsasha5.baldi.basics.plus.advanced", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("wazkitta.plusmod.communitystickers", BepInDependency.DependencyFlags.SoftDependency)]

    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance { get; private set; }
        public static AssetManager assetMan = new AssetManager();
        public static AssetManager assetPlusMan = new AssetManager();

        private void Awake()
        {
            Harmony harmony = new(PluginInfo.PLUGIN_GUID);
            harmony.PatchAllConditionals();
            instance = this;

            LoadingEvents.RegisterOnAssetsLoaded(Info, PreLoad(), LoadingEventOrder.Pre);
            LoadingEvents.RegisterOnAssetsLoaded(Info, PostLoad(), LoadingEventOrder.Post);
            GeneratorManagement.Register(this, GenerationModType.Finalizer, GetFloorResources);

            ModdedSaveGame.AddSaveHandler(Info);
        }

        public void GetFloorResources(string floorName, int floorNum, SceneObject scene)
        {
            foreach (WeightedSticker sticker in scene.potentialStickers)
            {
                if (!EndlessZoneManager.stickers.ContainsKey(sticker.selection))
                    EndlessZoneManager.stickers.Add(sticker.selection, new List<WeightedSticker>());

                EndlessZoneManager.stickers[sticker.selection].Add(sticker);
            }

            foreach (WeightedItemObject item in scene.shopItems)
            {
                if (!EndlessZoneManager.shopItems.ContainsKey(item.selection.itemType))
                    EndlessZoneManager.shopItems.Add(item.selection.itemType, new List<WeightedItemObject>());

                EndlessZoneManager.shopItems[item.selection.itemType].Add(item);
            }

            foreach (WeightedNPC npc in scene.potentialNPCs)
            {
                if (!EndlessZoneManager.potentialNPCs.ContainsKey(npc.selection.Character))
                    EndlessZoneManager.potentialNPCs.Add(npc.selection.Character, new List<WeightedNPC>());

                EndlessZoneManager.potentialNPCs[npc.selection.Character].Add(npc);
            }
        }

        private IEnumerator PreLoad()
        {
            yield return 1;
            yield return "Pre-loading Zones+";

            BinaryReader reader = new BinaryReader(File.OpenRead(Path.Combine(AssetLoader.GetModPath(this), "ZonePit.bpl")));
            var level = LevelImporter.CreateSceneObject(BaldiLevel.Read(reader));
            level.storeUsesNextLevelData = true;
            level.nextLevel = level;
            reader.Close();

            PosterObject advancedPoster = ObjectCreators.CreatePosterObject(AssetLoader.TextureFromMod(this, "Compacts", "AdvancedPoster.png"), []);
            level.levelAsset.posters.Add(new PosterData { poster = advancedPoster, position = new(12, 15), direction = Direction.North });

            PosterObject communityStickerPoster = ObjectCreators.CreatePosterObject(AssetLoader.TextureFromMod(this, "Compacts", "CommunityStickerPosters.png"), []);
            level.levelAsset.posters.Add(new PosterData { poster = communityStickerPoster, position = new(6, 11), direction = Direction.North });

            RoomFunctionContainer saveAndQuitContainer = new GameObject("SaveAndQuitRoomFunction").AddComponent<RoomFunctionContainer>();
            saveAndQuitContainer.ReflectionSetVariable("functions", new List<RoomFunction>());
            saveAndQuitContainer.AddFunction(saveAndQuitContainer.gameObject.AddComponent<CallSpecialManagerRoomFunction>());
            saveAndQuitContainer.GetComponent<CallSpecialManagerRoomFunction>().value = 1;
            saveAndQuitContainer.gameObject.ConvertToPrefab(true);

            level.levelAsset.rooms[1].roomFunctionContainer = saveAndQuitContainer;

            assetMan.Add("ZonePitStop", level);

            RoomTexturesHandler.LoadTextures(RoomCategory.Hall, CellTexturePart.Wall);
            RoomTexturesHandler.LoadTextures(RoomCategory.Class, CellTexturePart.Wall);
            RoomTexturesHandler.LoadTextures(RoomCategory.Faculty, CellTexturePart.Wall);

            assetMan.Add("ZoneBasePoster", AssetLoader.TextureFromMod(this, "ZonePosters", "ZonePosterBase.png"));

            for (int i = 0; i < 35; i++)
                assetMan.Add("ZoneBasePoster_" + i, AssetLoader.TextureFromMod(this, "ZonePosters", $"ZonePoster_{i}.png"));

            assetMan.Add("Clipboard", AssetLoader.SpriteFromMod(this, Vector2.one / 2, 1f, "ZoneStickers", "Clipboard.png"));
            assetMan.Add("Paper", AssetLoader.SpriteFromMod(this, Vector2.one / 2, 1f, "ZoneStickers", "StickerPaper.png"));
            assetMan.Add("PowerBonusDisplay", AssetLoader.SpriteFromMod(this, Vector2.one / 2, 1f, "ZoneStickers", "PowerBonusDisplay.png"));

            assetMan.Add("PaperSelectSound", ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromMod(this, "SelectZoneRule.wav"), "", SoundType.Effect, Color.white, 0f));
            assetMan.Add("PaperApplySound", ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromMod(this, "ChoseZoneRule.wav"), "", SoundType.Effect, Color.white, 0f));

            string[] paths = Directory.GetFiles(Path.Combine(AssetLoader.GetModPath(this), "ZoneStickers", "Drawings"), "*.png");

            foreach (string path in paths)
            {
                string name = Path.GetFileNameWithoutExtension(path);
                var sprite = AssetLoader.SpriteFromFile(path, Vector2.one / 2, 1f);
                assetMan.Add(name, sprite);
            }

            ZonePitStopManager manager = new BaseGameManagerBuilder<ZonePitStopManager>()
            .Build();

            manager.audElvMachine = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromMod(this, "ZoneMachine.wav"), "[Machine Working]", SoundType.Effect, Color.white);

            BaseGameManager zoneManager = new MainGameManagerBuilder<BasicZoneManager>()
            .SetCustomPitstop(level)
            .Build();

            level.manager = manager;

            assetMan.Add("ZoneManager", zoneManager);
            assetMan.Add("LobbyMusic", AssetLoader.MidiFromMod("LobbyMusic", this, "ZoneLobby.midi"));

            CharacterCreator.LoadAllNPCs();

            string roomFolder = Path.Combine(AssetLoader.GetModPath(this), "RoomAssets");

            assetPlusMan.AddFromResourcesNoClones<GameObject>();

            foreach (string file in Directory.GetFiles(roomFolder, "*.rbpl", SearchOption.AllDirectories))
            {
                using (BinaryReader roomReader = new BinaryReader(File.OpenRead(file)))
                {
                    var room = LevelImporter.CreateRoomAsset(BaldiRoomAsset.Read(roomReader), false);
                    room.posters = AssetFinder.FindAllOfType<RoomAsset>(true).LastOrDefault(x => x.category == room.category && (!x.hasActivity || x.activity.prefab == room.activity.prefab)).posters;
                    room.posterChance = AssetFinder.FindAllOfType<RoomAsset>(true).LastOrDefault(x => x.category == room.category && (!x.hasActivity || x.activity.prefab.GetType() == room.activity.prefab.GetType())).posterChance;

                    if (room.category == RoomCategory.Class)
                        room.roomFunctionContainer = AssetFinder.FindAllOfType<RoomAsset>(true).First(x => x.activity.prefab == room.activity.prefab).roomFunctionContainer;

                    room.name = Path.GetFileNameWithoutExtension(file);

                    room.basicSwaps.Add(new BasicObjectSwapData
                    {
                        prefabToSwap = assetPlusMan.Get<GameObject>("SodaMachine").transform,
                        potentialReplacements = new WeightedTransform[] 
                        { 
                            new WeightedTransform { selection = assetPlusMan.Get<GameObject>("SodaMachine").transform, weight = 25 },
                            new WeightedTransform { selection = assetPlusMan.Get<GameObject>("ZestyMachine").transform, weight = 100 },
                            new WeightedTransform { selection = assetPlusMan.Get<GameObject>("DietSodaMachine").transform, weight = 100 },
                            new WeightedTransform { selection = assetPlusMan.Get<GameObject>("CrazyVendingMachineZesty").transform, weight = 5 },
                            new WeightedTransform { selection = assetPlusMan.Get<GameObject>("CrazyVendingMachineBSODA").transform, weight = 5 }
                        },
                        chance = 1f
                    });

                    EndlessZoneManager.customRoomAssets.Add(room);
                    roomReader.Close();
                }
            }

            ZoneRuleDummyStructure dummyPrefab = new GameObject("ZoneRuleDummyStructure").gameObject.AddComponent<ZoneRuleDummyStructure>();
            dummyPrefab.gameObject.ConvertToPrefab(true);
            assetMan.Add<StructureBuilder>("ZoneRuleDummyStructure", dummyPrefab);

            string[] woodPaths = Directory.GetFiles(Path.Combine(AssetLoader.GetModPath(this), "ObjectTextures"), "*.png");

            foreach (string path in woodPaths)
            {
                string name = Path.GetFileNameWithoutExtension(path);
                Debug.Log(name);
                if (name.StartsWith("Wood_"))
                {
                    var texture = AssetLoader.TextureFromFile(path);
                    assetMan.Add(name, texture);
                }
            }

            assetMan.Add("UnvaliableSlot", AssetLoader.SpriteFromMod(this, Vector2.one / 2f, 50f, "Items", "UnvaliableSlot.png"));

            AssetLoader.LocalizationFromMod(this);

            CustomModesHandler.OnMainMenuInitialize += () =>
            {
                var mainScreen = ModeObject.CreateModeObjectOverExistingScreen(SelectionScreen.MainScreen);
                var zoneScreen = ModeObject.CreateBlankScreenInstance("ZoneLevelSelectionScreen", false, [Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero]);
                var zoneMenu = zoneScreen.ScreenTransform.gameObject.AddComponent<ZoneLevelSelectMenu>();

                var descriptionText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24, "Pick A Level!", zoneScreen.ScreenTransform, Vector3.zero, false);
                descriptionText.rectTransform.anchoredPosition = new(0f, -20f);
                descriptionText.rectTransform.sizeDelta = new(460f, 0f);
                descriptionText.color = Color.black;
                descriptionText.alignment = TextAlignmentOptions.Center;

                void CreateSelectableFloor(string floor, int id, float x, string levelDesc)
                {
                    var text = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24, floor, zoneScreen.ScreenTransform, Vector3.zero, false);
                    text.rectTransform.anchoredPosition = new(x, 60f);
                    text.color = Color.black;
                    text.raycastTarget = true;
                    text.alignment = TextAlignmentOptions.Center;

                    var button = text.gameObject.ConvertToButton<StandardMenuButton>(true);
                    button.underlineOnHigh = true;
                    button.eventOnHigh = true;
                    button.OnPress.AddListener(() =>
                    {
                        ZoneLevelSelectMenu.selectedLevel = id;
                        zoneMenu.UpdateButtons();
                    });
                    button.OnHighlight.AddListener(() => { descriptionText.text = levelDesc; });
                    zoneMenu.buttons.Add(button);
                }

                CreateSelectableFloor("F1", 0, -140f, "A small-sized level containing only 4 notebooks; the stickers cost the same, and the map costs 100 YTPs.\n(Not recommended, but it's here just for fun :D)");
                CreateSelectableFloor("F2", 1, 0f, "A medium-sized level containing only 7 notebooks; stickers cost 1.5x more, and the map costs 500 YTPs.");
                CreateSelectableFloor("F3", 2, 140f, "A large-sized level containing only 9 notebooks; stickers cost double, and the map costs 1.000 YTPs.");

                var newGameButton = zoneScreen.StandardButtonBuilder.CreateModeButton(level, false, 2, Mode.Main).AddTextVisual("Start New Game", out TextMeshProUGUI newGameText);
                newGameButton.OnPress.AddListener(() => {

                    if (ZoneLevelSelectMenu.lifeMode == LifeMode.Intense)
                        Singleton<CoreGameManager>.Instance.SetLives(0, true);
                    else if (ZoneLevelSelectMenu.lifeMode == LifeMode.Explorer)
                        Singleton<CoreGameManager>.Instance.currentMode = Mode.Free;


                    Singleton<CoreGameManager>.Instance.lifeMode = ZoneLevelSelectMenu.lifeMode;
                    ZoneLevelSelectMenu.toLoad = false; 
                });
                newGameText.alignment = TextAlignmentOptions.Left;
                newGameText.rectTransform.anchoredPosition = new(-110f, -129f);
                newGameText.rectTransform.sizeDelta = new(250f, 25f);

                var continueGameButton = zoneScreen.StandardButtonBuilder.CreateModeButton(level, false, 2, Mode.Main).AddTextVisual("Continue Saved Game", out TextMeshProUGUI continueGameText);
                continueGameButton.OnPress.AddListener(() => {

                    if (ZoneLevelSelectMenu.lifeMode == LifeMode.Intense)
                        Singleton<CoreGameManager>.Instance.SetLives(0, true);
                    else if (ZoneLevelSelectMenu.lifeMode == LifeMode.Explorer)
                        Singleton<CoreGameManager>.Instance.currentMode = Mode.Free;

                    Singleton<CoreGameManager>.Instance.lifeMode = ZoneLevelSelectMenu.lifeMode;
                    ZoneLevelSelectMenu.toLoad = true;
                });
                continueGameText.alignment = TextAlignmentOptions.Left;
                continueGameText.rectTransform.anchoredPosition = new(-110f, -159f);
                continueGameText.rectTransform.sizeDelta = new(250f, 25f);

                var playStyleText = zoneScreen.StandardButtonBuilder.CreateTextLabel(Vector2.zero, "Play Style");
                playStyleText.alignment = TextAlignmentOptions.Right;
                playStyleText.rectTransform.sizeDelta = new(300f, 50f);
                playStyleText.rectTransform.anchoredPosition = new(80f, -129f);

                var lifeModeText = zoneScreen.StandardButtonBuilder.CreateTextLabel(Vector2.zero, "Normal");
                lifeModeText.alignment = TextAlignmentOptions.Center;
                lifeModeText.rectTransform.sizeDelta = new(300f, 50f);
                lifeModeText.rectTransform.anchoredPosition = new(175f, -159f);

                var leftLifeArrow = zoneScreen.StandardButtonBuilder.CreateBlankButton("LeftLifeArrow").AddVisual(AssetFinder.FindOfTypeWithName<Sprite>("MenuArrowSheet_2", false));
                leftLifeArrow.image.rectTransform.sizeDelta = new(32f, 32f);
                leftLifeArrow.image.rectTransform.anchoredPosition = new(120f, -159f);
                leftLifeArrow.AddHighlightAnimation(AssetFinder.FindOfTypeWithName<Sprite>("MenuArrowSheet_0", false), AssetFinder.FindOfTypeWithName<Sprite>("MenuArrowSheet_2", false));
                leftLifeArrow.OnPress.AddListener(() => { zoneMenu.SetLifeMode(-1); });

                var rightLifeArrow = zoneScreen.StandardButtonBuilder.CreateBlankButton("RightLifeArrow").AddVisual(AssetFinder.FindOfTypeWithName<Sprite>("MenuArrowSheet_3", false));
                rightLifeArrow.image.rectTransform.sizeDelta = new(32f, 32f);
                rightLifeArrow.image.rectTransform.anchoredPosition = new(230f, -159f);
                rightLifeArrow.AddHighlightAnimation(AssetFinder.FindOfTypeWithName<Sprite>("MenuArrowSheet_1", false), AssetFinder.FindOfTypeWithName<Sprite>("MenuArrowSheet_3", false));
                rightLifeArrow.OnPress.AddListener(() => { zoneMenu.SetLifeMode(1); });

                zoneMenu.continueButton = continueGameButton;
                zoneMenu.lifeModeText = lifeModeText;

                zoneScreen.StandardButtonBuilder.CreateSeedInput(out SeedInput seedInput);

                var zoneButton = mainScreen.StandardButtonBuilder.CreateTransitionButton(zoneScreen).AddTextVisual("Endless Zones", false, out TextMeshProUGUI text);
                mainScreen.StandardButtonBuilder.AddDescriptionText(zoneButton, "Play the same level infinitely through zones, as Baldi's new machine can create infinite variations of the same level, and see how far you can go!");
            };
        }

        private IEnumerator PostLoad()
        {
            yield return 1;
            yield return "Post-loading Zones+";
            assetPlusMan.AddFromResourcesNoClones<LevelObject>();
            assetPlusMan.AddFromResourcesNoClones<GameButtonBase>();
            assetPlusMan.AddFromResourcesNoClones<PosterObject>();
            assetPlusMan.AddFromResourcesNoClones<StructureBuilder>();
            assetPlusMan.AddFromResourcesNoClones<RoomAsset>();
            assetPlusMan.AddFromResourcesNoClones<Transform>();
            assetPlusMan.AddFromResourcesNoClones<Texture2D>();
            assetPlusMan.AddFromResourcesNoClones<Cubemap>();
            assetPlusMan.AddFromResourcesNoClones<RoomFunctionContainer>();
            assetPlusMan.AddFromResourcesNoClones<StandardDoorMats>();
            assetPlusMan.AddFromResourcesNoClones<GameObject>();

            if (Chainloader.PluginInfos.ContainsKey("ganaisthere.plus.livestudentreaction"))
                LiveStudentReactionCompact.Load();

            if (Chainloader.PluginInfos.ContainsKey("mtm101.rulerp.baldiplus.levelstudio"))
                EditorCompability.LoadCompact();

            if (Chainloader.PluginInfos.ContainsKey("wazkitta.plusmod.communitystickers"))
                CommunityStickerCompact.LoadCompact();

            if (Chainloader.PluginInfos.ContainsKey("mrsasha5.baldi.basics.plus.advanced"))
                AdvancedCompact.LoadCompact();
        }
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "wazkitta.plusmod.pluszones";
        public const string PLUGIN_NAME = "In-Between Time-Zones";
        public const string PLUGIN_VERSION = "1.0";
    }
}
