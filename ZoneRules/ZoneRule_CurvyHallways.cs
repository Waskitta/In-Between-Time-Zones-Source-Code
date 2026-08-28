using System.Collections.Generic;
using UnityEngine;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_CurvyHallways : ZoneRule
    {
        public ZoneRule_CurvyHallways(int weight) : base(0, weight) { }

        public override void Generate(LevelBuilder lb, System.Random random)
        {
            base.Generate(lb, random);

            List<List<Cell>> hallways = new List<List<Cell>>(lb.Ec.FindHallways());

            foreach (List<Cell> hallway in hallways)
            {
                Direction hallwayDir = Direction.Null;

                if (hallway.Count > 1)
                {
                    IntVector2 delta = hallway[1].position - hallway[0].position;

                    if (delta.x > 0)
                        hallwayDir = Direction.East;
                    else if (delta.x < 0)
                        hallwayDir = Direction.West;
                    else if (delta.z > 0)
                        hallwayDir = Direction.North;
                    else if (delta.z < 0)
                        hallwayDir = Direction.South;
                }

                foreach (Cell cell in hallway)
                {
                    if (random.Next(0, 2) == 1 && !cell.locked && !cell.HasAnyHardCoverage)
                    {
                        Direction side = GetRandomSideDirection(hallwayDir, random);
                        MoveCell(lb, cell, side);
                    }
                }
            }

            foreach (RoomController room in lb.Ec.rooms)
            {
                foreach (Door door in room.doors)
                {
                    if (door.aTile.Null)
                        lb.Ec.CreateCell(15, door.aTile.position, lb.Ec.mainHall);
                    if (door.bTile.Null)
                        lb.Ec.CreateCell(15, door.bTile.position, lb.Ec.mainHall);
                }
            }

            foreach (Elevator elevator in lb.Ec.Elevators)
            {
                Direction direction1 = elevator.Door.direction.GetOpposite();
                IntVector2 position1 = elevator.Door.position + (direction1.ToIntVector2() * 2);

                if (lb.Ec.ContainsCoordinates(position1) && lb.Ec.CellFromPosition(position1).Null)
                    lb.Ec.CreateCell(15, position1, lb.Ec.mainHall);
            }

            List<Cell> cellsToModify = new List<Cell>(lb.Ec.mainHall.cells);
            cellsToModify.RemoveAll(x => x.locked);

            for (int i = 0; i < cellsToModify.Count; i++)
            {
                Cell cell = cellsToModify[i];

                if (cell.shape == TileShapeMask.End)
                {
                    foreach (Direction dir in Directions.All())
                    {
                        IntVector2 position = cell.position + dir.ToIntVector2();

                        if (lb.Ec.ContainsCoordinates(position) &&
                            lb.Ec.CellFromPosition(position).Null)
                        {
                            lb.Ec.CreateCell(15, position, cell.room);
                            cellsToModify.Add(lb.Ec.CellFromPosition(position));
                            break;
                        }
                    }
                }

                cell.SetShape(15, TileShapeMask.Closed);

                foreach (Direction dir in Directions.All())
                {
                    IntVector2 position = cell.position + dir.ToIntVector2();

                    if (lb.Ec.ContainsCoordinates(position))
                    {
                        Cell otherCell = lb.Ec.CellFromPosition(position);

                        if (otherCell.room == cell.room)
                            lb.Ec.ConnectCells(cell.position, dir);
                        else
                            lb.Ec.CloseCell(cell.position, dir);
                    }
                }

                if (cell.shape == TileShapeMask.Closed)
                    lb.Ec.DestroyCell(cell);
            }

            Door[] doors = Object.FindObjectsOfType<Door>();

            foreach (Door door in doors)
            {
                if (door.aTile.room.type == RoomType.Room || door.bTile.room.type == RoomType.Room)
                    lb.Ec.ConnectCells(door.aTile.position, door.direction);
            }
        }

        public void MoveCell(LevelBuilder lb, Cell cell, Direction dir)
        {
            IntVector2 newPosition = cell.position + dir.ToIntVector2();
            RoomController room = cell.room;

            if (!lb.Ec.ContainsCoordinates(newPosition) || !lb.Ec.CellFromPosition(newPosition).Null)
                return;

            lb.Ec.DestroyCell(cell);
            lb.Ec.CreateCell(15, newPosition, room);

            Cell newCell = lb.Ec.CellFromPosition(newPosition);

            foreach (Direction direction in newCell.AllWallDirections)
            {
                if (direction == dir || direction == dir.GetOpposite())
                    continue;

                IntVector2 sidePosition = newPosition + direction.ToIntVector2();

                if (!lb.Ec.ContainsCoordinates(sidePosition) || !lb.Ec.CellFromPosition(sidePosition).Null)
                    continue;

                lb.Ec.CreateCell(15, sidePosition, room);
                lb.Ec.ConnectCells(sidePosition, direction.GetOpposite());
            }
        }

        public Direction GetRandomSideDirection(Direction origin, System.Random random)
        {
            List<Direction> dirs = new List<Direction>();

            foreach (Direction dir in Directions.All())
            {
                if (origin == dir || dir == origin.GetOpposite())
                    continue;

                dirs.Add(dir);
            }

            return dirs[random.Next(0, dirs.Count)];
        }

        public override void LoadPreparation(System.Random random)
        {
            base.LoadPreparation(random);
            type = ZoneRuleType.Negative;
            weight = negativeWeight;
            powerBonus = negativePowerBonus;
        }


        public override ZoneRuleCategory category => ZoneRuleCategory.CurvyHallways;
        public override float negativePowerBonus => 1.6f;
    }
}
