using MTM101BaldAPI.Registers;
using System;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_ValuablePoints : ZoneRule
    {
        public ZoneRule_ValuablePoints(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight) { }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            foreach (WeightedItemObject item in level.potentialItems)
            {
                if (item.selection.itemType != Items.Points && type == ZoneRuleType.Positive)
                    item.selection = ItemMetaStorage.Instance.GetPointsObject(item.selection.price / 10, false);
                else if (item.selection.itemType == Items.Points && type == ZoneRuleType.Negative)
                    item.weight = 0;
            }

            for (int i = level.forcedItems.Count - 1; i >= 0; i--)
            {
                if (level.forcedItems[i].itemType != Items.Points && type == ZoneRuleType.Positive)  
                    level.forcedItems[i] = ItemMetaStorage.Instance.GetPointsObject(level.forcedItems[i].price / 10, false);
                else if (level.forcedItems[i].itemType == Items.Points && type == ZoneRuleType.Negative)
                    level.forcedItems.RemoveAt(i);
            }
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.ValuablePoints;
        public override float positivePowerBonus => 1.25f;
        public override float negativePowerBonus => 1.18f;
    }
}
