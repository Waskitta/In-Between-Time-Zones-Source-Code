using BaldiPlusRandomZone.EndlessSupport;
using MTM101BaldAPI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaldiPlusRandomZone.RoomFunctions
{
    public class WoodRoomFunction : RoomFunction
    {
        public override void OnGenerationFinished()
        {
            base.OnGenerationFinished();

            return;
            RendererContainer[] renderers = room.objectObject.GetComponentsInChildren<RendererContainer>();

            if (Singleton<CoreGameManager>.Instance.sceneObject.GetCustomLevelObjects().Length > 0 && Singleton<EndlessZoneManager>.Instance != null)
            {
                System.Random rng = new(Singleton<CoreGameManager>.Instance.Seed() + Singleton<EndlessZoneManager>.Instance.currentZone + (int)room.category);
                Texture2D[] textures = (Texture2D[])Singleton<CoreGameManager>.Instance.sceneObject.GetCurrentCustomLevelObject().GetCustomModValue(Plugin.instance.Info, "WoodTextures");
                Texture2D texture = textures[rng.Next(textures.Length)];

                foreach (RendererContainer conainer in renderers)
                {
                    foreach (Renderer renderer in conainer.renderers)
                    {
                        if (renderer is MeshRenderer meshRenderer)
                        {
                            Material[] materials = meshRenderer.materials;

                            for (int i = 0; i < materials.Length; i++)
                            {
                                if (materials[i].mainTexture != null && materials[i].mainTexture.name == "wood 1")
                                    materials[i] = GetOrCreate(texture, materials[i]);
                            }

                            meshRenderer.materials = materials;
                        }
                    }
                }
            }
        }

        public Material GetOrCreate(Texture2D texture, Material material)
        {
            string materialName = texture.name + "_Mat";

            Material existing = createdMaterials.FirstOrDefault(x => x.name == materialName);

            if (existing != null)
                return existing;

            Material mat = new Material(material);
            mat.mainTexture = texture;
            mat.name = materialName;

            createdMaterials.Add(mat);
            return mat;
        }

        //WoodSimple
        private static readonly List<Material> createdMaterials = new List<Material>();
    }
}
