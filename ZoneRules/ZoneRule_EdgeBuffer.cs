using System;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_EdgeBuffer : ZoneRule
    {
        public ZoneRule_EdgeBuffer(int positiveWeight, int negativeWeight): base(positiveWeight, negativeWeight) { }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            if (type == ZoneRuleType.Negative)
                level.outerEdgeBuffer *= 2;
            else
                level.outerEdgeBuffer = 2;
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            type = ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.EdgeBuffer;
        public override float positivePowerBonus => 1.15f;
        public override float negativePowerBonus => 1.2f;
    }
}
