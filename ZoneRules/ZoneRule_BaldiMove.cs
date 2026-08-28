using MTM101BaldAPI.Reflection;
using System;
using System.Collections;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_BaldiMove : ZoneRule
    {
        public ZoneRule_BaldiMove(int negativeWeight) : base(0, negativeWeight) { }

        public override void OnSpoopModeBegin(EnvironmentController ec)
        {
            base.OnSpoopModeBegin(ec);
            ec.StartCoroutine(WaitSpawn(ec));
        }

        private IEnumerator WaitSpawn(EnvironmentController ec)
        {
            while (true)
            {
                foreach (NPC npc in ec.Npcs.ToArray())
                {
                    if (npc is Baldi baldi)
                    {
                        baldi.ReflectionSetVariable("smoothMove", true);
                        yield break;
                    }
                    yield return null;
                }
            }
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            type = ZoneRuleType.Negative;
            weight = negativeWeight;
            powerBonus = negativePowerBonus;
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.BaldiMove;
        public override float negativePowerBonus => 1.75f;
    }
}
