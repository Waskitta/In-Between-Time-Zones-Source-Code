using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Reflection;
using UnityEngine;

namespace BaldiPlusRandomZone.CustomCharacters
{
    public class Reskin_Businessman : PrincipalReskin
    {
        public override void SetupPrefab()
        {
            base.SetupPrefab();

            Sprite normalSprite = AssetLoader.SpriteFromMod(Plugin.instance, Vector2.one / 2, 65f, "CustomCharacters", "Businessman.png");
            Sprite downSprite = AssetLoader.SpriteFromMod(Plugin.instance, Vector2.one / 2, 65f, "CustomCharacters", "Businessman_HandDown.png");

            PosterObject poster = ObjectCreators.CreateCharacterPoster(AssetLoader.TextureFromMod(Plugin.instance, "CustomCharacters", "pri_businessman.png"), "PST_PRI_BusinessMan1", "PST_PRI_BusinessMan2");
            poster.textData[0].size = new(25 + poster.textData[0].size.x, poster.textData[0].size.z);
            poster.textData[0].position = new(poster.textData[0].position.x - 12, poster.textData[0].position.z);
            SetPoster(poster);
            principal.ReflectionSetVariable("normalSprite", downSprite);
            principal.ReflectionSetVariable("chasingSprite", normalSprite);
            principal.SwitchToNormalSprite();
        }

        public override void SendToDetention(bool validColision)
        {
            base.SendToDetention(validColision);

            if (validColision)
                Singleton<CoreGameManager>.Instance.AddPoints(-25, 0, true);
        }

        public override void OnEntityEnter(Entity entity, bool validColision)
        {
            base.OnEntityEnter(entity, validColision);

            if (validColision && entity is PlayerEntity && !entity.GetComponent<PlayerManager>().Disobeying && principal.behaviorStateMachine.currentState is not Principal_ChasingPlayer && entity.CurrentRoom.category != RoomCategory.Office)
                Singleton<CoreGameManager>.Instance.AddPoints(5, 0, true);
        }

        public override float voicePitch => 0.75f;
    }
}
