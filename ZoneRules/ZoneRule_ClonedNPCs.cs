using System;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_ClonedNPCs : ZoneRule
    {
        public ZoneRule_ClonedNPCs(int negativeWeight) : base(0, negativeWeight) { }

        public override void ModifySceneObject(SceneObject level)
        {
            base.ModifySceneObject(level);
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            type = ZoneRuleType.Negative;
            weight = negativeWeight;
            powerBonus = negativePowerBonus;
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.ClonedNPCs;
        public override float negativePowerBonus => 2.25f;
    }
}
