using System;
using System.Collections.Generic;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_DeadEnd : ZoneRule
    {
        private int minDeadEndLength = 2;
        private int maxDeadEndLength = 7;
        private int maxCreatedDeadEnds = 3;

        public ZoneRule_DeadEnd(int negativeWeight) : base(0, negativeWeight) { }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);
            maxCreatedDeadEnds = 2 + (2 * levelId);
        }

        public override void Generate(LevelBuilder lb, Random random)
        {
            base.Generate(lb, random);

            List<Cell> candidates = GetCandidates(lb);
            candidates.ControlledShuffle(random);

            int created = 0;

            foreach (Cell candidate in candidates)
            {
                if (created >= maxCreatedDeadEnds)
                    break;

                if (!TryCreateDeadEnd(lb, candidate, random))
                    continue;

                created++;
            }
        }

        private List<Cell> GetCandidates(LevelBuilder lb)
        {
            List<Cell> result = new List<Cell>();

            foreach (Cell cell in lb.Ec.AllCells())
            {
                if (cell == null)
                    continue;

                if (cell.shape != TileShapeMask.Straight)
                    continue;

                if (IsImportantCell(cell))
                    continue;

                List<Cell> neighbors = GetNavigableNeighbors(cell);

                if (neighbors.Count != 2)
                    continue;

                if (GetNavigableNeighbors(neighbors[0]).Count > 2)
                    continue;

                if (GetNavigableNeighbors(neighbors[1]).Count > 2)
                    continue;

                result.Add(cell);
            }

            return result;
        }

        private bool TryCreateDeadEnd(LevelBuilder lb, Cell candidate, Random random)
        {
            List<Cell> neighbors = GetNavigableNeighbors(candidate);

            if (neighbors.Count != 2)
                return false;

            Cell sideA = neighbors[0];
            Cell sideB = neighbors[1];

            int lengthA = GetCorridorLength(sideA, candidate);
            int lengthB = GetCorridorLength(sideB, candidate);

            Cell deadEndSide;
            Cell connectionSide;

            if (lengthA >= minDeadEndLength && lengthA <= maxDeadEndLength)
            {
                deadEndSide = sideA;
                connectionSide = sideB;
            }
            else if (lengthB >= minDeadEndLength && lengthB <= maxDeadEndLength)
            {
                deadEndSide = sideB;
                connectionSide = sideA;
            }
            else
            {
                return false;
            }

            if (GetNavigableNeighbors(deadEndSide).Count > 2 || !HasJunctionAhead(deadEndSide, candidate) || IsImportantCell(candidate) || !WouldRemainConnected(lb, deadEndSide, candidate) || DoesDisconnectMap(lb, candidate))
                return false;

            if (!HasJunctionAhead(deadEndSide, candidate))
                return false;

            if (IsImportantCell(candidate))
                return false;


            foreach (Direction dir in Directions.All())
            {
                IntVector2 position = candidate.position + dir.ToIntVector2();

                if (lb.Ec.ContainsCoordinates(position) && lb.Ec.CellFromPosition(position).room == candidate.room)
                    lb.Ec.CloseCell(position, dir.GetOpposite());
            }

            lb.Ec.DestroyCell(candidate);

            return true;
        }

        private int GetCorridorLength(Cell start, Cell previous)
        {
            int length = 1;

            Cell current = start;
            Cell last = previous;

            HashSet<Cell> visited = new HashSet<Cell>();
            visited.Add(previous);

            while (current != null && length <= maxDeadEndLength)
            {
                if (!visited.Add(current))
                    break;

                List<Cell> neighbors = GetNavigableNeighbors(current);

                if (neighbors.Count > 2)
                    break;

                if (neighbors.Count <= 1)
                {
                    length++;
                    break;
                }

                neighbors.Remove(last);

                if (neighbors.Count != 1)
                    break;

                last = current;
                current = neighbors[0];

                length++;
            }

            return length;
        }

        private bool HasJunctionAhead(Cell start, Cell previous)
        {
            Cell current = start;
            Cell last = previous;

            HashSet<Cell> visited = new HashSet<Cell>();

            while (current != null)
            {
                if (!visited.Add(current))
                    return false;

                List<Cell> neighbors = GetNavigableNeighbors(current);

                if (neighbors.Count > 2)
                    return true;

                if (neighbors.Count <= 1)
                    return false;

                neighbors.Remove(last);

                if (neighbors.Count != 1)
                    return false;

                last = current;
                current = neighbors[0];
            }

            return false;
        }

        private bool WouldRemainConnected(LevelBuilder lb, Cell start, Cell blocked)
        {
            Queue<Cell> queue = new Queue<Cell>();
            HashSet<Cell> visited = new HashSet<Cell>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                Cell current = queue.Dequeue();

                foreach (Cell next in GetNavigableNeighbors(current))
                {
                    if (next == blocked)
                        continue;

                    if (!visited.Add(next))
                        continue;

                    queue.Enqueue(next);
                }
            }

            return visited.Count >= minDeadEndLength;
        }

        private List<Cell> GetNavigableNeighbors(Cell cell)
        {
            List<Cell> result = new List<Cell>();

            foreach (Direction dir in Directions.All())
            {
                if (!cell.NavNavigable(dir))
                    continue;

                Cell neighbor = cell.room.ec.CellFromPosition(cell.position + dir.ToIntVector2());

                if (neighbor != null)
                    result.Add(neighbor);
            }

            return result;
        }

        private bool DoesDisconnectMap(LevelBuilder lb, Cell candidate)
        {
            List<Cell> cells = new List<Cell>();

            foreach (Cell cell in lb.Ec.AllCells())
            {
                if (cell != null && cell != candidate)
                    cells.Add(cell);
            }

            if (cells.Count == 0)
                return true;

            Cell start = cells[0];

            Queue<Cell> queue = new Queue<Cell>();
            HashSet<Cell> visited = new HashSet<Cell>();

            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                Cell current = queue.Dequeue();

                foreach (Cell next in GetNavigableNeighbors(current))
                {
                    if (next == candidate)
                        continue;

                    if (!visited.Add(next))
                        continue;

                    queue.Enqueue(next);
                }
            }

            return visited.Count != cells.Count;
        }

        private bool IsImportantCell(Cell cell)
        {
            return cell.hasLight || cell.doorHere || cell.HasAnyHardCoverage || cell.locked || cell.room.type != RoomType.Hall;
        }

        public override void LoadPreparation(Random random)
        {
            base.LoadPreparation(random);
            SetRuleType(ZoneRuleType.Negative);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.DeadEnd;

        public override float negativePowerBonus => 2f;
    }
}