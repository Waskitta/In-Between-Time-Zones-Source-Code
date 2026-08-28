using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace BaldiPlusRandomZone.CustomCharacters
{
    public class Reskin_CoolBeans : BeansReskin
    {
        public override void SetupPrefab()
        {
            base.SetupPrefab();
            spritesheet = AssetLoader.SpritesFromSpritesheet(8, 4, 35f, Vector2.one / 2f, AssetLoader.TextureFromMod(Plugin.instance, "CustomCharacters", "CoolBeans_SpriteSheet.png"));
            SetPoster(ObjectCreators.CreateCharacterPoster(AssetLoader.TextureFromMod(Plugin.instance, "CustomCharacters", "pri_coolbeans.png"), "PST_PRI_CoolBeans1", "PST_PRI_CoolBeans2"));

            var flyingRenderer = (GameObject)gum.ReflectionGetVariable("flyingSprite");
            flyingRenderer.GetComponent<SpriteRenderer>().sprite = AssetLoader.SpriteFromMod(Plugin.instance, Vector2.one / 2f, 20, "CustomCharacters", "coolbeans_gumwad.png");

            var groundedSprite = (GameObject)gum.ReflectionGetVariable("groundedSprite");
            groundedSprite.GetComponent<SpriteRenderer>().sprite = AssetLoader.SpriteFromMod(Plugin.instance, Vector2.one / 2f, 20, "CustomCharacters", "coolbeans_enemywad.png");

            var canvas = (Canvas)gum.ReflectionGetVariable("canvas");
            canvas.GetComponentInChildren<Image>().sprite = AssetLoader.SpriteFromMod(Plugin.instance, Vector2.one / 2f, 1, "CustomCharacters", "coolbeans_overlay.png");

            gum.ReflectionSetVariable("gaugeSprite", AssetLoader.SpriteFromMod(Plugin.instance, Vector2.one / 2f, 1, "CustomCharacters", "coolbeans_gum_icon.png"));
            gum.ReflectionSetVariable("moveMod", new MovementModifier(Vector3.zero, 1.15f));
            gum.ReflectionSetVariable("playerMod", new MovementModifier(Vector3.zero, 1.3f));
        }

        public override float voicePitch => 1.25f;
    }
}
