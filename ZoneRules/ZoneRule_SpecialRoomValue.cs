using System;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_SpecialRoomValue : ZoneRule
    {
        public ZoneRule_SpecialRoomValue(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight)
        {
        }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            if (type == ZoneRuleType.Positive)
            {
                level.minSpecialRooms += 1;
                level.maxSpecialRooms += 1;
                return;
            }

            level.minSpecialRooms = 0;
            level.maxSpecialRooms = 0;
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            type = ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.SpecialRoomValue;
        public override float positivePowerBonus => 1.1f;
        public override float negativePowerBonus => 1.35f;
    }
}
