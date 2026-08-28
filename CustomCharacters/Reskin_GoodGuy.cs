using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Registers;
using UnityEngine;

namespace BaldiPlusRandomZone.CustomCharacters
{
    public class Reskin_GoodGuy : BullyReskin
    {
        public override void SetupPrefab()
        {
            base.SetupPrefab();
            bully.spriteRenderer[0].sprite = AssetLoader.SpriteFromMod(Plugin.instance, Vector2.one / 2f, 26f, "CustomCharacters", "GoodGuy_final.png");
            SetPoster(ObjectCreators.CreateCharacterPoster(AssetLoader.TextureFromMod(Plugin.instance, "CustomCharacters", "pri_goodguy.png"), "PST_PRI_GoodGuy1", "PST_PRI_GoodGuy2"));
        }

        public override void OnEntityEnter(Entity entity, bool validColision)
        {
            base.OnEntityEnter(entity, validColision);

            if (entity is PlayerEntity && validColision && bully.behaviorStateMachine.currentState is Bully_Active)
            {
                PlayerManager pm = entity.GetComponent<PlayerManager>();

                if (pm.itm.HasItem() || !pm.Tagged)
                {
                    EnvironmentController ec = entity.Ec;
                    Pickup pickup = ec.CreateItem(bully.Entity.CurrentRoom, quarter, Vector2.zero);
                    pickup.transform.position = bully.transform.position;
                }
            }
        }

        public ItemObject quarter => ItemMetaStorage.Instance.FindByEnum(Items.Quarter).value;
        public override float voicePitch => 1.25f;
    }
}
