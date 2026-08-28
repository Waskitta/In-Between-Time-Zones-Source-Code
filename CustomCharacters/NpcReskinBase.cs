using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Reflection;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BaldiPlusRandomZone.CustomCharacters
{
    public class NpcReskinBase : MonoBehaviour
    {
        public virtual void Initialize(NPC npc)
        {
            this.npc = npc;

            foreach (AudioManager audMan in npc.GetComponents<AudioManager>())
                audMan.pitchModifier = voicePitch;   
        }

        private void LateUpdate()
        {
            VirtualUpdate();
        }

        public virtual void SetupPrefab() 
        {

        }

        public virtual void OnNpcInitialize()
        {

        }

        public virtual void OnEntityEnter(Entity entity, bool validColision)
        {

        }

        public virtual void OnEntityStay(Entity entity, bool validColision)
        {

        }

        public virtual void OnEntityExit(Entity entity, bool validColision)
        {

        }

        public virtual void VirtualUpdate()
        {

        }

        public void SetPoster(PosterObject poster) => _poster.SetValue(npc, poster);

        public NPC npc;
        public virtual float voicePitch => 1f;
        readonly static FieldInfo _poster = AccessTools.Field(typeof(NPC), "poster");
    }

    public class PrincipalReskin : NpcReskinBase
    {
        public Principal principal;

        public override void Initialize(NPC npc)
        {
            base.Initialize(npc);
            principal = npc.GetComponent<Principal>();
            SetupPrefab();
        }

        public virtual void SendToDetention(bool validColision)
        {

        }
    }

    public class PlaytimeReskin : NpcReskinBase
    {
        public Playtime playtime;
        public Jumprope jumpropePrefab;
        public SpriteOverrider overrider;

        public override void Initialize(NPC npc)
        {
            base.Initialize(npc);
            playtime = npc.GetComponent<Playtime>();
            var jumpropePre = (Jumprope)playtime.ReflectionGetVariable("jumpropePre");
            var jumprope = Instantiate<Jumprope>(jumpropePre);
            jumprope.name = "CustomJumprope";
            jumprope.gameObject.ConvertToPrefab(true);
            jumpropePrefab = jumprope;
            SetupPrefab();
            playtime.spriteRenderer[0].sprite = spritesheet[0];
            playtime.ReflectionSetVariable("jumpropePre", jumpropePrefab);
        }

        public override void OnNpcInitialize()
        {
            base.OnNpcInitialize();
            originalSprites = AssetFinder.FindAllOfType<Sprite>(true).Where(x => x.name.StartsWith("Playtime_")).OrderBy(x => int.Parse(x.name.Replace("Playtime_", ""))).ToArray();
            overrider = playtime.gameObject.AddComponent<SpriteOverrider>();
            overrider.overriderMaps = [new(originalSprites, spritesheet)];
            overrider.renderer = playtime.spriteRenderer[0];
            playtime.spriteRenderer[0].sprite = spritesheet[0];
        }

        public override void VirtualUpdate()
        {
            base.VirtualUpdate();

            if (overrider != null)
                overrider.SetSprite();
        }

        public Sprite[] spritesheet = new Sprite[8];
        private Sprite[] originalSprites;
    }

    public class SweepReskin : NpcReskinBase
    {
        public GottaSweep sweep;

        public override void Initialize(NPC npc)
        {
            base.Initialize(npc);
            sweep = npc.GetComponent<GottaSweep>();
            SetupPrefab();
        }
    }

    public class MsPompReskin : NpcReskinBase
    {
        public NoLateTeacher pomp;
        public NoLateIcon icon;

        public override void Initialize(NPC npc)
        {
            base.Initialize(npc);
            pomp = npc.GetComponent<NoLateTeacher>();
            var iconPrefab = (NoLateIcon)pomp.ReflectionGetVariable("mapIconPre");
            var icon = Instantiate(iconPrefab);
            icon.name = "CustomPompIcon";
            icon.gameObject.ConvertToPrefab(true);
            this.icon = icon;

            SetupPrefab();
            pomp.ReflectionSetVariable("mapIconPre", icon);
        }
    }

    public class BullyReskin : NpcReskinBase
    {
        public Bully bully;

        public override void Initialize(NPC npc)
        {
            base.Initialize(npc);
            bully = npc.GetComponent<Bully>();
            SetupPrefab();
        }
    }

    public class CloudyReskin : NpcReskinBase
    {
        public Cumulo cloudy;

        public override void Initialize(NPC npc)
        {
            base.Initialize(npc);
            cloudy = npc.GetComponent<Cumulo>();
            SetupPrefab();
        }
    }

    public class BeansReskin : NpcReskinBase
    {
        public Beans beans;
        public Gum gum;
        public SpriteOverrider overrider;

        public override void Initialize(NPC npc)
        {
            base.Initialize(npc);
            beans = npc.GetComponent<Beans>();
            originalSprites = AssetFinder.FindAllOfType<Sprite>(true).Where(x => x.name.StartsWith("Beans_SpriteSheet_")).OrderBy(x => int.Parse(x.name.Replace("Beans_SpriteSheet_", ""))).ToArray();

            var gumPrefab = (Gum)beans.ReflectionGetVariable("gumPre");
            var gum = Instantiate(gumPrefab);
            gum.gameObject.ConvertToPrefab(true);
            gumPrefab.name = "CustomGum";
            this.gum = gum;

            SetupPrefab();
            beans.spriteRenderer[0].sprite = spritesheet[1];
            beans.ReflectionSetVariable("gumPre", this.gum);
        }

        public override void OnNpcInitialize()
        {
            base.OnNpcInitialize();
            overrider = beans.gameObject.AddComponent<SpriteOverrider>();
            overrider.overriderMaps = [new(originalSprites, spritesheet)];
            overrider.renderer = beans.spriteRenderer[0];
            beans.spriteRenderer[0].sprite = spritesheet[0];
        }

        public override void VirtualUpdate()
        {
            base.VirtualUpdate();

            if (overrider != null)
                overrider.SetSprite();
        }

        public Sprite[] originalSprites;
        public Sprite[] spritesheet = new Sprite[31];
    }

    public class ChalklesReskin : NpcReskinBase
    {
        public ChalkFace chalkles;

        public override void Initialize(NPC npc)
        {
            base.Initialize(npc);
            chalkles = npc.GetComponent<ChalkFace>();
            SetupPrefab();
        }
    }
}
