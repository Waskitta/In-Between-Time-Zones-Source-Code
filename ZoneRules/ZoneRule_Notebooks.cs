using UnityEngine;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_Notebooks : ZoneRule
    {
        public ZoneRule_Notebooks(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight)
        {
        }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            foreach (RoomGroup group in level.roomGroup)
            {
                if (group.name == "Class")
                {
                    if (type == ZoneRuleType.Positive)
                    {
                        group.minRooms = Mathf.Max(1, group.minRooms / 2);
                        group.maxRooms -= 1;
                    }
                    else
                    {
                        int mediumLevel = group.maxRooms / 2;
                        group.minRooms += 1;
                        group.maxRooms += mediumLevel;
                    }
                }
            }
        }

        public override void LoadPreparation(System.Random random)
        {
            base.LoadPreparation(random);
            type = ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.Notebooks;
        public override float positivePowerBonus => 1.15f;
        public override float negativePowerBonus => 1.75f;
    }
}
