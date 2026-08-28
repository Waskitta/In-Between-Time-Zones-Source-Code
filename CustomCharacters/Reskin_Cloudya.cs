using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Reflection;
using UnityEngine;

namespace BaldiPlusRandomZone.CustomCharacters
{
    public class Reskin_Cloudya : CloudyReskin
    {
        public override void SetupPrefab()
        {
            base.SetupPrefab();
            BeltManager windManager = (BeltManager)cloudy.ReflectionGetVariable("windManager");
            Material windMat = new((Material)windManager.ReflectionGetVariable("sourceMaterial"));
            windMat.SetMainTexture(AssetLoader.TextureFromMod(Plugin.instance, "CustomCharacters", "ReverseWind.png"));
            windManager.ReflectionSetVariable("sourceMaterial", windMat);

            cloudy.spriteRenderer[0].sprite = AssetLoader.SpriteFromMod(Plugin.instance, Vector2.one / 2f, 25f, "CustomCharacters", "Cloudya.png");
            SetPoster(ObjectCreators.CreateCharacterPoster(AssetLoader.TextureFromMod(Plugin.instance, "CustomCharacters", "pri_cloudya.png"), "PST_PRI_Cloudya1", "PST_PRI_Cloudya2"));
        }

        public override void VirtualUpdate()
        {
            base.VirtualUpdate();

            if (cloudy.behaviorStateMachine.currentState is Cumulo_Blowing)
            {
                if (reverseDir)
                {
                    BeltManager wind = (BeltManager)cloudy.ReflectionGetVariable("windManager");
                    wind.SetDirection(this.dir);
                    return;
                }

                Direction dir = (Direction)cloudy.ReflectionGetVariable("dir");
                this.dir = dir.GetOpposite();
                reverseDir = true;
            }
            else
                reverseDir = false;
        }

        public bool reverseDir;
        public Direction dir;
        public override float voicePitch => 1.35f;
    }
}
