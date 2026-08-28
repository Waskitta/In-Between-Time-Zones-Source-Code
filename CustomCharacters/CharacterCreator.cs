using MTM101BaldAPI;
using MTM101BaldAPI.Registers;
using System.Collections.Generic;
using UnityEngine;

namespace BaldiPlusRandomZone.CustomCharacters
{
    public static class CharacterCreator
    {
        public static void LoadAllNPCs()
        {
            var bussinesMan = CreateCharacterDuplicate((Principal)NPCMetaStorage.Instance.Get(Character.Principal).value, "BussinesMan");
            bussinesMan.gameObject.AddComponent<Reskin_Businessman>().Initialize(bussinesMan);
            replacementCharacters.Add(new(Character.Principal, bussinesMan, 45, "BussinesMan"));

            var educatedTime = CreateCharacterDuplicate((Playtime)NPCMetaStorage.Instance.Get(Character.Playtime).value, "EducatedTime");
            educatedTime.gameObject.AddComponent<Reskin_EducatedTime>().Initialize(educatedTime);
            replacementCharacters.Add(new(Character.Playtime, educatedTime, 60, "EducatedTime"));

            var classicSweep = CreateCharacterDuplicate((GottaSweep)NPCMetaStorage.Instance.Get(Character.Sweep).value, "ClassicSweep");
            classicSweep.gameObject.AddComponent<Reskin_ClassicSweep>().Initialize(classicSweep);
            replacementCharacters.Add(new(Character.Sweep, classicSweep, 50, "ClassicSweep"));

            var caffeinePomp = CreateCharacterDuplicate((NoLateTeacher)NPCMetaStorage.Instance.Get(Character.Pomp).value, "CaffeinePomp");
            caffeinePomp.gameObject.AddComponent<Reskin_CaffeinatedPomp>().Initialize(caffeinePomp);
            replacementCharacters.Add(new(Character.Pomp, caffeinePomp, 50, "CaffeinePomp"));

            var goodGuy = CreateCharacterDuplicate((Bully)NPCMetaStorage.Instance.Get(Character.Bully).value, "GoodGuy");
            goodGuy.gameObject.AddComponent<Reskin_GoodGuy>().Initialize(goodGuy);
            replacementCharacters.Add(new(Character.Bully, goodGuy, 40, "GoodGuy"));

            var cloudya = CreateCharacterDuplicate((Cumulo)NPCMetaStorage.Instance.Get(Character.Cumulo).value, "Cloudya");
            cloudya.gameObject.AddComponent<Reskin_Cloudya>().Initialize(cloudya);
            replacementCharacters.Add(new(Character.Cumulo, cloudya, 60, "Cloudya"));

            var coolBeans = CreateCharacterDuplicate((Beans)NPCMetaStorage.Instance.Get(Character.Beans).value, "CoolBeans");
            coolBeans.gameObject.AddComponent<Reskin_CoolBeans>().Initialize(coolBeans);
            replacementCharacters.Add(new(Character.Beans, coolBeans, 30, "CoolBeans"));

            var chalkDemon = CreateCharacterDuplicate((ChalkFace)NPCMetaStorage.Instance.Get(Character.Chalkles).value, "Chalk-Demon");
            chalkDemon.gameObject.AddComponent<Reskin_ChalkDemon>().Initialize(chalkDemon);
            replacementCharacters.Add(new(Character.Chalkles, chalkDemon, 35, "Chalk-Demon"));
        }

        internal static T CreateCharacterDuplicate<T>(T toClone, string name) where T : NPC
        {
            GameObject dummyObj = new GameObject();
            dummyObj.SetActive(false);
            T newNpc = GameObject.Instantiate<T>(toClone, dummyObj.transform);
            newNpc.gameObject.ConvertToPrefab(true);
            newNpc.name = name;
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
