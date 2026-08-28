using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using UnityEngine;

namespace BaldiPlusRandomZone.CustomCharacters
{
    public class Reskin_ClassicSweep : SweepReskin
    {
        public override void SetupPrefab()
        {
            base.SetupPrefab();
            SetPoster(ObjectCreators.CreateCharacterPoster(AssetLoader.TextureFromMod(Plugin.instance, "CustomCharacters", "pri_classicgottasweep.png"), "PST_PRI_ClassicSweep1", "PST_PRI_ClassicSweep2"));
            sweep.spriteRenderer[0].sprite = AssetLoader.SpriteFromMod(Plugin.instance, Vector2.one / 2, 26f, "CustomCharacters", "ClassicGottaSweep.png");
            sweep.Navigator.Am.moveMods.Add(new(Vector3.zero, 0.5f));
        }

        public override float voicePitch => 0.8f;
    }
}
