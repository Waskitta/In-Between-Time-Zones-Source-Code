using UnityEngine;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_MapSize : ZoneRule
    {
        public ZoneRule_MapSize(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight)
        {
        }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            float multiplier = type == ZoneRuleType.Positive ? 0.45f : 1.75f;

            level.minSize = new(Mathf.FloorToInt(level.minSize.x * multiplier), Mathf.FloorToInt(level.minSize.z * multiplier));
            level.maxSize = new(Mathf.FloorToInt(level.maxSize.x * multiplier), Mathf.FloorToInt(level.maxSize.z * multiplier));
        }

        public override void LoadPreparation(System.Random random)
        {
            base.LoadPreparation(random);
            type = ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.MapSize;
        public override float positivePowerBonus => 1.05f;
        public override float negativePowerBonus => 1.82f;
    }
}
