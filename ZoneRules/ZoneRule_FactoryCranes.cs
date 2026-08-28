using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_FactoryCranes : ZoneRule
    {
        public ZoneRule_FactoryCranes(int positiveWeight) : base(positiveWeight)
        {
        }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);
            level.type = LevelType.Factory;
            level.potentialStructures = level.potentialStructures.Where(x => x.selection.prefab.GetType() != typeof(Structure_Rotohalls)).ToArray();
            level.potentialStructures = level.potentialStructures.Where(x => !x.selection.prefab.name.Contains("LockdownDoorConstructor")).ToArray();

            level.minSpecialRooms = 0;
            level.maxSpecialRooms = 0;
            level.forcedStructures = level.forcedStructures.AddRangeToArray([
            new StructureWithParameters { prefab = Plugin.assetPlusMan.Get<StructureBuilder>("ConveyorBeltConstructor"), parameters = new() },
            new StructureWithParameters { prefab = Plugin.assetPlusMan.Get<StructureBuilder>("FactoryBoxConstructor"), parameters = new() },
            new StructureWithParameters {
                prefab = Plugin.assetPlusMan.Get<StructureBuilder>("LockdownDoorConstructor"),
                parameters = new StructureParameters
                {
                    prefab = new WeightedGameObject[]
                    { 
                        new WeightedGameObject { selection = Plugin.assetPlusMan.Get<GameObject>("LockdownDoor_TrapCheck"), weight = 80 },
                        new WeightedGameObject { selection = Plugin.assetPlusMan.Get<GameObject>("LockdownDoor_Shut"), weight = 20 }
                    },
                    minMax = [new(3, 6), new(4, 6)],
                    chance = [0.5f]
                }
            },
            new StructureWithParameters
            {
                prefab = Plugin.assetPlusMan.Get<StructureBuilder>("Rotohall_Structure"),
                parameters = new StructureParameters
                { minMax = [new(1 + levelId, 2 + levelId), new(0, 6)] }
            }]);

            if (levelId == 0)
            {
                level.minSize = new(level.minSize.x + 5, level.minSize.z + 5);
                level.maxSize = new(level.minSize.x + 5, level.minSize.z + 5);
            }

            level.hallCeilingTexs = [new WeightedTexture2D { selection = Plugin.assetPlusMan.Get<Texture2D>("Transparent"), weight = 100 }];
            level.hallLights = [new WeightedTransform { selection = Plugin.assetPlusMan.Get<Transform>("CordedHangingLight"), weight = 100 }];

            foreach (RoomGroup group in level.roomGroup)
                group.light = [new WeightedTransform { selection = Plugin.assetPlusMan.Get<Transform>("HangingLight"), weight = 100 }];
        }

        public override void LoadPreparation(System.Random random)
        {
            base.LoadPreparation(random);
            type = ZoneRuleType.Maybe;
            weight = positiveWeight;
            powerBonus = positivePowerBonus;
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.FactoryCranes;
        public override float positivePowerBonus => 1.45f;
    }
}
