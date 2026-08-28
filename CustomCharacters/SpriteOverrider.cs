using UnityEngine;

namespace BaldiPlusRandomZone.CustomCharacters
{
    public class SpriteOverrider : MonoBehaviour
    {
        public SpriteRenderer renderer;
        public OverriderMap[] overriderMaps = [];

        public void SetSprite()
        {
            foreach (OverriderMap map in overriderMaps)
            {
                for (int i = 0; i < map.originalSprites.Length; i++)
                {
                    if (renderer.sprite == map.originalSprites[i])
                        renderer.sprite = map.overrideSprites[i];
                }
            }
        }
    }

    public class OverriderMap
    {
        public OverriderMap(Sprite[] original, Sprite[] overriders)
        {
            originalSprites = original;
            overrideSprites = overriders;
        }

        public Sprite[] originalSprites = [];
        public Sprite[] overrideSprites = [];
    }
}
