using MTM101BaldAPI.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_ConveyorBelts : ZoneRule
    {
        public ZoneRule_ConveyorBelts(int negativeWeight) : base(0, negativeWeight)
        {
        }

        public override void Generate(LevelBuilder lb, System.Random random)
        {
            base.Generate(lb, random);

            Structure_ConveyorBelt structure = Object.Instantiate((Structure_ConveyorBelt)Plugin.assetPlusMan.Get<StructureBuilder>("ConveyorBeltConstructor"));
            structure.Initialize(lb.Ec, new());
            structure.ReflectionSetVariable("beltSpeed", 15f);
            structure.Load(GenerateBeltStructureData(lb, random));
        }

        public override void ModifyLevelObject(LevelGenerationParameters level, int levelId)
        {
            base.ModifyLevelObject(level, levelId);

            foreach (WeightedItemObject item in level.potentialItems)
            {
                if (item.selection.itemType == Items.Boots)
                    item.weight = Mathf.CeilToInt(item.weight * 1.5f);
            }    
        }

        private static List<StructureData> GenerateBeltStructureData(LevelBuilder lb, System.Random random)
        {
            List<StructureData> result = new List<StructureData>();
            HashSet<IntVector2> cells = new HashSet<IntVector2>();

            foreach (Cell cell in lb.Ec.mainHall.cells)
                cells.Add(cell.position);

            List<(IntVector2 start, IntVector2 end, Direction direction)> belts = new List<(IntVector2, IntVector2, Direction)>();
            HashSet<IntVector2> beltCells = new HashSet<IntVector2>();

            foreach (IntVector2 position in cells)
            {
                bool west = cells.Contains(position + Direction.West.ToIntVector2());
                bool east = cells.Contains(position + Direction.East.ToIntVector2());

                if (!west && east)
                {
                    IntVector2 end = position;

                    while (cells.Contains(end + Direction.East.ToIntVector2()))
                        end += Direction.East.ToIntVector2();

                    if (end != position)
                    {
                        bool overlaps = false;

                        IntVector2 current = position;

                        while (true)
                        {
                            if (beltCells.Contains(current))
                            {
                                overlaps = true;
                                break;
                            }

                            if (current == end)
                                break;

                            current += Direction.East.ToIntVector2();
                        }

                        if (!overlaps)
                        {
                            belts.Add((position, end, Direction.East));
                            current = position;

                            while (true)
                            {
                                beltCells.Add(current);

                                if (current == end)
                                    break;

                                current += Direction.East.ToIntVector2();
                            }
                        }
                    }
                }
            }

            foreach (IntVector2 position in cells)
            {
                bool north = cells.Contains(position + Direction.North.ToIntVector2());
                bool south = cells.Contains(position + Direction.South.ToIntVector2());

                if (!north && south)
                {
                    IntVector2 end = position;

                    while (cells.Contains(end + Direction.South.ToIntVector2()))
                        end += Direction.South.ToIntVector2();

                    if (end != position)
                    {
                        bool overlaps = false;

                        IntVector2 current = position;

                        while (true)
                        {
                            if (beltCells.Contains(current))
                            {
                                overlaps = true;
                                break;
                            }

                            if (current == end)
                                break;

                            current += Direction.South.ToIntVector2();
                        }

                        if (!overlaps)
                        {
                            belts.Add((position, end, Direction.South));
                            current = position;

                            while (true)
                            {
                                beltCells.Add(current);

                                if (current == end)
                                    break;

                                current += Direction.South.ToIntVector2();
                            }
                        }
                    }
                }
            }

            foreach (var belt in belts)
            {
                result.Add(new StructureData(null, belt.start, belt.direction, 0));
                result.Add(new StructureData(null, belt.end, belt.direction, 0));
            }

            if (belts.Count > 0)
            {
                var chosenBelt = belts[random.Next(belts.Count)];
                IntVector2 buttonPosition = chosenBelt.start;
                Direction buttonDirection = chosenBelt.direction;
                Cell buttonCell = lb.Ec.ClosestCellFromPosition(buttonPosition);

                if (!buttonCell.HasWallInDirection(buttonDirection))
                {
                    List<Direction> validDirections = new List<Direction>();

                    foreach (Direction direction in Directions.All())
                    {
                        if (buttonCell.HasWallInDirection(direction))
                            validDirections.Add(direction);
                    }

                    if (validDirections.Count > 0)
                        buttonDirection = validDirections[random.Next(validDirections.Count)];
                }

                result.Add(new StructureData(null, buttonCell.position, buttonDirection, 1));
            }

            return result;
        }

        private static Direction GetDirection(HashSet<IntVector2> cells, IntVector2 position)
        {
            bool north = cells.Contains(position + Direction.North.ToIntVector2());
            bool south = cells.Contains(position + Direction.South.ToIntVector2());
            bool east = cells.Contains(position + Direction.East.ToIntVector2());
            bool west = cells.Contains(position + Direction.West.ToIntVector2());

            int horizontal = 0;
            int vertical = 0;

            if (east || west)
                horizontal++;

            if (north || south)
                vertical++;

            if (horizontal > 0 && vertical == 0)
                return east ? Direction.East : Direction.West;

            if (vertical > 0 && horizontal == 0)
                return south ? Direction.South : Direction.North;

            if (east)
                return Direction.East;

            if (south)
                return Direction.South;

            if (west)
                return Direction.West;

            return Direction.North;
        }

        public override void LoadPreparation(System.Random random)
        {
            base.LoadPreparation(random);
            type = ZoneRuleType.Negative;
            weight = negativeWeight;
            powerBonus = negativePowerBonus;
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.ConveyorBelts;
        public override float negativePowerBonus => 2.1f;
    }
}
