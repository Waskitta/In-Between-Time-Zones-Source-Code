using UnityEngine;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_LightValue : ZoneRule
    {
        public ZoneRule_LightValue(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight)
        {
        }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            if (type == ZoneRuleType.Negative)
            {
                level.maxLightDistance = Mathf.FloorToInt(level.standardLightStrength * 0.45f);
                return;
            }

            level.maxLightDistance = Mathf.FloorToInt(level.standardLightStrength * 2.5f);
        }

        public override void LoadPreparation(System.Random random)
        {
            base.LoadPreparation(random);
            type = ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.LightValue;
        public override float positivePowerBonus => 1.15f;
        public override float negativePowerBonus => 1.25f;
    }
}
