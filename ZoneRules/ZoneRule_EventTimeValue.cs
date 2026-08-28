using System;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_EventTimeValue : ZoneRule
    {
        public ZoneRule_EventTimeValue(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight)
        {
        }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            if (type == ZoneRuleType.Negative)
            {
                level.initialEventGap /= 3f;
                level.minEventGap /= 3f;
                level.maxEventGap /= 3f;
                return;
            }

            level.initialEventGap *= 1.4f;
            level.minEventGap *= 1.4f;
            level.maxEventGap *= 1.4f;
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            type = ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.EventTimeValue;
        public override float positivePowerBonus => 1.1f;
        public override float negativePowerBonus => 1.6f;
    }
}
