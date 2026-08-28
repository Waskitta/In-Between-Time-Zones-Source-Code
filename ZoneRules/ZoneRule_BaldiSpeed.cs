using BaldiPlusRandomZone.EndlessSupport;
using System;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_BaldiSpeed : ZoneRule
    {
        public ZoneRule_BaldiSpeed(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight) { }

        public override void Initialize(BasicZoneManager gameManager, EnvironmentController ec)
        {
            base.Initialize(gameManager, ec);
            gameManager.bookMultiplier = type == ZoneRuleType.Positive ? 0.4f : 1.6f;
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            type = ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.BaldiSpeed;
        public override float positivePowerBonus => 1.05f;
        public override float negativePowerBonus => 1.85f;
    }
}
