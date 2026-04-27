namespace KPI_PROJECT.Models.EnemySkills;

public interface ISkill
{
    public string Name {get;}
    public void Execute(IBattleUnit.IBattleUnit casteer, IBattleUnit.IBattleUnit target);
}