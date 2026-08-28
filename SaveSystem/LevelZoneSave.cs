using BaldiPlusRandomZone.ZoneRules;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BaldiPlusRandomZone.SaveSystem
{
    [Serializable]
    public class LevelZoneSave
    {
        public int zone;
        public int level;
        public string[] items = new string[9];
        public string[] lockersItems = new string[3];
        public bool[] avaliableLockerSlots = new bool[3];
        public ZoneStickerStateData[] appliedStickers = new ZoneStickerStateData[4];
        public List<ZoneStickerStateData> inventoryStickers = new List<ZoneStickerStateData>();
        public bool[] upgradedSlots = new bool[4];
        public int lives;
        public int attempts;
        public int ytps;
        public int seed;
        public ZoneRuleCategory zoneRule;
        public ZoneRuleType zoneRuleType;
        public LifeMode lifeMode;
        public MapSaveData map = new MapSaveData();
        public List<ShopItemData> shopItems = new List<ShopItemData>();
        public bool saveAvaliable;
    }

    [Serializable]
    public class MapSaveData 
    {
        public List<MapMarkerData> markers = new List<MapMarkerData>();
        public bool buyedMap;
        public bool mapAvaliable;
        public bool[] foundedTiles = new bool[0];
        public IntVector2 mapSize;
        public bool empty = true;
    }

    [Serializable]
    public class MapMarkerData
    {
        public Vector2 position;
        public int id;

        public MapMarkerData(Vector2 position, int id)
        {
            this.position = position;
            this.id = id;
        }
    }

    [Serializable]
    public class ZoneStickerStateData
    {
        public ZoneStickerStateData(StickerStateData data)
        {
            sticker = data.sticker.ToStringExtended();
            activeLevel = data.activeLevel;
            opened = data.opened;
            sticky = data.sticky;
        }

        public string sticker;
        public int activeLevel;
        public bool opened;
        public bool sticky;
    }

    [Serializable]
    public class ShopItemData
    {
        public string itemId;
        public int originalPrice;
        public int demand;
    }

}
