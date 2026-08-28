using HarmonyLib;
using System;
using UnityEngine;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_LabZones : ZoneRule
    {
        public ZoneRule_LabZones(int positiveWeight) : base(positiveWeight)
        {
        }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);
            level.type = LevelType.Laboratory;
            StructureBuilder structure = Plugin.assetPlusMan.Get<StructureBuilder>("TeleporterRoomConstructor");

            level.minSpecialRooms = 0;
            level.maxSpecialRooms = 0;

            level.forcedStructures = level.forcedStructures.AddRangeToArray([new StructureWithParameters {
                prefab = structure,
                parameters = new()
            }]);

            StructureWithParameters first = level.forcedStructures[0];
            level.forcedStructures[0] = level.forcedStructures[level.forcedStructures.Length - 1];
            level.forcedStructures[level.forcedStructures.Length - 1] = first;

            if (level.minSize.x < 30 || level.minSize.z < 30)
            {
                level.minSize = new(level.minSize.x + 8, level.minSize.z + 8);
                level.maxSize = new(level.minSize.x + 10, level.minSize.z + 10);

                if (levelId == 0)
                {
                    level.minPlots *= 2;
                    level.maxPlots *= 2;
                }
            }

            level.hallLights = [new WeightedTransform { selection = Plugin.assetPlusMan.Get<Transform>("HangingLight"), weight = 100 }];

            foreach (RoomGroup group in level.roomGroup)
                group.light = level.hallLights;
        }

        public override void ModifySceneObject(SceneObject level)
        {
            base.ModifySceneObject(level);
            level.skybox = Plugin.assetPlusMan.Get<Cubemap>("Cubemap_Void");
        }

        public override void LoadPreparation(System.Random random)
        {
            base.LoadPreparation(random);
            type = ZoneRuleType.Maybe;
            weight = positiveWeight;
            powerBonus = positivePowerBonus;
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.LabZones;
        public override float positivePowerBonus => 1.5f;
    }
}
