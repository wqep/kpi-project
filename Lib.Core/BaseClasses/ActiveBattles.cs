namespace Lib.Core.BaseClasses;

public class ActiveBattle
{
    public int CharacterId { get; set; }
    public string EnemyName { get; set; } = string.Empty;
    public int CurrentHp { get; set; }
    public int MaxHp { get; set; }
    public int BaseDamage { get; set; }
}