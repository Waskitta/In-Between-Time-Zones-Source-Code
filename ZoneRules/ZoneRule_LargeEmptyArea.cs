using MTM101BaldAPI.Reflection;
using System;
using System.Linq;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_LargeEmptyArea : ZoneRule
    {
        public ZoneRule_LargeEmptyArea(int weight) : base(weight) { }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);
            level.specialRoomsStickToEdge = false;
        }

        public override void Generate(LevelBuilder lb, Random random)
        {
            base.Generate(lb, random);

            EnvironmentController ec = lb.Ec;

            foreach (RoomController room in ec.rooms)
            {
                if (room.category == RoomCategory.Special)
                {
                    Cell[] cells = room.cells.ToArray();

                    foreach (Door door in room.doors.ToArray())
                    {
                        if (door is SwingDoor swingDoor)
                        {
                            MapTile aMapTile = (MapTile)swingDoor.ReflectionGetVariable("aMapTile");
                            MapTile bMapTile = (MapTile)swingDoor.ReflectionGetVariable("bMapTile");
                            aMapTile.SpriteRenderer.enabled = false;
                            bMapTile.SpriteRenderer.enabled = false;
                            room.doors.Remove(door);
                        }
                    }

                    foreach (Cell cell in cells)
                    {
                        int constbin = cell.ConstBin;

                        foreach (Direction dir in Directions.All())
                        {
                            if (ec.ContainsCoordinates(cell.position + dir.ToIntVector2()) && (ec.CellFromPosition(cell.position + dir.ToIntVector2()).room == ec.mainHall || ec.CellFromPosition(cell.position + dir.ToIntVector2()).room == cell.room) && constbin.ContainsDirection(dir) && !ec.CellFromPosition(cell.position + dir.ToIntVector2()).hideFromMap && !cell.hideFromMap)
                            {
                                constbin = constbin - (1 << (int)dir);
                                ec.ConnectCells(cell.position + dir.ToIntVector2(), dir.GetOpposite());
                            }
                        }

                        ec.CreateCell(constbin, cell.room.transform, cell.position, ec.mainHall, true, false);
                        Cell newCell = ec.CellFromPosition(cell.position);
                        newCell.hideFromMap = false;
                    }

                    foreach (Door door in room.doors)
                    {
                        door.position = ec.CellFromPosition(door.transform.position).position;
                        door.Initialize();
                        ec.CellFromPosition(door.position).AssignDoor(door, door.direction);
                    }

                    room.objectObject.gameObject.SetActive(false);
                    room.functionObject.gameObject.SetActive(false);
                    ec.rooms.Remove(room);
                    break;
                }
            }
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            weight = positiveWeight;
            type = ZoneRuleType.Maybe;
            powerBonus = positivePowerBonus;
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.LargeEmptyArea;
        public override float positivePowerBonus => 1.25f;
    }
}
