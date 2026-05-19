namespace Lib.Infrastructure.Services;

public static class TurnsHelper
{
    public static int GetTurnsForFloor(int location, int floor)
    {
        return (location, floor) switch
        {
            (1, 1) => 25,
            (1, 2) => 35,
            (1, 3) => 45,
            (2, 1) => 35,
            (2, 2) => 45,
            (2, 3) => 55,
            (3, 1) => 45,
            (3, 2) => 55,
            (3, 3) => 65,
            _ => 25
        };
    }
}