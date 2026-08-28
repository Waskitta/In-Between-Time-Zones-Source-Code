using BaldiPlusRandomZone.EndlessSupport;
using UnityEngine;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_ColorLights : ZoneRule
    {
        public ZoneRule_ColorLights(int weight) : base(weight) { }

        public override void Initialize(BasicZoneManager gameManager, EnvironmentController ec)
        {
            base.Initialize(gameManager, ec);
            
            foreach (Cell cell in ec.AllCells())
            {
                if (cell.hasLight)
                    cell.lightColor = Random.ColorHSV();

                cell.SetPower(cell.room.Powered);
            }
        }

        public override void LoadPreparation(System.Random random)
        {
            base.LoadPreparation(random);
            type = ZoneRuleType.Maybe;
            weight = positiveWeight;
            powerBonus = positivePowerBonus;
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.ColorLights; 
        public override float positivePowerBonus => 1.15f;
    }
}
