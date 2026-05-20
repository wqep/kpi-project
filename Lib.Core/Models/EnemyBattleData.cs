using Lib.Core.Models.StatesAndEffects;

namespace Lib.Core.Models;

public class EnemyBattleData
{
    public string Name { get; set; } = string.Empty;
    public string ClassType { get; set; } = string.Empty;
    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public List<ActiveEffect> Effects { get; set; } = new();
}