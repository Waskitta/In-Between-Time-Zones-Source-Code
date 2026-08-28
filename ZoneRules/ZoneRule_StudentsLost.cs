using HarmonyLib;
using System;
using System.Linq;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_StudentsLost : ZoneRule
    {
        public ZoneRule_StudentsLost(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight) { }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);
            level.forcedStructures = level.forcedStructures.Where(x => x.prefab.GetType() != typeof(Structure_StudentSpawner)).ToArray();

            level.forcedStructures = level.forcedStructures.AddRangeToArray([new StructureWithParameters {
                prefab = Plugin.assetPlusMan.Get<StructureBuilder>("StudentSpawnerConstructor"),
                parameters = new StructureParameters{
                    minMax = [type == ZoneRuleType.Positive ? new(1 + levelId, 1 + (levelId * 2)) : new(0, 0)]
                }
            }]);
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.StudentsLost;
        public override float positivePowerBonus => 1.1f;
        public override float negativePowerBonus => 1.2f;
    }
}
