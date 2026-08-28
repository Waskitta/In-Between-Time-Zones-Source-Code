using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Reflection;
using UnityEngine;

namespace BaldiPlusRandomZone.CustomCharacters
{
    public class Reskin_CaffeinatedPomp : MsPompReskin
    {
        public override void SetupPrefab()
        {
            base.SetupPrefab();

            Sprite[] sprites = AssetLoader.SpritesFromSpritesheet(2, 1, 26f, Vector2.one / 2, AssetLoader.TextureFromMod(Plugin.instance, "CustomCharacters", "CaffeinePomp.png"));
            SpriteRenderer renderer = pomp.spriteRenderer[0];
            renderer.sprite = sprites[0];

            pomp.ReflectionSetVariable("normalSprite", sprites[0]);
            pomp.ReflectionSetVariable("angrySprite", sprites[1]);
            SetPoster(ObjectCreators.CreateCharacterPoster(AssetLoader.TextureFromMod(Plugin.instance, "CustomCharacters", "pri_caffeinepomp.png"), "PST_PRI_CaffeinePomp1", "PST_PRI_CaffeinePomp2"));

            pomp.ReflectionSetVariable("walkSpeed", 20f);
            pomp.ReflectionSetVariable("runSpeed", 50f);
            pomp.ReflectionSetVariable("angrySpeed", 100f);
            pomp.ReflectionSetVariable("dragSpeed", 75f);
            pomp.ReflectionSetVariable("classTime", 60f);
            pomp.ReflectionSetVariable("teachTime", 1.5f);
            pomp.ReflectionSetVariable("angryTeachTime", 10f);
            pomp.ReflectionSetVariable("resetDelay", 60f);
            pomp.ReflectionSetVariable("successPoints", 7);

            icon.spriteRenderer.sprite = renderer.sprite;
        }

        public override float voicePitch => 1.5f;
    }
}
