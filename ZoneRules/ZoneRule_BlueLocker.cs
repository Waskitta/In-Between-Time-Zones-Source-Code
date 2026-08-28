using System;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_BlueLocker : ZoneRule
    {
        public ZoneRule_BlueLocker(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight) { }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            foreach (StructureWithParameters structure in level.forcedStructures)
            {
                if (structure.prefab is Structure_Lockers)
                    structure.parameters.chance[0] = type == ZoneRuleType.Positive ? 1f : 0f;
            }
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.BlueLocker;
        public override float positivePowerBonus => 1.05f;
        public override float negativePowerBonus => 1.46f;
    }
}
