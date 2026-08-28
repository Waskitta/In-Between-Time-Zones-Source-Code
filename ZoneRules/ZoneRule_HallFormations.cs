using System;
using System.Linq;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_HallFormations : ZoneRule
    {
        public ZoneRule_HallFormations(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight) { }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            if (type == ZoneRuleType.Positive)
            {
                level.minPostPlotSpecialHalls *= 2;
                level.maxPostPlotSpecialHalls *= 2;
                level.postPlotSpecialHallChance *= 2;
                level.potentialPostPlotSpecialHalls = Plugin.assetPlusMan.GetAll<RoomAsset>().Where(x => x.category == RoomCategory.Hall && x.type == RoomType.Hall).Select(x => new WeightedRoomAsset { selection = x, weight = 100 }).ToArray();
                return;
            }

            level.minPostPlotSpecialHalls = 0;
            level.maxPostPlotSpecialHalls = 0;
            level.postPlotSpecialHallChance = 0;
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            type = ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.HallFormationsValue;
        public override float positivePowerBonus => 1.22f;
        public override float negativePowerBonus => 1.05f;
    }
}
