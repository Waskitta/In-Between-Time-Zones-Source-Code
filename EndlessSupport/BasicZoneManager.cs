using BaldiPlusRandomZone.Patches;
using BaldiPlusRandomZone.ZoneRules;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaldiPlusRandomZone.EndlessSupport
{
    public class BasicZoneManager : MainGameManager
    {
        public override void Initialize()
        {
            base.Initialize();

            foreach (ZoneRule rule in Singleton<EndlessZoneManager>.Instance.zoneRules)
                rule.Initialize(this, ec);

            foreach (RoomController roomController in ec.rooms)
            {
                foreach (Pickup pickup in roomController.pickups)
                {
                    if (pickup.item.itemType == Items.BusPass)
                    {
                        pickup.Hide(true);
                        ec.RespawnItemInRoom(ItemMetaStorage.Instance.GetPointsObject(100, true), roomController);
                    }
                }
            }

            if (!Singleton<EndlessZoneManager>.Instance.mapSaveData.empty)
            {
                for (int x = 0; x < ec.map.size.x; x++)
                {
                    for (int z = 0; z < ec.map.size.z; z++)
                    {
                        int index = x * ec.map.size.z + z;

                        if (Singleton<EndlessZoneManager>.Instance.mapSaveData.foundedTiles[index])
                            ec.map.Find(x, z, ec.cells[x, z].ConstBin, ec.cells[x, z].room);
                    }
                }

                ec.map.LoadMarkers(Singleton<EndlessZoneManager>.Instance.mapSaveData.markers.Select(x => x.position).ToList(), Singleton<EndlessZoneManager>.Instance.mapSaveData.markers.Select(x => x.id).ToList());
                Singleton<EndlessZoneManager>.Instance.mapSaveData.empty = true;
            }
        }

        public override void LoadNextLevel()
        {
            Singleton<HighlightManager>.Instance.Highlight("steam_completed", Singleton<LocalizationManager>.Instance.GetLocalizedText("Steam_Highlight_Win"), string.Format(Singleton<LocalizationManager>.Instance.GetLocalizedText("Steam_Highlight_Win_Desc"), base.CurrentLevel + 1), 2u, 0f, 0f, TimelineEventClipPriority.Standard);
            int num = 0;
            if (ec.GetBaldi() != null)
            {
                num = ec.NavigableDistance(ec.CellFromPosition(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position), ec.CellFromPosition(ec.GetBaldi().transform.position), PathType.Nav);
                if (num < 0)
                {
                    num = ec.NavigableDistance(ec.CellFromPosition(Singleton<CoreGameManager>.Instance.GetPlayer(0).transform.position), ec.CellFromPosition(ec.GetBaldi().transform.position), PathType.Const) * 2;
                    if (num < 0)
                        num = 100;
                }
            }

            int stickerBonuses = Singleton<CoreGameManager>.Instance.GetStickerBonuses(ec.RemainingTime, num, ec.map.PlayerDiscoveredCells);
            Singleton<CoreGameManager>.Instance.AddPoints(stickerBonuses, 0, playAnimation: false, includeInLevelTotal: false, multiply: true);
            Singleton<CoreGameManager>.Instance.saveMapAvailable = false;
            Singleton<CoreGameManager>.Instance.saveMapPurchased = false;

            int points = Singleton<CoreGameManager>.Instance.GetPointsThisLevel(0);
            float multiplier = Singleton<EndlessZoneManager>.Instance.zoneRules[0].powerBonus;

            int bonus = Mathf.FloorToInt(points * multiplier);

            Singleton<CoreGameManager>.Instance.AddPoints(bonus - points, 0, playAnimation: false, includeInLevelTotal: false, multiply: true);
            Singleton<EndlessZoneManager>.Instance.AdvanceZone();

            PrepareToLoad();
            elevatorScreen = Instantiate(elevatorScreenPre);
            DontDestroyOnLoad(elevatorScreen);
            elevatorScreen.OnLoadReady += LoadNextLevelAfterElevator;
            elevatorScreen.Initialize();

            elevatorScreen.ShowResults(time, Mathf.RoundToInt(stickerBonuses * Singleton<CoreGameManager>.Instance.YtpMultiplier));
        }

        private void LoadNextLevelAfterElevator()
        {
            StopAllCoroutines();
            ec.gameObject.SetActive(value: false);
            PrepareToLoad();
            Singleton<CoreGameManager>.Instance.PrepareForReload();
            if (Singleton<CoreGameManager>.Instance.sceneObject.levelNo > Singleton<CoreGameManager>.Instance.lastLevelNumber)
            {
                Singleton<CoreGameManager>.Instance.tripPlayed = false;
            }

            if (Singleton<CoreGameManager>.Instance.currentMode == Mode.Main)
            {
                foreach (NPC item in ec.npcsToSpawn)
                {
                    Singleton<PlayerFileManager>.Instance.Find(Singleton<PlayerFileManager>.Instance.foundChars, (int)item.Character);
                }

                foreach (Obstacle obstacle in ec.obstacles)
                {
                    Singleton<PlayerFileManager>.Instance.Find(Singleton<PlayerFileManager>.Instance.foundObstcls, (int)obstacle);
                }

                Singleton<PlayerFileManager>.Instance.Find(Singleton<PlayerFileManager>.Instance.clearedLevels, levelNo);
            }

            Singleton<SubtitleManager>.Instance.DestroyAll();
            LoadSceneObject(Singleton<CoreGameManager>.Instance.sceneObject.nextLevel);
        }

        public override void PrepareLevelGenerationData()
        {
            base.PrepareLevelGenerationData();

            levelObject.forcedStructures = new StructureWithParameters[]
            {
                new StructureWithParameters
                {
                    prefab = Plugin.assetMan.Get<StructureBuilder>("ZoneRuleDummyStructure"),
                    parameters = new(),
                }
            }.Concat(levelObject.forcedStructures).ToArray();

            foreach (ZoneRule rule in Singleton<EndlessZoneManager>.Instance.zoneRules)
                rule.ModifyLevelObject(levelObject, Singleton<EndlessZoneManager>.Instance.currentLevelData.levelSize);
        }

        public override void BeginSpoopMode()
        {
            base.BeginSpoopMode();

            foreach (ZoneRule rule in Singleton<EndlessZoneManager>.Instance.zoneRules)
                rule.OnSpoopModeBegin(ec);
        }

        protected override void VirtualUpdate()
        {
            base.VirtualUpdate();

            if (playStarted)
            {
                foreach (ZoneRule rule in Singleton<EndlessZoneManager>.Instance.zoneRules)
                    rule.OnGameUpdate(this);
            }
        }

        protected override void AllNotebooks()
        {
            if (!allNotebooksFound)
            {
                allNotebooksFound = true;
                ec.ElevatorManager.SetTotalOutOfOrderElevators(ec.Elevators.Count - 1);
                ec.ElevatorManager.SetAllElevators(ElevatorState.OpenForExit);
            }

            foreach (Activity activity in ec.activities)
            {
                if (activity != lastActivity)
                {
                    activity.Corrupt(val: false);
                    activity.SetBonusMode(val: true);
                }
            }

            if (!ec.timeOut)
                Singleton<MusicManager>.Instance.PlayMidi("Level_1_End", true);
        }

        public override void EndGame(Transform player, Baldi baldi)
        {
            List<Vector2> positions = new List<Vector2>();
            List<int> ids = new List<int>();

            ec.map.SaveMarkers(positions, ids);

            Singleton<EndlessZoneManager>.Instance.mapSaveData = new SaveSystem.MapSaveData
            {
                mapAvaliable = Singleton<CoreGameManager>.Instance.saveMapAvailable,
                buyedMap = Singleton<CoreGameManager>.Instance.saveMapPurchased,
                foundedTiles = ec.map.foundTiles.ConvertTo1d(ec.map.size.x, ec.map.size.z),
                mapSize = ec.map.size,
                markers = positions.Select(x => new SaveSystem.MapMarkerData(x, ids[positions.IndexOf(x)])).ToList(),
                empty = false
            };

            base.EndGame(player, baldi);
        }

        public override void AngerBaldi(float val)
        {
            if (bookMultiplier == 0)
            {
                base.AngerBaldi(val);
                return;
            }

            base.AngerBaldi(val * bookMultiplier);
        }

        public float bookMultiplier;
    }
}
