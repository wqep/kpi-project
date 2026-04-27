using KPI_PROJECT.Models.EnumStates;

namespace KPI_PROJECT.Models.EnemySkills;

public class FireballSkill : ISkill
{
    public string Name =>  "Fireball";
    public int BaseDmg { get; set; }
    public int Multiplier { get; set; }

    public FireballSkill(int dmg)
    {
        BaseDmg = dmg;
        Multiplier = 2;
    }

    public void Execute(IBattleUnit.IBattleUnit caster, IBattleUnit.IBattleUnit target)
    {
        var existBurning = target.CurrentEffects.Find(e => e.BattleState == BattleState.Burning);
        var existCharm = target.CurrentEffects.Find(e => e.BattleState == BattleState.Charmed);
        int resDamage = BaseDmg + caster.MagicPower;
        if (existBurning != null)
        { 
            existBurning.TurnsLeft = 3;
        }
        else
        {
            target.CurrentEffects.Add(new ActiveEffect(BattleState.Burning, 3));
        }
        
        if (existCharm != null)
        {
            resDamage *= Multiplier;
        }
        
        target.HP =  Math.Max(0, target.HP - resDamage);
        
    }
}