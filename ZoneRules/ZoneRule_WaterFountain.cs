using System;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_WaterFountain : ZoneRule
    {
        public ZoneRule_WaterFountain(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight) { }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            foreach (StructureWithParameters structure in level.forcedStructures)
            {
                if (structure.prefab is Structure_EnvironmentObjectPlacer)
                {
                    for (int i = 0; i < structure.parameters.prefab.Length; i++)
                    {
                        if (structure.parameters.prefab[i].selection.GetComponent<WaterFountain>())
                        {
                            if (type == ZoneRuleType.Positive)
                                structure.parameters.minMax[i] = new(structure.parameters.minMax[i].x * 2, structure.parameters.minMax[i].x * 2);
                            else
                                structure.parameters.minMax[i] = new(0, 0);
                        }
                    }
                }
            }
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.WaterFountain;
        public override float positivePowerBonus => 1.15f;
        public override float negativePowerBonus => 1.75f;
    }
}
