using MTM101BaldAPI;
using MTM101BaldAPI.Reflection;
using MTM101BaldAPI.Registers;
using System.Collections.Generic;
using UnityEngine;

namespace BaldiPlusRandomZone.CustomCharacters
{
    public static class CharacterCreator
    {
        public static void LoadAllNPCs()
        {
            var bussinesMan = CreateCharacterDuplicate<Principal, Reskin_Businessman>((Principal)NPCMetaStorage.Instance.Get(Character.Principal).value, "BussinesMan");
            replacementCharacters.Add(new(Character.Principal, bussinesMan, 45, "BussinesMan"));

            var educatedTime = CreateCharacterDuplicate<Playtime, Reskin_EducatedTime>((Playtime)NPCMetaStorage.Instance.Get(Character.Playtime).value, "EducatedTime");
            replacementCharacters.Add(new(Character.Playtime, educatedTime, 60, "EducatedTime"));

            var classicSweep = CreateCharacterDuplicate<GottaSweep, Reskin_ClassicSweep>((GottaSweep)NPCMetaStorage.Instance.Get(Character.Sweep).value, "ClassicSweep");
            replacementCharacters.Add(new(Character.Sweep, classicSweep, 50, "ClassicSweep"));

            var caffeinePomp = CreateCharacterDuplicate<NoLateTeacher, Reskin_CaffeinatedPomp>((NoLateTeacher)NPCMetaStorage.Instance.Get(Character.Pomp).value, "CaffeinePomp");
            replacementCharacters.Add(new(Character.Pomp, caffeinePomp, 50, "CaffeinePomp"));

            var goodGuy = CreateCharacterDuplicate<Bully, Reskin_GoodGuy>((Bully)NPCMetaStorage.Instance.Get(Character.Bully).value, "GoodGuy");
            replacementCharacters.Add(new(Character.Bully, goodGuy, 40, "GoodGuy"));

            var cloudya = CreateCharacterDuplicate<Cumulo, Reskin_Cloudya>((Cumulo)NPCMetaStorage.Instance.Get(Character.Cumulo).value, "Cloudya");
            replacementCharacters.Add(new(Character.Cumulo, cloudya, 60, "Cloudya"));

            var coolBeans = CreateCharacterDuplicate<Beans, Reskin_CoolBeans>((Beans)NPCMetaStorage.Instance.Get(Character.Beans).value, "CoolBeans");
            replacementCharacters.Add(new(Character.Beans, coolBeans, 30, "CoolBeans"));

            var chalkDemon = CreateCharacterDuplicate<ChalkFace, Reskin_ChalkDemon>((ChalkFace)NPCMetaStorage.Instance.Get(Character.Chalkles).value, "Chalk-Demon");
            replacementCharacters.Add(new(Character.Chalkles, chalkDemon, 35, "ChalkDemon"));
        }

        internal static T CreateCharacterDuplicate<T, R>(T toClone, string name) where T : NPC where R : NpcReskinBase
        {
            GameObject dummyObj = new GameObject();
            dummyObj.SetActive(false);
            T newNpc = GameObject.Instantiate<T>(toClone, dummyObj.transform);
            newNpc.gameObject.ConvertToPrefab(true);
            newNpc.name = name;

            Character metaEnum = EnumExtensions.ExtendEnum<Character>(name);
            newNpc.ReflectionSetVariable("character", metaEnum);
            newNpc.AddMeta(Plugin.instance, toClone.GetMeta().flags);
            newNpc.GetMeta().tags.Add("custom_mode_force_npc_in_list");

            var reskin = newNpc.gameObject.AddComponent<R>();
            reskin.Initialize(newNpc);

            newNpc.GetMeta().nameLocalizationKey = reskin.npc.Poster.textData[0].textKey;

            Object.Destroy(dummyObj);
            return newNpc;
        }

        public static List<ReplacementCharacter> replacementCharacters = new List<ReplacementCharacter>();
    }

    public class ReplacementCharacter
    {
        public ReplacementCharacter(Character npc, NPC replacement, int weight, string name)
        {
            this.npc = npc;
            this.replacement = replacement;
            this.weight = weight;
            this.name = name;
        }

        public Character npc;
        public NPC replacement;
        public int weight;
        public string name;
    }
}
