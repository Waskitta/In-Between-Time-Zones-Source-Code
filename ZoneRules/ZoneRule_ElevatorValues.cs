using UnityEngine;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_ElevatorValues : ZoneRule
    {
        public ZoneRule_ElevatorValues(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight)
        {
        }
        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            if (type == ZoneRuleType.Negative)
            {
                level.exitCount = Mathf.Min(level.exitCount + 1, 3);
                return;
            }

            level.exitCount = 0;
        }

        public override void LoadPreparation(System.Random random)
        {
            base.LoadPreparation(random);
            type = ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.ElevatorValues;

        public override float positivePowerBonus => 1.05f;
        public override float negativePowerBonus => 1.35f;
    }
}
