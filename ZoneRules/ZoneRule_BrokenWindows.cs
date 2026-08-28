using BaldiPlusRandomZone.EndlessSupport;
using UnityEngine;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_BrokenWindows : ZoneRule
    {
        public ZoneRule_BrokenWindows(int weight) : base(weight) { }

        public override void Initialize(BasicZoneManager gameManager, EnvironmentController ec)
        {
            base.Initialize(gameManager, ec);

            Window[] windows = Object.FindObjectsOfType<Window>();

            foreach (Window window in windows)
                window.Break(false);
        }

        public override void LoadPreparation(System.Random random)
        {
            base.LoadPreparation(random);
            type = ZoneRuleType.Maybe;
            weight = positiveWeight;
            powerBonus = negativePowerBonus;
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.BrokenWindows;
        public override float negativePowerBonus => 1.4f;
    }
}
