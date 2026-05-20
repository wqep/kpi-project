using Lib.Core.BaseClasses;

namespace Lib.Core.Models.Items.Legendary;

public class Jocker : BaseItem
{
    public override string Name => "Jocker";
    public override string Description => "Jimbo? Is it you?...\n\nYou have 1 in 54 chance to instantly defeat an enemy...";
    public override Rarity Rarity => Rarity.Legendary;
    public override void AddBonuses(Character character)
    {
        
    }
}