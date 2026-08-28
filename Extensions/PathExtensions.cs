
namespace BaldiPlusRandomZone.Extensions
{
    public static class PathExtensions
    {
        public static bool CheckRoomTrap(this Cell cell, RoomController roomNeedToBeConnected)
        {
            foreach (Direction dir in Directions.All())
            {
                IntVector2 position = cell.position + dir.ToIntVector2();

                if (cell.room.ec.ContainsCoordinates(position) && cell.room.ec.CellFromPosition(position).room == roomNeedToBeConnected)
                    return false;
            }

            return true;
        }
    }
}
