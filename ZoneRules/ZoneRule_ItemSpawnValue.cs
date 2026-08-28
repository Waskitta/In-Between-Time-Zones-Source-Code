using System;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_ItemSpawnValue : ZoneRule
    {
        public ZoneRule_ItemSpawnValue(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight)
        {
        }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            if (type == ZoneRuleType.Positive)
                level.maxItemValue *= 2;    
            else
                level.maxItemValue /= 2;
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            type = ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.ItemSpawnValue;
        public override float positivePowerBonus => 1.05f;
        public override float negativePowerBonus => 1.65f;
    }
}
