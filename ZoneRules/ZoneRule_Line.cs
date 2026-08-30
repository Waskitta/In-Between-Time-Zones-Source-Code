using System;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_Line : ZoneRule
    {
        public ZoneRule_Line(int negativeWeight) : base(0, negativeWeight) { }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);
            int minX = level.minSize.x;
            int maxX = level.maxSize.x;

            level.minSize = new(minX, 1);
            level.maxSize = new(maxX, 1);

            level.minPostPlotSpecialHalls = 0;
            level.maxPostPlotSpecialHalls = 0;
            level.minPrePlotSpecialHalls = 0;
            level.maxPrePlotSpecialHalls = 0;
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            SetRuleType(ZoneRuleType.Negative);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.Line;
        public override float negativePowerBonus => 1.8f;
    }
}
