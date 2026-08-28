using UnityEngine;

namespace BaldiPlusRandomZone.Extensions
{
    public static class PosterExtensions
    {
        public static PosterObject CreateZonePosters(int zone, System.Random random)
        {
            PosterObject poster = ScriptableObject.CreateInstance<PosterObject>();
            poster.name = "ZonePoster_" + zone;
            poster.baseTexture = Plugin.assetMan.Get<Texture2D>("ZoneBasePoster_" + random.Next(0, 35));
            poster.textData = Plugin.assetPlusMan.Get<PosterObject>("LaboratoryZone_1").textData.GetNew();

            Color color = new((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());

            poster.baseTexture = poster.baseTexture.GenerateColoredTexture(color);

            bool invertText = color.grayscale < 0.25f;

            foreach (PosterTextData data in poster.textData)
            {
                if (data.textKey == "PST_HNT_Zone")
                {
                    if (invertText)
                        data.color = Color.white;

                    continue;
                }

                float red = data.color.r;
                Color finalColor;

                if (invertText)
                    finalColor = Color.Lerp(Color.white, color, red);
                else
                    finalColor = new Color(red * color.r, red * color.g, red * color.b, 1f);
                
                data.color = finalColor;
                data.textKey = zone.ToString();
            }

            return poster;
        }

        public static PosterTextData[] GetNew(this PosterTextData[] original)
        {
            PosterTextData[] cloneed = new PosterTextData[original.Length];

            for (int i = 0; i < original.Length; i++)
            {
                cloneed[i] = new PosterTextData
                {
                    alignment = original[i].alignment,
                    color = original[i].color,
                    font = original[i].font,
                    fontSize = original[i].fontSize,
                    position = original[i].position,
                    size = original[i].size,
                    style = original[i].style,
                    textKey = original[i].textKey
                };
            }

            return cloneed;
        }

        public static Texture2D GenerateColoredTexture(this Texture2D texture, Color color)
        {
            Texture2D tex = new(texture.width, texture.height);

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                    tex.SetPixel(x, y, texture.GetPixel(x, y) * color);
            }

            tex.Apply();
            return tex;
        }
    }
}
