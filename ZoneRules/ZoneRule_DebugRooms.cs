using System.Linq;
using UnityEngine;

namespace BaldiPlusRandomZone.ZoneRules
{
   public class ZoneRule_DebugRooms : ZoneRule
    {
        public ZoneRule_DebugRooms(int weight) : base(0, weight) { }

        public override void AfterAllRoomsPlaced(RoomController room, LevelBuilder lb, System.Random random)
        {
            base.AfterAllRoomsPlaced(room, lb, random);

            if (room.category == RoomCategory.Office || room.category == RoomCategory.Store || room.category == RoomCategory.Special) return;

            room.wallTex = Plugin.assetPlusMan.Get<Texture2D>("Placeholder_Wall_W");
            room.florTex = Plugin.assetPlusMan.Get<Texture2D>("Placeholder_Floor");
            room.ceilTex = Plugin.assetPlusMan.Get<Texture2D>("Placeholder_Celing");
            room.color = Color.blue;
            room.functions = Object.Instantiate(Plugin.assetPlusMan.Get<RoomFunctionContainer>("NoFunction"));
            room.functions.Initialize(room);
            room.doorMats = Plugin.assetPlusMan.Get<StandardDoorMats>("DefaultDoorSet");
            room.mapMaterial = null;
        }

        public override void LoadPreparation(System.Random random)
        {
            base.LoadPreparation(random);
            SetRuleType(ZoneRuleType.Maybe);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.DebugRooms;
        public override float negativePowerBonus => 1.45f;
    }
}
