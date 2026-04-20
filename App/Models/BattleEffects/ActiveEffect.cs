namespace KPI_PROJECT.Models.EnumStates;

public class ActiveEffect
{
    public BattleState BattleState { get; set; }
    public int TurnsLeft { get; set; }
    
    public ActiveEffect(BattleState battleState, int turnsLeft)
    { 
        BattleState = battleState;
        TurnsLeft = turnsLeft;
    }
}