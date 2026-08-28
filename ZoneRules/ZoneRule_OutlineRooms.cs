using BaldiPlusRandomZone.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_OutlineRooms : ZoneRule
    {
        public ZoneRule_OutlineRooms(int weight) : base(weight) { }

        public override void Generate(LevelBuilder lb, Random random)
        {
            base.Generate(lb, random);

            List<Cell> toConnect = new List<Cell>();

            foreach (RoomController room in lb.Ec.rooms)
            {
                foreach (Cell cell in room.cells)
                {
                    IntVector2[] directions =
                    {
                        new IntVector2(0, 1),
                        new IntVector2(1, 1),
                        new IntVector2(1, 0),
                        new IntVector2(1, -1),
                        new IntVector2(0, -1),
                        new IntVector2(-1, -1),
                        new IntVector2(-1, 0),
                        new IntVector2(-1, 1)
                    };

                    foreach (IntVector2 offset in directions)
                    {
                        IntVector2 position = cell.position + offset;

                        if (lb.Ec.ContainsCoordinates(position) && lb.Ec.CellFromPosition(position).Null)
                        {
                            lb.Ec.CreateCell(15, position, lb.Ec.mainHall);
                            toConnect.Add(lb.Ec.CellFromPosition(position));
                        }
                    }
                }
            }

            foreach (Cell cell in toConnect)
            {
                foreach (Direction dir in Directions.All())
                {
                    IntVector2 position = cell.position + dir.ToIntVector2();

                    if (lb.Ec.ContainsCoordinates(position) && lb.Ec.CellFromPosition(position).room == lb.Ec.mainHall && !lb.Ec.CellFromPosition(position).locked)
                        lb.Ec.ConnectCells(cell.position, dir);
                }

                if (cell.CheckRoomTrap(cell.room))
                    lb.Ec.DestroyCell(cell);
            }
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            type = ZoneRuleType.Maybe;
            weight = positiveWeight;
            powerBonus = positivePowerBonus;
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.OutlineRooms;
        public override float positivePowerBonus => 1.15f; 
    }
}
