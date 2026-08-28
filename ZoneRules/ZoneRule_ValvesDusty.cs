using HarmonyLib;
using System;
using System.Linq;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_ValvesDusty : ZoneRule
    {
        public ZoneRule_ValvesDusty(int weight) : base(weight) { }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);
            level.potentialStructures = level.potentialStructures.Where(x => x.selection.prefab.GetType() != typeof(Structure_SteamValves)).ToArray();

            level.forcedStructures = level.forcedStructures.AddRangeToArray([new StructureWithParameters {
                prefab = Plugin.assetPlusMan.Get<StructureBuilder>("SteamValveConstructor"),
                parameters = new StructureParameters{
                    minMax = [new(2 + levelId, 4 + (2 * levelId)), new(4 + (2 * levelId), 6 + (2 * levelId))],
                    chance = [0.8f]
                }
            }]);
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            type = ZoneRuleType.Maybe;
            weight = positiveWeight;
            powerBonus = positivePowerBonus;
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.ValvesDusty;
        public override float positivePowerBonus => 1.5f;
    }
}
