using CommunityStickersPlus.Structures;
using System.Linq;
using UnityEngine;

namespace BaldiPlusRandomZone.Compacts
{
    public static class CommunityStickerCompact
    {
        public static void LoadCompact()
        {
            SceneObject scene = Plugin.assetMan.Get<SceneObject>("ZonePitStop");

            scene.levelAsset.structures.Add(new StructureBuilderData
            {
                prefab = Resources.FindObjectsOfTypeAll<Structure_BullyShop>().FirstOrDefault(),
                data = [new(null, new(6, 11), Direction.North)]
            });
        }
    }
}
