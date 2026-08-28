using System;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_RoomQuantityValue : ZoneRule
    {
        public ZoneRule_RoomQuantityValue(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight)
        {
        }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            foreach (RoomGroup group in level.roomGroup)
            {
                if (group.name == "Class") continue;

                if (type == ZoneRuleType.Negative)
                {
                    group.minRooms *= 2;
                    group.maxRooms *= 2;
                    continue;
                }

                group.minRooms /= 2;
                group.maxRooms /= 2;
            }
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            type = ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.RoomQuantityValue;
        public override float positivePowerBonus => 1.15f;
        public override float negativePowerBonus => 1.5f;
    }
}
