using KPI_PROJECT.Models.EnumStates;

namespace KPI_PROJECT.Models.EnemySkills;

public class ThunderstrikeSkill : ISkill
{
    public string Name => "Thunderstrike";
    public int TotalDmg { get; set; }
    public int Multiplier { get; set; }
    
    public ThunderstrikeSkill(int totalDmg)
    {
        TotalDmg = totalDmg;
        Multiplier = 2;
    }

    public void Execute(IBattleUnit.IBattleUnit caster, IBattleUnit.IBattleUnit target)
    {
        var existCharm = target.CurrentEffects.Find(e => e.BattleState == BattleState.Charmed); ; 
        if (existCharm != null)
        {
            TotalDmg *= Multiplier;
        }

        target.HP = Math.Max(0, target.HP - TotalDmg);
    }
}