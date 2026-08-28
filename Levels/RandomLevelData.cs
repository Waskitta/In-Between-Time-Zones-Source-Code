using MTM101BaldAPI;
using System;

namespace BaldiPlusRandomZone.Levels
{
    [Serializable]
    public class RandomLevelData
    {
        public int levelSize = 0;
        public CustomLevelObject level;
        public Baldi mainBaldi;
        public int shopItemCount;
        public int mapPrice;
        public float stickerPriceMultiplier = 1f;
        public int stickerRottenZone = 2;
    }
}
