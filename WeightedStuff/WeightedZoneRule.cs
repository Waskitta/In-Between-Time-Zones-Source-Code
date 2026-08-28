using BaldiPlusRandomZone.ZoneRules;
using System.Collections.Generic;

namespace BaldiPlusRandomZone.WeightedStuff
{
    public class WeightedZoneRule : WeightedSelection<ZoneRule>
    {
        public WeightedZoneRule(ZoneRule rule, int weight)
        {
            this.selection = rule;
            this.weight = weight;
        }

        public static List<WeightedSelection<ZoneRule>> Convert(List<WeightedZoneRule> list)
        {
            List<WeightedSelection<ZoneRule>> list2 = new List<WeightedSelection<ZoneRule>>();
            foreach (WeightedZoneRule item in list)
            {
                list2.Add(item);
            }
            return list2;
        }
    }
}
