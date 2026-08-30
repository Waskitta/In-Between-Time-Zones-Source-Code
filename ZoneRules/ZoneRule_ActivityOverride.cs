using BaldiPlusRandomZone.EndlessSupport;
using System;
using System.Linq;
using UnityEngine;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_ActivityOverride : ZoneRule
    {
        public ZoneRule_ActivityOverride(ZoneRuleCategory overrideCategory, ZoneRuleType typeOverride, int weight) : base(weight, weight)
        {
            this.overrideCategory = overrideCategory;
            this.typeOverride = typeOverride;

        }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            bool ContainsActivity(Type type, LevelObject level)
            {
                RoomGroup classGroup = level.roomGroup.FirstOrDefault(x => x.name == "Class");

                foreach (WeightedRoomAsset room in classGroup.potentialRooms)
                {
                    if (room.selection.hasActivity && room.selection.activity.prefab.GetType() == type)
                        return true;
                }

                return false;
            }

            LevelObject baseLevelReference = Resources.FindObjectsOfTypeAll<LevelObject>().FirstOrDefault(x => x.type == LevelType.Schoolhouse && ContainsActivity(GetActivityType(), x));
            WeightedRoomAsset[] weightedRooms = baseLevelReference.roomGroup.FirstOrDefault(x => x.name == "Class").potentialRooms.Where(x => x.selection.hasActivity && x.selection.activity.GetType() == GetActivityType()).ToArray();
            RoomAsset[] allRoomsWithActivity = Resources.FindObjectsOfTypeAll<RoomAsset>().Where(x => x.hasActivity && x.activity.prefab.GetType() == GetActivityType()).ToArray();

            foreach (RoomGroup group in level.roomGroup)
            {
                if (group.name == "Class")
                    group.potentialRooms = allRoomsWithActivity.Select(x => Singleton<EndlessZoneManager>.Instance.WeightRoomBasedOnArray(x, weightedRooms)).ToArray();
            }
        }

        public Type GetActivityType()
        {
            switch (overrideCategory)
            {
                case ZoneRuleCategory.MathMachineOverride:
                    return typeof(MathMachine);
                case ZoneRuleCategory.BalloonBusterOverride:
                    return typeof(BalloonBuster);
                case ZoneRuleCategory.MatchMachineOverride:
                    return typeof(MatchActivity);
                default:
                    return typeof(NoActivity);
            }
        }

        public override void LoadPreparation(System.Random random)
        {
            base.LoadPreparation(random);
            SetRuleType(typeOverride);

            switch (overrideCategory)
            {
                case ZoneRuleCategory.NotebookOverride:
                    powerBonus = 1.3f;
                    break;
                case ZoneRuleCategory.MathMachineOverride:
                    powerBonus = 1.25f;
                    break;
                case ZoneRuleCategory.BalloonBusterOverride:
                    powerBonus = 1.4f;
                    break;
                case ZoneRuleCategory.MatchMachineOverride:
                    powerBonus = 1.6f;
                    break;
            }
        }


        public ZoneRuleCategory overrideCategory;
        public ZoneRuleType typeOverride;

        public override ZoneRuleCategory category => overrideCategory;
    }
}
