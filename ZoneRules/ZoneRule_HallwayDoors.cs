using System;
using System.Linq;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_HallwayDoors : ZoneRule
    {
        public ZoneRule_HallwayDoors(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight) { }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            foreach (StructureWithParameters structure in level.forcedStructures)
            {
                if (structure.prefab is Structure_HallDoor && structure.parameters.minMax.Length > 1)
                    structure.parameters.minMax[1] = type == ZoneRuleType.Negative ? new(structure.parameters.minMax[1].x * 6, structure.parameters.minMax[1].z * 6) : new(0, 0);
            }

            foreach (StructureWithParameters structure in level.potentialStructures.Select(x => x.selection))
            {
                if (structure.prefab is Structure_HallDoor && structure.parameters.minMax.Length > 1)
                    structure.parameters.minMax[1] = type == ZoneRuleType.Negative ? new(structure.parameters.minMax[1].x * 6, structure.parameters.minMax[1].z * 6) : new(0, 0);
            }
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            type = ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.HallwayDoors;
        public override float positivePowerBonus => 1.05f;
        public override float negativePowerBonus => 1.25f;
    }
}
