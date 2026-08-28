using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Reflection;
using UnityEngine;

namespace BaldiPlusRandomZone.CustomCharacters
{
    public class Reskin_ChalkDemon : ChalklesReskin
    {
        public override void SetupPrefab()
        {
            base.SetupPrefab();

            SpriteRenderer chalkRenderer = (SpriteRenderer)chalkles.ReflectionGetVariable("chalkRenderer");
            chalkRenderer.sprite = AssetLoader.SpriteFromMod(Plugin.instance, new(0.5f, 0.25f), 25f, "CustomCharacters", "DemonChalk.png");

            SpriteRenderer flyingRenderer = (SpriteRenderer)chalkles.ReflectionGetVariable("flyingRenderer");
            flyingRenderer.sprite = chalkRenderer.sprite;

            SetPoster(ObjectCreators.CreateCharacterPoster(AssetLoader.TextureFromMod(Plugin.instance, "CustomCharacters", "pri_demonchalk.png"), "PST_PRI_DemonChalk1", "PST_PRI_DemonChalk2"));

            chalkles.ReflectionSetVariable("unchargeRate", 0.05f);
            chalkles.ReflectionSetVariable("lockTime", 10f);
            chalkles.ReflectionSetVariable("setTime", 6f);
            chalkles.ReflectionSetVariable("acceleration", 20f);
        }

        public override float voicePitch => 0.75f;
    }
}
