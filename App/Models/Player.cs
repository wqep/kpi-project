using KPI_PROJECT.Models.EnumStates;

namespace KPI_PROJECT.Models;

public class Player
{
    public long TelegramId { get; set; }
    public string Nickname { get; set; }
    public string Class { get; set; }
    public int Level { get; set; } = 1;
    public int MaxHp { get; set; }
    public int HP { get; set; }
    public int Defense { get; set; }
    public int BaseDefense { get; set; }
    public List<ActiveEffect> CurrentEffects { get; set; }
}