using UnityEngine;

namespace BaldiPlusRandomZone.ZoneRules
{
    public class ZoneRule_ExtraWindow : ZoneRule
    {
        public ZoneRule_ExtraWindow(int positiveWeight, int negativeWeight) : base(positiveWeight, negativeWeight) { }

        public override void Generate(LevelBuilder lb, System.Random random)
        {
            base.Generate(lb, random);

            if (type == ZoneRuleType.Positive)
            {
                foreach (RoomController room in lb.Ec.rooms)
                {
                    if (room.windowObject == null) continue;
                    foreach (Cell cell in room.cells)
                    {
                        if (!cell.hideFromMap)
                        {
                            for (int i = 0; i < 4; i++)
                                lb.BuildWindowIfPossible(cell.position, room, out RoomController otherRoom, out IntVector2 otherPosition);
                        }
                    }
                }
            }
            else
            {
                Window[] windows = Object.FindObjectsOfType<Window>();

                foreach (Window window in windows)
                {
                    window.gameObject.SetActive(false);
                    lb.Ec.CloseCell(window.aTile.position, window.direction);
                    lb.Ec.CloseCell(window.bTile.position, window.direction.GetOpposite());
                }
            }
        }

        public override void LoadPreparation(System.Random random)
        {
            base.LoadPreparation(random);
            type = ChoseRandomRuleType(random);
        }

        public override ZoneRuleCategory category => ZoneRuleCategory.ExtraWindow;
        public override float positivePowerBonus => 1.2f;
        public override float negativePowerBonus => 1.32f;
    }
}
