using BaldiPlusRandomZone.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_OpenHallways : ZoneRule
    {
        public ZoneRule_OpenHallways(int weight) : base(weight) { }

        public override void Generate(LevelBuilder lb, System.Random random)
        {
            base.Generate(lb, random);

            foreach (Cell cell in lb.Ec.cells)
            {
                if (cell.Null && !cell.locked && cell.room.type == RoomType.Null)
                    lb.Ec.CreateCell(0, cell.position, lb.Ec.mainHall);
            }

            foreach (Cell cell in lb.Ec.mainHall.cells.ToArray())
            {
                lb.Ec.UpdateCell(cell.position);

                if (cell.CheckRoomTrap(cell.room))
                    lb.Ec.DestroyCell(cell);
            }
            

            foreach (Door door in lb.Ec.mainHall.doors)
            {
                if (lb.Ec.CellFromPosition(door.aTile.position + door.direction.ToIntVector2()).room == lb.Ec.mainHall)
                    lb.Ec.ConnectCells(door.bTile.position, door.direction.GetOpposite());
                else if (lb.Ec.CellFromPosition(door.bTile.position + door.direction.ToIntVector2()).room == lb.Ec.mainHall)
                    lb.Ec.ConnectCells(door.aTile.position, door.direction);
            }

            foreach (Door door in Object.FindObjectsOfType<Window>())
            {
                if (lb.Ec.CellFromPosition(door.aTile.position + door.direction.ToIntVector2()).room == lb.Ec.mainHall)
                    lb.Ec.ConnectCells(door.bTile.position, door.direction.GetOpposite());
                else if (lb.Ec.CellFromPosition(door.bTile.position + door.direction.ToIntVector2()).room == lb.Ec.mainHall)
                    lb.Ec.ConnectCells(door.aTile.position, door.direction);
            }

            foreach (RoomController room in lb.Ec.rooms)
            {
                foreach (Cell cell in room.cells)
                {
                    foreach (Direction dir in cell.AllWallDirections)
                    {
                        if (lb.Ec.ContainsCoordinates(cell.position + dir.ToIntVector2()) && !lb.Ec.CellFromPosition(cell.position + dir.ToIntVector2()).HasWallInDirection(dir))
                            lb.Ec.CloseCell(cell.position + dir.ToIntVector2(), dir.GetOpposite());
                    }
                }
            }
        }

        public override void LoadPreparation(System.Random random)
        {
            base.LoadPreparation(random);
            type = ZoneRuleType.Negative;
            weight = positiveWeight;
            powerBonus = negativePowerBonus;
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.OpenHallways;
        public override float negativePowerBonus => 1.6f;
    }
}
