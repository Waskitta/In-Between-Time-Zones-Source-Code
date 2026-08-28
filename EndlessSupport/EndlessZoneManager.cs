using BaldiPlusRandomZone.CustomCharacters;
using BaldiPlusRandomZone.Extensions;
using BaldiPlusRandomZone.Levels;
using BaldiPlusRandomZone.Menu;
using BaldiPlusRandomZone.SaveSystem;
using BaldiPlusRandomZone.ZoneRules;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Reflection;
using MTM101BaldAPI.Registers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BaldiPlusRandomZone.EndlessSupport
{
    public class EndlessZoneManager : Singleton<EndlessZoneManager>
    {
        private void Update()
        {
            if (Singleton<CoreGameManager>.Instance == null)
                Destroy(gameObject);
        }

        public void AdvanceZone()
        {
            currentZone++;
        }

        public void LoadNextZone(out bool alreadyLoaded)
        {
            if (currentScene != null)
            {
                if (currentScene.levelNo != currentZone)
                {
                    DestroyImmediate(currentLevel, true);
                    DestroyImmediate(currentScene, true);
                }
                else
                {
                    levelLoaded = true;
                    alreadyLoaded = true;
                    return;
                }
            }

            levelLoaded = false;
            alreadyLoaded = false;
            StartCoroutine(CreateNextZone());
        }

        public IEnumerator CreateNextZone()
        {
            var random = new System.Random(Singleton<CoreGameManager>.Instance.Seed() + currentZone);
            currentLevel = Instantiate(currentLevelData.level);
            yield return null;
            currentLevel.hallWallTexs = currentLevel.hallWallTexs.AddRangeToArray(RoomTexturesHandler.LoadTextures(RoomCategory.Hall, CellTexturePart.Wall));
            yield return null;
            currentLevel.hallFloorTexs = currentLevel.hallFloorTexs.AddRangeToArray(RoomTexturesHandler.LoadTextures(RoomCategory.Hall, CellTexturePart.Floor));
            yield return null;
            currentLevel.hallCeilingTexs = currentLevel.hallCeilingTexs.AddRangeToArray(RoomTexturesHandler.LoadTextures(RoomCategory.Hall, CellTexturePart.Ceiling));
            yield return null;

            currentLevel.SetCustomModValue(Plugin.instance.Info, "WoodTextures", Plugin.assetMan.GetAll<Texture2D>().Where(x => x.name.StartsWith("Wood_")).ToArray());

            foreach (RoomGroup group in currentLevel.roomGroup)
            {
                if (group.name == "Class")
                {
                    group.wallTexture = group.wallTexture.AddRangeToArray(RoomTexturesHandler.LoadTextures(RoomCategory.Class, CellTexturePart.Wall));
                    group.floorTexture = group.floorTexture.AddRangeToArray(RoomTexturesHandler.LoadTextures(RoomCategory.Hall, CellTexturePart.Floor));
                    group.potentialRooms = group.potentialRooms.AddRangeToArray(customRoomAssets.Where(x => x.category == RoomCategory.Class && ContainsActivity(x.activity.prefab, group.potentialRooms)).Select(x => WeightRoomBasedOnArray(x, group.potentialRooms)).ToArray());
                    yield return null;
                }
                else if (group.name == "Faculty" || group.name == "LockedRoom")
                {
                    group.wallTexture = group.wallTexture.AddRangeToArray(RoomTexturesHandler.LoadTextures(RoomCategory.Hall, CellTexturePart.Wall));
                    group.wallTexture = group.wallTexture.AddRangeToArray(RoomTexturesHandler.LoadTextures(RoomCategory.Faculty, CellTexturePart.Wall));
                    group.floorTexture = group.floorTexture.AddRangeToArray(RoomTexturesHandler.LoadTextures(RoomCategory.Hall, CellTexturePart.Floor));

                    if (group.name != "LockedRoom")
                        group.potentialRooms = group.potentialRooms.AddRangeToArray(customRoomAssets.Where(x => x.category == RoomCategory.Faculty).Select(x => WeightRoomBasedOnArray(x, group.potentialRooms)).ToArray());
                    yield return null;
                }
                else if (group.name == "Office")
                {
                    group.wallTexture = group.wallTexture.AddRangeToArray(RoomTexturesHandler.LoadTextures(RoomCategory.Class, CellTexturePart.Wall));
                    group.floorTexture = group.floorTexture.AddRangeToArray(RoomTexturesHandler.LoadTextures(RoomCategory.Hall, CellTexturePart.Floor));
                    group.potentialRooms = group.potentialRooms.AddRangeToArray(customRoomAssets.Where(x => x.category == RoomCategory.Office).Select(x => WeightRoomBasedOnArray(x, group.potentialRooms)).ToArray());
                    yield return null;
                }

                group.ceilingTexture = group.ceilingTexture.AddRangeToArray(RoomTexturesHandler.LoadTextures(RoomCategory.Hall, CellTexturePart.Ceiling));
                yield return null;
            }

            List<WeightedNPC> potentialNpcs = new List<WeightedNPC>();

            foreach (WeightedNPC npc in GetPotentialNPCs())
                potentialNpcs.Add(new WeightedNPC { selection = GetReplacementCharacter(npc.selection, random), weight = npc.weight });

            int target = Mathf.RoundToInt(Mathf.Pow(1.25f, currentZone));
            currentScene = ScriptableObject.CreateInstance<SceneObject>();
            currentScene.levelObject = currentLevel;
            currentScene.levelNo = currentZone;
            currentScene.levelTitle = "Z" + currentZone;
            currentScene.manager = Plugin.assetMan.Get<BaseGameManager>("ZoneManager");
            currentScene.skybox = AssetFinder.FindOfTypeWithName<Cubemap>("Cubemap_Twilight", true);
            currentScene.baldiPrefab = currentLevelData.mainBaldi;
            currentScene.additionalNPCs = target + 1;
            currentScene.name = "ZoneScene_" + currentZone;
            currentScene.potentialNPCs = potentialNpcs;
            currentScene.shopItems = GetShopItems();
            currentScene.totalShopItems = currentLevelData.shopItemCount;
            currentScene.potentialStickers = GetStickers();
            currentScene.forcedNpcs = [GetReplacementCharacter(NPCMetaStorage.Instance.Get(Character.Principal).value, random)];

            foreach (ZoneRule rule in zoneRules)
            {
                rule.ModifySceneObject(currentScene);
                yield return null;
            }

            levelLoaded = true;
        }

        public NPC GetReplacementCharacter(NPC npc, System.Random random)
        {
            List<WeightedNPC> list = new List<WeightedNPC> { new WeightedNPC { selection = npc, weight = 100 } };

            foreach (ReplacementCharacter rc in CharacterCreator.replacementCharacters)
            {
                if (rc.npc == npc.Character)
                    list.Add(new WeightedNPC { selection = rc.replacement, weight = rc.weight });
            }

            foreach (var n in list)
                Debug.Log(n.selection.name + " - " + n.weight.ToString());

            return WeightedNPC.ControlledRandomSelectionList(WeightedNPC.Convert(list), random);
        }

        public NPC GetReplacementCharacter(Character npc, System.Random random)
        {
            List<WeightedNPC> list = new List<WeightedNPC>();

            foreach (ReplacementCharacter rc in CharacterCreator.replacementCharacters)
            {
                if (rc.npc == npc)
                    list.Add(new WeightedNPC { selection = rc.replacement, weight = rc.weight });
            }

            return WeightedNPC.ControlledRandomSelectionList(WeightedNPC.Convert(list), random);
        }

        public WeightedRoomAsset WeightRoomBasedOnArray(RoomAsset room, WeightedRoomAsset[] array)
        {
            int cellsCount = room.cells.Count;
            int itemSpawnCount = room.itemSpawnPoints.Count;

            float totalWeight = 0f;
            float totalInfluence = 0f;

            foreach (var weightedRoom in array)
            {
                int cellDifference = Mathf.Abs(weightedRoom.selection.cells.Count - cellsCount);
                int itemDifference = Mathf.Abs(weightedRoom.selection.itemSpawnPoints.Count - itemSpawnCount);

                float distance = cellDifference + itemDifference * 2f;
                float influence = 1f / (1f + distance);

                totalWeight += weightedRoom.weight * influence;
                totalInfluence += influence;
            }

            float finalWeight = totalInfluence > 0f ? totalWeight / totalInfluence : 100f;

            return new WeightedRoomAsset { selection = room, weight = Mathf.RoundToInt(finalWeight) };
        }

        public bool ContainsActivity(Activity activity, WeightedRoomAsset[] array)
        {
            foreach (WeightedRoomAsset weightedRoom in array)
            {
                if (weightedRoom.selection.activity.prefab.GetType() == activity.GetType())
                    return true;
            }

            return false;
        }

        public void Save()
        {
            LevelZoneSave save = new LevelZoneSave();
            PlayerManager player = Singleton<CoreGameManager>.Instance.GetPlayer(0);
            Map map = Singleton<BaseGameManager>.Instance.Ec.map;

            save.items = new string[player.itm.items.Length];
            for(int i = 0; i < player.itm.items.Length; i++)
                save.items[i] = player.itm.items[i].itemType.ToStringExtended();

            save.lockersItems = new string[Singleton<CoreGameManager>.Instance.currentLockerItems.Length];
            for (int i = 0; i < Singleton<CoreGameManager>.Instance.currentLockerItems.Length; i++)
                save.lockersItems[i] = Singleton<CoreGameManager>.Instance.currentLockerItems[i].itemType.ToStringExtended();

            save.appliedStickers = Singleton<StickerManager>.Instance.activeStickerData.Select(x => new ZoneStickerStateData(x)).ToArray();
            save.inventoryStickers = Singleton<StickerManager>.Instance.stickerInventory.Select(x => new ZoneStickerStateData(x)).ToList();

            save.upgradedSlots = new bool[save.upgradedSlots.Length];
            for (int i = 0; i < save.upgradedSlots.Length; i++)
                save.upgradedSlots[i] = Singleton<StickerManager>.Instance.SlotUpgraded(i);

            save.ytps = Singleton<CoreGameManager>.Instance.GetPoints(0);
            save.lives = Singleton<CoreGameManager>.Instance.Lives;
            save.attempts = Singleton<CoreGameManager>.Instance.Attempts;
            save.saveAvaliable = true;

            save.map = mapSaveData;

            save.zone = currentZone;
            save.level = currentLevelData.levelSize;

            if (zoneRules.Count > 0)
            {
                save.zoneRule = zoneRules[0].category;
                save.zoneRuleType = zoneRules[0].type;
            }

            save.shopItems = shopItemsData;
            save.avaliableLockerSlots = upgradedLockerSlots;
            save.seed = Singleton<CoreGameManager>.Instance.Seed();
            save.lifeMode = Singleton<CoreGameManager>.Instance.lifeMode;
            savefile = save;
        }

        public void SaveAsFile()
        {
            string path = Path.Combine(Application.persistentDataPath, $"LevelZoneSave_{Singleton<PlayerFileManager>.Instance.fileName}_{savefile.level}_{savefile.lifeMode}.lzsf");
            File.WriteAllText(path, JsonUtility.ToJson(savefile, true));
        }

        public void SaveAndQuit()
        {
            Save();
            SaveAsFile();
            Singleton<CoreGameManager>.Instance.Quit();
        }

        public void Load()
        {
            if (loadedSaveFile == null) return;

            Singleton<StickerManager>.Instance.activeStickerData = loadedSaveFile.appliedStickers.Select(x => new StickerStateData(StickerMetaStorage.Instance.All().FirstOrDefault(y => y.type.ToStringExtended() == x.sticker).type, x.activeLevel, x.opened, x.sticky)).ToArray();
            Singleton<StickerManager>.Instance.stickerInventory = loadedSaveFile.inventoryStickers.Select(x => new StickerStateData(StickerMetaStorage.Instance.All().FirstOrDefault(y => y.type.ToStringExtended() == x.sticker).type, x.activeLevel, x.opened, x.sticky)).ToList();

            for (int i = 0; i < loadedSaveFile.upgradedSlots.Length; i++)
            {
                if (loadedSaveFile.upgradedSlots[i])
                    Singleton<StickerManager>.Instance.UpgradeSlot(i);
            }

            Singleton<CoreGameManager>.Instance.SetSeed(loadedSaveFile.seed);
            Singleton<CoreGameManager>.Instance.SetLives(loadedSaveFile.lives, true);
            Singleton<CoreGameManager>.Instance.SetAttempts(loadedSaveFile.attempts);
            Singleton<CoreGameManager>.Instance.AddPoints(loadedSaveFile.ytps, 0, false, false, false);
            Singleton<CoreGameManager>.Instance.saveMapPurchased = loadedSaveFile.map.buyedMap;
            Singleton<CoreGameManager>.Instance.saveMapAvailable = loadedSaveFile.map.mapAvaliable;

            mapSaveData = loadedSaveFile.map;

            PlayerManager pm = Singleton<CoreGameManager>.Instance.GetPlayer(0);

            for (int i = 0; i < loadedSaveFile.items.Length; i++)
                pm.itm.SetItem(ItemMetaStorage.Instance.All().FirstOrDefault(x => x.value.itemType.ToStringExtended() == loadedSaveFile.items[i]).value, i);

            for (int i = 0; i < loadedSaveFile.lockersItems.Length; i++)
                Singleton<CoreGameManager>.Instance.currentLockerItems[i] = ItemMetaStorage.Instance.All().FirstOrDefault(x => x.value.itemType.ToStringExtended() == loadedSaveFile.lockersItems[i]).value;

            Singleton<CoreGameManager>.Instance.lifeMode = loadedSaveFile.lifeMode;
            shopItemsData = loadedSaveFile.shopItems;

            upgradedLockerSlots = loadedSaveFile.avaliableLockerSlots;

            currentZone = loadedSaveFile.zone;
        }

        public void LoadFile(int level, LifeMode lifeMode)
        {
            string path = Path.Combine(Application.persistentDataPath, $"LevelZoneSave_{Singleton<PlayerFileManager>.Instance.fileName}_{level}_{lifeMode.ToString()}.lzsf");

            if (File.Exists(path))
                loadedSaveFile = JsonUtility.FromJson<LevelZoneSave>(File.ReadAllText(path));
        }

        public void DeleteFile(int level, LifeMode lifeMode)
        {
            string path = Path.Combine(Application.persistentDataPath, $"LevelZoneSave_{Singleton<PlayerFileManager>.Instance.fileName}_{level}_{lifeMode.ToString()}.lzsf");

            if (File.Exists(path))
                File.Delete(path);
        }

        public ZoneRule GetZoneRule(ZoneRuleCategory catgeory)
        {
            foreach (ZoneRule rule in possibleZoneRules)
            {
                if (catgeory == rule.category)
                    return rule;
            }

            return null;
        }

        public void OnBuyItem(Pickup pickup, int player)
        {
            string itemId = pickup.item.itemType.ToStringExtended();
            ShopItemData data = shopItemsData.FirstOrDefault(x => x.itemId == itemId);

            if (data == null)
            {
                data = new ShopItemData
                {
                    itemId = itemId,
                    demand = 0,
                    originalPrice = pickup.item.price
                };

                shopItemsData.Add(data);
            }

            data.demand++;
        }

        public void OnBuySticker(Pickup pickup, int player)
        {
            if (pickup.item.item.GetComponent<ITM_StickerPack>() == null)
                return;

            StickerPackType type = (StickerPackType)pickup.item.item.GetComponent<ITM_StickerPack>().ReflectionGetVariable("type");
            int total = (int)pickup.item.item.GetComponent<ITM_StickerPack>().ReflectionGetVariable("total");
            string itemId = type.ToString() + "_" + total.ToString();
            ShopItemData data = shopItemsData.FirstOrDefault(x => x.itemId == itemId);

            if (data == null)
            {
                data = new ShopItemData
                {
                    itemId = itemId,
                    demand = 0,
                    originalPrice = pickup.item.price
                };

                shopItemsData.Add(data);
            }

            data.demand++;
        }

        public WeightedItemObject[] GetShopItems()
        {
            List<WeightedItemObject> items = new List<WeightedItemObject>();

            foreach (Items item in shopItems.Keys)
            {
                items.Add(new WeightedItemObject
                {
                    selection = shopItems[item][0].selection,
                    weight = GetWeightFromMultipleWeighteds(shopItems[item])
                });
            }

            return items.ToArray();
        }

        public WeightedSticker[] GetStickers()
        {
            List<WeightedSticker> stickersList = new List<WeightedSticker>();

            foreach (Sticker sticker in stickers.Keys)
                stickersList.Add(new WeightedSticker(sticker, GetWeightFromMultipleWeighteds(stickers[sticker])));

            return stickersList.ToArray();
        }

        public WeightedNPC[] GetPotentialNPCs()
        {
            List<WeightedNPC> npcs = new List<WeightedNPC>();

            foreach (Character character in potentialNPCs.Keys)
            {
                npcs.Add(new WeightedNPC
                {
                    selection = potentialNPCs[character][0].selection,
                    weight = GetWeightFromMultipleWeighteds(potentialNPCs[character]) * (character == Character.Baldi ? 99 : 1)
                });
            }

            return npcs.ToArray();
        }

        public int GetWeightFromMultipleWeighteds<T>(IEnumerable<WeightedSelection<T>> weighteds)
        {
            int weight = 0;
            int count = 0;

            foreach (WeightedSelection<T> weighted in weighteds)
            {
                weight += weighted.weight;
                count++;
            }

            return count > 0 ? weight / count : 0;
        }

        public int currentZone;
        public SceneObject currentScene;
        public CustomLevelObject currentLevel;
        public bool levelLoaded;
        public RandomLevelData currentLevelData => levels[ZoneLevelSelectMenu.selectedLevel];

        public RandomLevelData[] levels = new RandomLevelData[3]
        {
            new RandomLevelData
            {
                levelSize = 0,
                level = AssetFinder.FindOfTypeWithName<CustomLevelObject>("Schoolhouse_Lvl1", false),
                mainBaldi = AssetFinder.FindAllOfType<Baldi>(true).FirstOrDefault(x => x.name == "Baldi_Main1"),
                shopItemCount = 3,
                mapPrice = 100,
                stickerPriceMultiplier = 1f
            },
            new RandomLevelData
            {
                levelSize = 1,
                level = AssetFinder.FindOfTypeWithName<CustomLevelObject>("Schoolhouse_Lvl2", false),
                mainBaldi = AssetFinder.FindAllOfType<Baldi>(true).FirstOrDefault(x => x.name == "Baldi_Main2"),
                shopItemCount = 6,
                mapPrice = 500,
                stickerPriceMultiplier = 1.5f
            },
            new RandomLevelData
            {
                levelSize = 2,
                level = AssetFinder.FindOfTypeWithName<CustomLevelObject>("Schoolhouse_Lvl3", false),
                mainBaldi = AssetFinder.FindAllOfType<Baldi>(true).FirstOrDefault(x => x.name == "Baldi_Main3"),
                shopItemCount = 9,
                mapPrice = 1000,
                stickerPriceMultiplier = 2f
            }
        };

        public LevelZoneSave savefile;
        public LevelZoneSave loadedSaveFile;
        public MapSaveData mapSaveData = new();
        public List<ShopItemData> shopItemsData = new List<ShopItemData>();
        public bool[] upgradedLockerSlots = new bool[3];

        public List<ZoneRule> zoneRules = new List<ZoneRule>();

        public static List<ZoneRule> possibleZoneRules = new List<ZoneRule>
        {
            new ZoneRule_Notebooks(50, 100),
            new ZoneRule_MapSize(60, 80),
            new ZoneRule_LightValue(50, 40),
            new ZoneRule_StickyRooms(100, 60),
            new ZoneRule_PotentialNpcValue(60, 75),
            new ZoneRule_ElevatorValues(80, 90),
            new ZoneRule_Plots(60, 70),
            new ZoneRule_TimeLimitValue(40, 80),
            new ZoneRule_FactoryCranes(80),
            new ZoneRule_LabZones(80),
            new ZoneRule_RoomQuantityValue(60, 75),
            new ZoneRule_ItemSpawnValue(45, 60),
            new ZoneRule_EventTimeValue(40, 50),
            new ZoneRule_SpecialRoomValue(70, 90),
            new ZoneRule_BaldiMove(60),
            new ZoneRule_ClonedNPCs(50),
            new ZoneRule_HallFormations(45, 35),
            new ZoneRule_NpcSpeed(30, 45),
            new ZoneRule_LargeEmptyArea(60),
            new ZoneRule_ColorLights(70),
            new ZoneRule_BaldiSpeed(35, 60),
            new ZoneRule_NoMap(46),
            new ZoneRule_OpenHallways(50),
            new ZoneRule_OutlineRooms(56),
            new ZoneRule_ConveyorBelts(40),
            new ZoneRule_BrokenWindows(50),
            new ZoneRule_ExtraWindow(50, 40),
            new ZoneRule_DebugRooms(45),
            new ZoneRule_EdgeBuffer(60, 25),
            new ZoneRule_HallwayBridges(60, 70),
            new ZoneRule_CurvyHallways(50),
            new ZoneRule_MaintenanceWires(42),
            new ZoneRule_HallwayDoors(40, 60),
            new ZoneRule_Line(50),
            new ZoneRule_DeadEnd(60),
            new ZoneRule_Ventilated(75),
            new ZoneRule_MoreReplacements(45, 55),
            new ZoneRule_ValvesDusty(60),
            new ZoneRule_BlueLocker(40, 60),
            new ZoneRule_WaterFountain(60, 50),
            new ZoneRule_StudentsLost(40, 50),
            new ZoneRule_ValuablePoints(50, 60)
        };

        public static Dictionary<Items, List<WeightedItemObject>> shopItems = new Dictionary<Items, List<WeightedItemObject>>();
        public static Dictionary<Character, List<WeightedNPC>> potentialNPCs = new Dictionary<Character, List<WeightedNPC>>();
        public static Dictionary<Sticker, List<WeightedSticker>> stickers = new Dictionary<Sticker, List<WeightedSticker>>();

        public static List<RoomAsset> customRoomAssets = new List<RoomAsset>();
    }
}
