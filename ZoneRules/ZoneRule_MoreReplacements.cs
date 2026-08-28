using BaldiPlusRandomZone.CustomCharacters;
using BaldiPlusRandomZone.EndlessSupport;
using MTM101BaldAPI.Registers;
using System;
using System.Linq;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_MoreReplacements : ZoneRule
    {
        public ZoneRule_MoreReplacements(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight) { }

        public override void ModifySceneObject(SceneObject level)
        {
            base.ModifySceneObject(level);

            for (int i = 0; i < level.potentialNPCs.Count; i++)
            {
                if (type == ZoneRuleType.Positive)
                {
                    if (!IsReplacement(level.potentialNPCs[i].selection)) continue;

                    ReplacementCharacter replacement = CharacterCreator.replacementCharacters.FirstOrDefault(x => x.replacement == level.potentialNPCs[i].selection);
                    level.potentialNPCs[i].selection = EndlessZoneManager.potentialNPCs[replacement.npc][0].selection;
                    return;
                }

                if (IsReplacement(level.potentialNPCs[i].selection)) continue;
                if (!HasReplacement(level.potentialNPCs[i].selection)) continue;

                level.potentialNPCs[i].selection = Singleton<EndlessZoneManager>.Instance.GetReplacementCharacter(level.potentialNPCs[i].selection.Character, new(Singleton<CoreGameManager>.Instance.Seed() + Singleton<EndlessZoneManager>.Instance.currentZone));
            }

            for (int i = 0; i < level.forcedNpcs.Length; i++)
            {
                if (type == ZoneRuleType.Positive)
                {
                    if (!IsReplacement(level.forcedNpcs[i])) continue;

                    ReplacementCharacter replacement = CharacterCreator.replacementCharacters.FirstOrDefault(x => x.replacement == level.potentialNPCs[i].selection);
                    level.forcedNpcs[i] = NPCMetaStorage.Instance.Get(replacement.npc).value;
                    return;
                }

                if (IsReplacement(level.forcedNpcs[i])) continue;
                if (!HasReplacement(level.forcedNpcs[i])) continue;

                level.forcedNpcs[i] = Singleton<EndlessZoneManager>.Instance.GetReplacementCharacter(level.forcedNpcs[i].Character, new(Singleton<CoreGameManager>.Instance.Seed() + Singleton<EndlessZoneManager>.Instance.currentZone));
            }
        }

        public bool IsReplacement(NPC npc) => CharacterCreator.replacementCharacters.Any(x => x.replacement == npc);
        public bool HasReplacement(NPC npc) => CharacterCreator.replacementCharacters.Any(x => x.npc == npc.Character);

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            ChoseRandomRuleType(random);
        }

        public Random random;

        public override ZoneRuleCategory category => ZoneRuleCategory.MoreReplacements;
        public override float positivePowerBonus => 1.2f;
        public override float negativePowerBonus => 1.7f;
    }
}
