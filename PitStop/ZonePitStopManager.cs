using BaldiPlusRandomZone.Compacts;
using BaldiPlusRandomZone.EndlessSupport;
using BaldiPlusRandomZone.Extensions;
using BaldiPlusRandomZone.Menu;
using BaldiPlusRandomZone.SaveSystem;
using BaldiPlusRandomZone.ZoneRules.UI;
using HarmonyLib;
using MidiPlayerTK;
using MTM101BaldAPI;
using MTM101BaldAPI.Reflection;
using MTM101BaldAPI.Registers.Buttons;
using MTM101BaldAPI.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace BaldiPlusRandomZone.PitStop
{
    public class ZonePitStopManager : BaseGameManager
    {
        public override void Initialize()
        {
            Singleton<CoreGameManager>.Instance.SpawnPlayers(ec);
            ec.AssignPlayers();
            generatorFinished = true;
            CollectNotebooks(0);
            GC.Collect();
            ec.ElevatorManager.SetAllElevators(ElevatorState.WaitingForPlayerExit);
            Singleton<CoreGameManager>.Instance.ResetCameras();
            Singleton<CoreGameManager>.Instance.ResetShaders();
            Singleton<CoreGameManager>.Instance.readyToStart = true;
            Singleton<CoreGameManager>.Instance.GetHud(0).ReInit();
            Singleton<CoreGameManager>.Instance.GetHud(0).SetNotebookDisplay(false);

            if (Singleton<EndlessZoneManager>.Instance == null)
            {
                var manager = new GameObject("EndlessZoneManager").AddComponent<EndlessZoneManager>();

                if (ZoneLevelSelectMenu.toLoad)
                {
                    manager.LoadFile(ZoneLevelSelectMenu.selectedLevel, ZoneLevelSelectMenu.lifeMode);
                    manager.Load();

                    if (manager.loadedSaveFile.attempts < 2)
                        Singleton<CoreGameManager>.Instance.lastLevelNumber = Singleton<EndlessZoneManager>.Instance.currentZone;
                    else
                        Singleton<CoreGameManager>.Instance.lastLevelNumber = Singleton<EndlessZoneManager>.Instance.currentZone - 1;

                    manager.DeleteFile(ZoneLevelSelectMenu.selectedLevel, ZoneLevelSelectMenu.lifeMode);
                }
            }

            Singleton<CoreGameManager>.Instance.sceneObject.shopItems = EndlessZoneManager.Instance.GetShopItems();
            Singleton<CoreGameManager>.Instance.sceneObject.potentialStickers = EndlessZoneManager.Instance.GetStickers();
            Singleton<CoreGameManager>.Instance.nextLevel = Singleton<CoreGameManager>.Instance.sceneObject;
            Singleton<CoreGameManager>.Instance.sceneObject.mapPrice = Singleton<EndlessZoneManager>.Instance.currentLevelData.mapPrice + (25 * Singleton<EndlessZoneManager>.Instance.currentZone);

            lockStore = true;
            if (Singleton<EndlessZoneManager>.Instance.currentZone > Singleton<CoreGameManager>.Instance.lastLevelNumber)
            {
                Singleton<CoreGameManager>.Instance.levelMapHasBeenPurchasedFor = null;
                Singleton<StickerManager>.Instance.ClearAppliedStickers();
                Singleton<StickerManager>.Instance.ReflectionSetVariable("slotUpgraded", new bool[4] { false, false, false, false });
                Singleton<EndlessZoneManager>.Instance.mapSaveData = new();
                Singleton<EndlessZoneManager>.Instance.loadedSaveFile = null;
                lockStore = false;
                clearZoneDrawings = true;
            }

            PosterObject levelChalk = ObjectCreators.CreateLevelTypeChalkboard(LevelNameGenerator.GenerateLevelName(new(Singleton<CoreGameManager>.Instance.Seed() + Singleton<EndlessZoneManager>.Instance.currentZone)));
            PosterObject zonePoster = PosterExtensions.CreateZonePosters(Singleton<EndlessZoneManager>.Instance.currentZone, new(Singleton<CoreGameManager>.Instance.Seed() + Singleton<EndlessZoneManager>.Instance.currentZone));
            ec.BuildPoster(zonePoster, ec.CellFromPosition(zonePosterPlacement.position), zonePosterPlacement.direction);
            ec.BuildPoster(levelChalk, ec.CellFromPosition(nextLevelPosterPlacement.position), nextLevelPosterPlacement.direction);
            var button = GameButton.Build(Plugin.assetPlusMan.Get<GameButtonBase>("GameButton_wBackFace"), ec, buttonPlacement.position, buttonPlacement.direction);
            button.gameObject.SetActive(true);
            button.ChangeColor("Lime");
            button.ReflectionSetVariable("triggerSpecialManagerFunction", true);
            button.transform.localPosition = Vector3.zero;
            button.name = "CoolButton";
            this.button = (GameButton)button;

            DestroyImmediate(levelChalk, true);
            DestroyImmediate(zonePoster, true);
            CreateCanvas();
        }

        public void CreateCanvas()
        {
            clipboardCanvas = UIHelpers.CreateBlankUIScreen("SelectScreenCanvas", true, false);
            clipboardCanvas.worldCamera = Singleton<GlobalCam>.Instance.Cam;
            clipboardCanvas.planeDistance = 0.31f;
            clipboardCanvas.renderMode = RenderMode.ScreenSpaceCamera;

            RectTransform rect = clipboardCanvas.GetComponent<RectTransform>();
            UIHelpers.AddCursorInitiatorToCanvas(clipboardCanvas,rect.rect.size);
            var clipboardMan = clipboardCanvas.gameObject.AddComponent<ZoneClipboardManager>();

            var clipboard = UIHelpers.CreateImage(Plugin.assetMan.Get<Sprite>("Clipboard"), clipboardCanvas.transform, Vector3.zero, false);
            clipboard.rectTransform.anchoredPosition = Vector2.zero;
            clipboard.rectTransform.anchorMin = new(0.5f, 0.5f);
            clipboard.rectTransform.anchorMax = new(0.5f, 0.5f);
            clipboard.rectTransform.pivot = new(0.5f, 0.5f);

            void CreatePaperSticker(Vector2 position)
            {
                var paper = UIHelpers.CreateImage(Plugin.assetMan.Get<Sprite>("Paper"), clipboardCanvas.transform, Vector3.zero, false);
                paper.rectTransform.anchoredPosition = position;
                paper.rectTransform.anchorMin = new(0.5f, 0.5f);
                paper.rectTransform.anchorMax = new(0.5f, 0.5f);
                paper.rectTransform.pivot = new(0.5f, 0.5f);

                var sticker = UIHelpers.CreateImage(Plugin.assetMan.Get<Sprite>("Drawing_Notebooks"), clipboardCanvas.transform, Vector3.zero, false);
                sticker.rectTransform.anchoredPosition = position;
                sticker.rectTransform.anchorMin = new(0.5f, 0.5f);
                sticker.rectTransform.anchorMax = new(0.5f, 0.5f);
                sticker.rectTransform.pivot = new(0.5f, 0.5f);

                var button = sticker.gameObject.ConvertToButton<StandardMenuButton>();
                button.eventOnHigh = true;

                button.OnHighlight.AddListener(() =>
                {
                    paper.transform.localPosition = new(paper.transform.localPosition.x, paper.transform.localPosition.y + 5f);
                    sticker.transform.localPosition = paper.transform.localPosition;
                    clipboardMan.OnHoverSticker(button);
                });

                button.OffHighlight.AddListener(() =>
                {
                    paper.transform.localPosition = new(paper.transform.localPosition.x, paper.transform.localPosition.y - 5f);
                    sticker.transform.localPosition = paper.transform.localPosition;
                });

                button.OnPress.AddListener(() => clipboardMan.SelectSticker(button, this));

                clipboardMan.papers.Add(paper);
                clipboardMan.buttons.Add(button);
            }

            CreatePaperSticker(new(-142f, 0f));
            CreatePaperSticker(new(0f, 0f));
            CreatePaperSticker(new(142f, 0f));

            var text1 = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24, "Select a Drawing:", clipboardCanvas.transform, Vector3.zero);
            text1.color = Color.black;
            text1.alignment = TextAlignmentOptions.Center;
            text1.rectTransform.anchoredPosition = new(0f, 85f);

            var text2 = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans18, "\"Select a drawing to see its description\"", clipboardCanvas.transform, Vector3.zero);
            text2.color = Color.black;
            text2.alignment = TextAlignmentOptions.Center;
            text2.rectTransform.anchoredPosition = new(0f, -85f);
            text2.rectTransform.sizeDelta = new(500f, 50f);
            clipboardMan.descText = text2;

            var pbDisplay = UIHelpers.CreateImage(Plugin.assetMan.Get<Sprite>("PowerBonusDisplay"), clipboardCanvas.transform, Vector3.zero, false);
            pbDisplay.rectTransform.anchoredPosition = new(0f, 200f);
            pbDisplay.rectTransform.anchorMin = new(0.5f, 0.5f);
            pbDisplay.rectTransform.anchorMax = new(0.5f, 0.5f);
            pbDisplay.rectTransform.pivot = new(0.5f, 0.5f);

            var pbText = UIHelpers.CreateText<TextMeshProUGUI>(BaldiFonts.ComicSans24, "1.0x", clipboardCanvas.transform, Vector3.zero);
            pbText.color = Color.green;
            pbText.alignment = TextAlignmentOptions.Center;
            pbText.rectTransform.anchoredPosition = new(0f, 165f);
            clipboardMan.pbText = pbText;
        }

        public override void BeginPlay()
        {
            base.BeginPlay();

            if (Singleton<EndlessZoneManager>.Instance.currentZone > Singleton<CoreGameManager>.Instance.lastLevelNumber)
            {
                CheckForStudents();
            }

            foreach (StorageLocker locker in FindObjectsOfType<StorageLocker>())
                locker.Invoke("Start", 0f);

            Singleton<CoreGameManager>.Instance.nextLevel.totalShopItems = 6;
            foreach (RoomController room in ec.rooms)
            {
                if (room.category == RoomCategory.Store)
                {
                    if (lockStore)
                    {
                        room.doors = FindObjectsOfType<Door>().Where(x => x.name == "Door_Auto(Clone)").ToList();
                        room.functions.GetComponent<StoreRoomFunction>().ReflectionInvoke("Close", []);
                        return;
                    }

                    room.itemSpawnPoints.AddRange(Plugin.assetPlusMan.Get<RoomAsset>("Room_JohnnysStore").itemSpawnPoints);

                    room.functions.Initialize(room);
                    var storeFunction = room.functions.gameObject.GetComponent<StoreRoomFunction>();
                    var pickups = (List<Pickup>)storeFunction.ReflectionGetVariable("pickups");
                    var tags = (PriceTag[])storeFunction.ReflectionGetVariable("tag");
                    var stickersPickups = (Pickup[])storeFunction.ReflectionGetVariable("stickerPickup");
                    var stickerTag = (PriceTag[])storeFunction.ReflectionGetVariable("stickerTag");

                    foreach (Pickup pickup in pickups)
                    {
                        pickup.transform.localPosition = pickup.transform.position;
                        pickup.gameObject.SetActive(true);
                        pickup.free = false;
                        pickup.price = GetItemPrice(pickup);
                        pickup.OnItemPurchased += Singleton<EndlessZoneManager>.Instance.OnBuyItem;
                        tags[pickups.IndexOf(pickup)].SetText(pickup.price.ToString());
                    }

                    foreach (Pickup pickup in stickersPickups)
                    {
                        pickup.OnItemPurchased += Singleton<EndlessZoneManager>.Instance.OnBuySticker;
                        pickup.price = Mathf.FloorToInt(GetStickerPackPrice(pickup) * Singleton<EndlessZoneManager>.Instance.currentLevelData.stickerPriceMultiplier);
                        stickerTag[Array.IndexOf(stickersPickups, pickup)].SetText(pickup.price.ToString());
                    }
                }
            }

            if (clearZoneDrawings)
                Singleton<EndlessZoneManager>.Instance.zoneRules.Clear();

            Singleton<MusicManager>.Instance.PlayMidi(Plugin.assetMan.Get<string>("LobbyMusic"), true);
        }

        public int GetItemPrice(Pickup pickup)
        {
            string itemId = pickup.item.itemType.ToStringExtended();
            ShopItemData data = Singleton<EndlessZoneManager>.Instance.shopItemsData.FirstOrDefault(x => x.itemId == itemId);

            if (data == null)
                return pickup.item.price;

            float multiplier = 1f + (Mathf.Sqrt(data.demand) * 0.25f);
            return Mathf.RoundToInt(data.originalPrice * multiplier);
        }

        public int GetStickerPackPrice(Pickup pickup)
        {
            if (pickup.item.item.GetComponent<ITM_StickerPack>() == null)
                return pickup.price;

            StickerPackType type = (StickerPackType)pickup.item.item.GetComponent<ITM_StickerPack>().ReflectionGetVariable("type");
            int total = (int)pickup.item.item.GetComponent<ITM_StickerPack>().ReflectionGetVariable("total");
            string itemId = type.ToString() + "_" + total.ToString();
            ShopItemData data = Singleton<EndlessZoneManager>.Instance.shopItemsData.FirstOrDefault(x => x.itemId == itemId);

            if (data == null)
                return pickup.item.price;

            float multiplier = 1f + (Mathf.Sqrt(data.demand) * 0.15f);
            return Mathf.RoundToInt(data.originalPrice * multiplier);
        }

        protected override void VirtualUpdate()
        {
            base.VirtualUpdate();

            CoreGameManager instance = Singleton<CoreGameManager>.Instance;
            if (instance == null) return;
            
            PlayerManager player = instance.GetPlayer(0);
            if (player == null) return;
            
            player.plm.AddStamina(100f, true);
        }

        protected override void LoadSceneObject(SceneObject sceneObject, bool restarting)
        {
            base.LoadSceneObject(Singleton<CoreGameManager>.Instance.nextLevel, restarting);
        }

        public override void LoadNextLevel()
        {
            base.PrepareToLoad();
            if (Singleton<ElevatorScreen>.Instance != null)
            {
                elevatorScreen = Singleton<ElevatorScreen>.Instance;
                elevatorScreen.Reinit();
                elevatorScreen.OnLoadReady += base.LoadNextLevel;
                return;
            }
            elevatorScreen = Instantiate(elevatorScreenPre);
            elevatorScreen.OnLoadReady += base.LoadNextLevel;
            elevatorScreen.Initialize();
        }

        public override void CallSpecialManagerFunction(int val, GameObject source)
        {
            base.CallSpecialManagerFunction(val, source);

            bool powered = (bool)button.ReflectionGetVariable("powered");

            switch (val)
            {
                case 0:
                    if (powered)
                    {
                        button.SetPowered(false);
                        OpenSelectScreen();
                    }
                    break;
                case 1:
                    Singleton<EndlessZoneManager>.Instance.SaveAndQuit();
                    break;
            }
        }

        public void OpenSelectScreen()
        {
            bool overrideLevelCheck = false;
            if (Singleton<EndlessZoneManager>.Instance.loadedSaveFile != null && Singleton<EndlessZoneManager>.Instance.loadedSaveFile.zoneRule != ZoneRules.ZoneRuleCategory.Empty)
            {
                var zoneRule = Singleton<EndlessZoneManager>.Instance.GetZoneRule(Singleton<EndlessZoneManager>.Instance.loadedSaveFile.zoneRule);
                zoneRule.LoadPreparation(new());
                zoneRule.SetRuleType(Singleton<EndlessZoneManager>.Instance.loadedSaveFile.zoneRuleType);

                Singleton<EndlessZoneManager>.Instance.zoneRules.Add(zoneRule);
                StartCoroutine(WaitLevelLoad());
                return;
            }
            else if (Singleton<EndlessZoneManager>.Instance.loadedSaveFile != null && Singleton<EndlessZoneManager>.Instance.loadedSaveFile.zoneRule == ZoneRules.ZoneRuleCategory.Empty)
                overrideLevelCheck = true;
            

            if (Singleton<EndlessZoneManager>.Instance.currentZone > Singleton<CoreGameManager>.Instance.lastLevelNumber || overrideLevelCheck)
            {
                clipboardCanvas.GetComponent<ZoneClipboardManager>().OnOpenScreen();
                clipboardCanvas.gameObject.SetActive(true);
                ec.AddTimeScale(pauseTimescale);
                Singleton<GlobalCam>.Instance.FadeIn(UiTransition.Dither, 0.0512222245669f);
                return;
            }

            StartCoroutine(WaitLevelLoad());
        }

        public void CloseSelectScreen()
        {
            Singleton<GlobalCam>.Instance.Transition(UiTransition.Dither, 0.0512222245669f);
            ec.RemoveTimeScale(pauseTimescale);
            Destroy(clipboardCanvas.gameObject);
            SetSilenceMusic(false);

            switch (Singleton<EndlessZoneManager>.Instance.zoneRules[0].type)
            {
                case ZoneRules.ZoneRuleType.Positive:
                    LiveStudentReactionCompact.SetCustomState("Happy", 8f);
                    break;
                case ZoneRules.ZoneRuleType.Maybe:
                    LiveStudentReactionCompact.SetCustomState("Mid", 8f);
                    break;
                case ZoneRules.ZoneRuleType.Negative:
                    LiveStudentReactionCompact.SetCustomState("Scary", 8f);
                    break;
            }

            StartCoroutine(WaitLevelLoad());
        }

        public IEnumerator WaitLevelLoad()
        {
            Singleton<EndlessZoneManager>.Instance.LoadNextZone(out bool alreadyLoaded);
            ElevatorDoor door = (ElevatorDoor)ec.Elevators[1].Door;
            AudioManager audMan = (AudioManager)door.ReflectionGetVariable("audMan");
            audMan.PlaySingle(audElvMachine);
            float targetPitch = 1f;
            float pitchSpeed = 0.5f;

            while (!Singleton<EndlessZoneManager>.Instance.levelLoaded || (audMan.AnyAudioIsPlaying && !alreadyLoaded))
            {
                if (Mathf.Abs(audMan.pitchModifier - targetPitch) < 0.01f)
                    targetPitch = UnityEngine.Random.Range(0.2f, 1.8f);

                audMan.pitchModifier = Mathf.MoveTowards(audMan.pitchModifier, targetPitch, pitchSpeed * Time.deltaTime);
                yield return null;
            }

            audMan.pitchModifier = 1f;

            if (alreadyLoaded)
                audMan.FlushQueue(true);

            Singleton<CoreGameManager>.Instance.nextLevel = Singleton<EndlessZoneManager>.Instance.currentScene;
            ec.ElevatorManager.SetTotalOutOfOrderElevators(0);
            ec.Elevators[1].SetState(ElevatorState.OpenForExit);
        }

        public void CheckForStudents()
        {
            var prefab = (Structure_StudentSpawner)Plugin.assetPlusMan.Get<StructureBuilder>("StudentSpawnerConstructor");
            var studentSpawner = Instantiate(prefab);
            studentSpawner.Initialize(ec, new());
            studentSpawner.SetStartingValue();
            var loseableItems = (ItemObject[])studentSpawner.ReflectionGetVariable("loseableItem");
            List<ItemObject> possibleItems = new List<ItemObject>();
            possibleItems.AddRange(loseableItems);
            
            foreach (var item in loseableItems)
            {
                if (Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.Has(item.itemType))
                {
                    studentSpawner.ReflectionSetVariable("loseableItem", new ItemObject[] { item });
                    studentSpawner.SpawnStudents(1, false);
                    ec.Npcs[ec.Npcs.Count - 1].Entity.Teleport(ec.mainHall.RandomEntitySafeCellNoGarbage().CenterWorldPosition);
                }
            }

            foreach (var item in Singleton<CoreGameManager>.Instance.currentLockerItems)
            {
                if (possibleItems.Contains(item))
                {
                    studentSpawner.ReflectionSetVariable("loseableItem", new ItemObject[] { item });
                    studentSpawner.SpawnStudents(1, false);
                    ec.Npcs[ec.Npcs.Count - 1].Entity.Teleport(ec.mainHall.RandomEntitySafeCellNoGarbage().CenterWorldPosition);
                }
            }

            foreach (var item in ec.items)
                item.gameObject.SetActive(false);
        }

        static FieldInfo _midiPlayer = AccessTools.Field(typeof(MusicManager), "midiPlayer");
        public static void SetSilenceMusic(bool silent)
        {
            MidiFilePlayer midiPlayer = (MidiFilePlayer)_midiPlayer.GetValue(Singleton<MusicManager>.Instance);

            for (int i = 0; i < midiPlayer.Channels.Length; i++)
                midiPlayer.MPTK_ChannelEnableSet(i, silent ? (i == 1 || i == 9) : true);
            
        }

        public override bool InPitstop() => !Singleton<CoreGameManager>.Instance.Paused || Singleton<CoreGameManager>.Instance.MapOpen;
        public override int CurrentStickerLevel() => Singleton<EndlessZoneManager>.Instance.currentZone;

        public GameButton button;
        public DirectedIntVector2 zonePosterPlacement = new DirectedIntVector2
        {
            position = new(10, 12),
            direction = Direction.North
        };

        public DirectedIntVector2 nextLevelPosterPlacement = new DirectedIntVector2
        {
            position = new(8, 12),
            direction = Direction.North
        };

        public DirectedIntVector2 buttonPlacement = new DirectedIntVector2
        {
            position = new(7, 11),
            direction = Direction.North
        };

        public Canvas clipboardCanvas;
        public SoundObject audElvMachine;
        public TimeScaleModifier pauseTimescale = new(0f, 0f, 0f);
        public bool lockStore, clearZoneDrawings;
    }
}
