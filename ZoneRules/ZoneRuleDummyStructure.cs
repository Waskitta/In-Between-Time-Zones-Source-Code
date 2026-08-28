using BaldiPlusRandomZone.EndlessSupport;
using System;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRuleDummyStructure : StructureBuilder
    { 
        public override void Generate(LevelGenerator lg, Random rng)
        {
            base.Generate(lg, rng);

            foreach (ZoneRule rule in Singleton<EndlessZoneManager>.Instance.zoneRules)
                rule.Generate(lg, rng);
        }
    }
}
