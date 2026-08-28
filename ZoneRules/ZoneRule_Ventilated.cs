using HarmonyLib;
using System;
using System.Linq;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_Ventilated : ZoneRule
    {
        public ZoneRule_Ventilated(int positiveWeight) : base(positiveWeight) { }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            level.potentialStructures = level.potentialStructures.Where(x => x.selection.prefab.GetType() != typeof(Structure_Vent)).ToArray();

            level.forcedStructures = level.forcedStructures.AddRangeToArray([new StructureWithParameters {
                prefab = Plugin.assetPlusMan.Get<StructureBuilder>("Structure_Vent"),
                parameters = new StructureParameters
                {
                    minMax = [new(8 + (4 * levelId), 10 + (5 * levelId)), new(2, 5), new(14, 0)],
                    chance = [],
                    prefab = []
                }
            }]);
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            SetRuleType(ZoneRuleType.Positive);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.Ventilated;
        public override float positivePowerBonus => 1.25f;
    }
}
