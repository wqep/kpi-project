using Lib.Core.Enums;
using Lib.Core.Interfaces;
using Lib.Core.Models.Items;
using Lib.Core.Models.Skills.DefaultSkills;
using Lib.Core.Models.StatesAndEffects;

namespace Lib.Core.BaseClasses;

public class Character : IBattleUnit
{
    public int Id { get; set; } 
    
    public long TelegramId { get; set; } 
    
    public string Class { get; set; }
    public int Level { get; set; } = 1;
    public LocationName LocationName { get; set; } 
    
    public int MaxHp { get; set; }
    public int Hp { get; set; }
    
    public int BasePhisDefense { get; set; }
    public int PhisDefense { get; set; }

    public List<ISkill> Skills { get; set; } = new List<ISkill>()
    {
        new HandAttack(),
        new Defend()
    };
    public List<ActiveEffect> CurrentEffects { get; set; } = new();
    public List<BaseItem> Items { get; set; } = new();
    
    public int HandDmg { get; set; }
    public int MagicPower { get; set; }

    public int CurrentRoomId { get; set; }
    public int State { get; set; } = 0;
    public int Location { get; set; } = 1;
    public int Floor { get; set; } = 1;
    public int TurnsLeft { get; set; } = 25;
    public int MapWidth { get; set; } = 4;
    public int MapHeight { get; set; } = 3;
}