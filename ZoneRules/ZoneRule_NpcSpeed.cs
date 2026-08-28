using BaldiPlusRandomZone.EndlessSupport;
using System.Collections.Generic;
using UnityEngine;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_NpcSpeed : ZoneRule
    {
        public ZoneRule_NpcSpeed(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight) { }

        public override void Initialize(BasicZoneManager gameManager, EnvironmentController ec)
        {
            base.Initialize(gameManager, ec);
            affectedNPCs.Clear();
        }

        public override void OnGameUpdate(BaseGameManager gameManager)
        {
            base.OnGameUpdate(gameManager);

            foreach (NPC npc in gameManager.Ec.Npcs.ToArray())
            {
                if (!affectedNPCs.Contains(npc))
                {
                    npc?.Navigator?.Am?.moveMods.Add(movMod);
                    affectedNPCs.Add(npc);
                }
            }
        }

        public override void LoadPreparation(System.Random random)
        {
            base.LoadPreparation(random);
            type = ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.NpcSpeed;
        public override float positivePowerBonus => 1.15f;
        public override float negativePowerBonus => 1.75f;

        public HashSet<NPC> affectedNPCs = new HashSet<NPC>();
        public MovementModifier movMod => new(Vector3.zero, type == ZoneRuleType.Negative ? 1.5f : 0.5f);
    }
}
