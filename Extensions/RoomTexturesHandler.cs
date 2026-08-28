using MTM101BaldAPI.AssetTools;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BaldiPlusRandomZone.Extensions
{
    public static class RoomTexturesHandler
    {
        public static WeightedTexture2D[] LoadTextures(RoomCategory category, CellTexturePart texturePart)
        {
            if (textures.ContainsKey(category))
            {
                switch (texturePart)
                {
                    case CellTexturePart.Wall:
                        return textures[category].walls.ToArray();
                    case CellTexturePart.Floor:
                        return textures[category].floors.ToArray();
                    case CellTexturePart.Ceiling:
                        return textures[category].ceiling.ToArray();
                }
            }
            else
            {
                RoomTextures roomTextures = new RoomTextures();

                string[] wallPaths = Directory.GetFiles(Path.Combine(AssetLoader.GetModPath(Plugin.instance), "LevelTextures", category.ToString(), "Walls"), "*.png");
                string[] florPaths = Directory.GetFiles(Path.Combine(AssetLoader.GetModPath(Plugin.instance), "LevelTextures", category.ToString(), "Floors"), "*.png");
                string[] ceilPaths = Directory.GetFiles(Path.Combine(AssetLoader.GetModPath(Plugin.instance), "LevelTextures", category.ToString(), "Ceilings"), "*.png");

                foreach (var path in wallPaths)
                {
                    Texture2D texture = AssetLoader.TextureFromFile(path);
                    string name = Path.GetFileNameWithoutExtension(path);
                    string[] parts = name.Split('!');
                    int weight = int.Parse(parts[0]);

                    roomTextures.walls.Add(new WeightedTexture2D { selection = texture, weight = weight });
                }

                foreach (var path in florPaths)
                {
                    Texture2D texture = AssetLoader.TextureFromFile(path);
                    string name = Path.GetFileNameWithoutExtension(path);
                    string[] parts = name.Split('!');
                    int weight = int.Parse(parts[0]);

                    roomTextures.floors.Add(new WeightedTexture2D { selection = texture, weight = weight });
                }

                foreach (var path in ceilPaths)
                {
                    Texture2D texture = AssetLoader.TextureFromFile(path);
                    string name = Path.GetFileNameWithoutExtension(path);
                    string[] parts = name.Split('!');
                    int weight = int.Parse(parts[0]);

                    roomTextures.ceiling.Add(new WeightedTexture2D { selection = texture, weight = weight });
                }

                textures.Add(category, roomTextures);
            }

            switch (texturePart)
            {
                case CellTexturePart.Wall:
                    return textures[category].walls.ToArray();
                case CellTexturePart.Floor:
                    return textures[category].floors.ToArray();
                case CellTexturePart.Ceiling:
                    return textures[category].ceiling.ToArray();
            }

            return null;
        }

        public static Dictionary<RoomCategory, RoomTextures> textures = new Dictionary<RoomCategory, RoomTextures>();
    }

    public class RoomTextures
    {
        public List<WeightedTexture2D> walls = new List<WeightedTexture2D>();
        public List<WeightedTexture2D> floors = new List<WeightedTexture2D>();
        public List<WeightedTexture2D> ceiling = new List<WeightedTexture2D>();
    }

    public enum CellTexturePart
    {
        Wall,
        Floor,
        Ceiling
    }
}
