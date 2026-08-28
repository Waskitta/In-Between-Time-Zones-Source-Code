using System;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_PotentialNpcValue : ZoneRule
    {
        public ZoneRule_PotentialNpcValue(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight)
        {
        }

        public override void ModifySceneObject(SceneObject level)
        {
            base.ModifySceneObject(level);

            if (type == ZoneRuleType.Negative)
            {
                level.additionalNPCs = level.additionalNPCs * 2;
                return;
            }

            level.additionalNPCs = level.additionalNPCs / 2;
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            type = ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.PotentialNpcValue;
        public override float positivePowerBonus => 1.15f;
        public override float negativePowerBonus => 1.5f;
    }
}
