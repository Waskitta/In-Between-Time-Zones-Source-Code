using System;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_StickyRooms : ZoneRule
    {
        public ZoneRule_StickyRooms(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight)
        {
        }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            foreach (RoomGroup group in level.roomGroup)
            {
                if (type == ZoneRuleType.Negative)
                    group.stickToHallChance = 0f;
                else
                    group.stickToHallChance = 1f;
            }
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            type = ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.StickyRooms;
        public override float negativePowerBonus => 1.35f;
        public override float positivePowerBonus => 1.1f;
    }
}
