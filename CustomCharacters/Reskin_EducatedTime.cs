using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI;
using UnityEngine;
using MTM101BaldAPI.Reflection;

namespace BaldiPlusRandomZone.CustomCharacters
{
    public class Reskin_EducatedTime :  PlaytimeReskin
    {
        public override void SetupPrefab()
        {
            base.SetupPrefab();
            spritesheet = AssetLoader.SpritesFromSpritesheet(4, 2, 100f, Vector2.one / 2, AssetLoader.TextureFromMod(Plugin.instance, "CustomCharacters", "Educatedtime.png"));
            SetPoster(ObjectCreators.CreateCharacterPoster(AssetLoader.TextureFromMod(Plugin.instance, "CustomCharacters", "pri_educatedtime.png"), "PST_PRI_EducatedTime1", "PST_PRI_EducatedTime2"));
            playtime.looker.distance *= 5;
            jumpropePrefab.ReflectionSetVariable("maxJumps", 3);
            jumpropePrefab.ReflectionSetVariable("startVal", 12);

            foreach (AudioManager audMan in playtime.GetComponentsInChildren<AudioManager>())
            {
                bool loop = (bool)audMan.ReflectionGetVariable("loopOnStart");

                if (loop)
                    Destroy(audMan);
            }
        }

        public override float voicePitch => 1.15f;
    }
}
