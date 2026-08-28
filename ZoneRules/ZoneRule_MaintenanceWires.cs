using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_MaintenanceWires : ZoneRule
    {
        public ZoneRule_MaintenanceWires(int negativeWeight) : base (0, negativeWeight) { }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);
            level.type = LevelType.Maintenance;
            level.potentialStructures = level.potentialStructures.Where(x => x.selection.prefab.GetType() != typeof(Structure_PowerLever)).ToArray();
            level.potentialStructures = level.potentialStructures.Where(x => x.selection.prefab.GetType() != typeof(Structure_SteamValves)).ToArray();
            level.potentialStructures = level.potentialStructures.Where(x => x.selection.prefab.GetType() != typeof(Structure_Vent)).ToArray();

            level.minSpecialRooms = 1;
            level.maxSpecialRooms = level.minSpecialRooms;
            level.potentialSpecialRooms = [new WeightedRoomAsset { selection = Plugin.assetPlusMan.Get<RoomAsset>("Room_LightbulbTesting_0") }];
            level.hallLights = [new WeightedTransform { selection = Plugin.assetPlusMan.Get<Transform>("CagedLight") }];
            level.standardLightColor = new(0.8774f, 0.818f, 0.4759f, 1f);
            level.maxLightDistance--;
            level.standardLightStrength = 3 + (levelId * 2);

            level.forcedStructures = level.forcedStructures.AddRangeToArray([
                new StructureWithParameters {
                    prefab = Plugin.assetPlusMan.Get<StructureBuilder>("PowerLeverConstructor"),
                    parameters = new StructureParameters
                    {
                        minMax = new IntVector2[]
                        {
                            new(5, 5),
                            new(1, Mathf.Clamp(levelId, 1, 2)),
                            new(levelId == 0 ? 2 : 3, levelId == 0 ? 2 : 3),
                            new(0, 30 + (levelId * 10)),
                            new(1 + levelId, 1 + (levelId * 2))
                        },
                        chance = [levelId == 0 ? 0 : 0.12f]
                    }
                },
                new StructureWithParameters
                {
                    prefab = Plugin.assetPlusMan.Get<StructureBuilder>("SteamValveConstructor"),
                    parameters = new StructureParameters
                    {
                        minMax = new IntVector2[]
                        {
                            new(levelId * 2, (levelId * 2) + 2),
                            new(3, 6 + Mathf.Min(levelId, 8))
                        },
                        chance = [0.5f]
                    }
                },
            new StructureWithParameters
            {
                prefab = Plugin.assetPlusMan.Get<StructureBuilder>("Structure_Vent"),
                parameters = new StructureParameters
                {
                    minMax = new IntVector2[]
                    {
                        new(levelId == 0 ? 1 : 2 + Mathf.Min(levelId, 1), levelId == 0 ? 1 : 3 + levelId),
                        new(2, 5),
                        new(20, 0)
                    }
                }
            }]);

            foreach (RoomGroup group in level.roomGroup)
                group.light = level.hallLights;
        }

        public override void LoadPreparation(System.Random random)
        {
            base.LoadPreparation(random);
            SetRuleType(ZoneRuleType.Negative);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.MaintenanceWires;
        public override float negativePowerBonus => 2.25f;
    }
}
