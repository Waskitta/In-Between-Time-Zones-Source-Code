using System;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_NoMap : ZoneRule
    {
        public ZoneRule_NoMap(int negativeWeight) : base(0, negativeWeight) { }

        public override void ModifySceneObject(SceneObject level)
        {
            base.ModifySceneObject(level);
            level.usesMap = false;
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            type = ZoneRuleType.Negative;
            weight = negativeWeight;
            powerBonus = negativePowerBonus;
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.NoMap;
        public override float negativePowerBonus => 2.1f;
    }
}
