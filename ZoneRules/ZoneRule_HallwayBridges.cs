using System;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_HallwayBridges : ZoneRule
    {
        public ZoneRule_HallwayBridges(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight) { }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            if (type == ZoneRuleType.Positive)
            {
                level.minReplacementHalls *= 12;
                level.maxReplacementHalls *= 12;
                level.minHallsToRemove = 0;
                level.maxHallsToRemove = 0;
            }
            else
            {
                level.minReplacementHalls = 0;
                level.maxReplacementHalls = 0;
            }
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            type = ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.HallwayBridges;
        public override float positivePowerBonus => 1.15f;
        public override float negativePowerBonus => 1.2f;
    }
}
