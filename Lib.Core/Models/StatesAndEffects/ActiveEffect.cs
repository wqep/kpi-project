namespace Lib.Core.Models.StatesAndEffects;

public class ActiveEffect
{
    public BattleStateEnum BattleStateEnum { get; set; }
    public int TurnsLeft { get; set; }
    
    public ActiveEffect(BattleStateEnum battleStateEnum, int turnsLeft)
    { 
        BattleStateEnum = battleStateEnum;
        TurnsLeft = turnsLeft;
    }
}