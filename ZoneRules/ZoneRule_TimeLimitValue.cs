using System;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_TimeLimitValue : ZoneRule
    {
        public ZoneRule_TimeLimitValue(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight)
        {
        }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            if (type == ZoneRuleType.Negative)
                level.timeLimit /= 2.5f;
            else
                level.timeOutEvent = null;
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            type = ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.TimeLimitValue;
        public override float positivePowerBonus => 1.05f;
        public override float negativePowerBonus => 1.8f;
    }
}
