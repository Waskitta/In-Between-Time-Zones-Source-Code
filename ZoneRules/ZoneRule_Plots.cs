using System;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_Plots : ZoneRule
    {
        public ZoneRule_Plots(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight)
        {
        }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            if (type == ZoneRuleType.Negative)
            {
                level.minPlots /= 3;
                level.maxPlots /= 3;
            }
            else
            {
                level.minPlots *= 3;
                level.maxPlots *= 3;
            }
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            type = ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.Plots;
        public override float positivePowerBonus => 1.25f;
        public override float negativePowerBonus => 1.4f;
    }
}
