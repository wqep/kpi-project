using Lib.Core.BaseClasses;

namespace Lib.Core.Models.Items.Legendary;

public class SandWatch : BaseItem
{
    public override string Name => "Sand Watch";
    public override string Description => "Why are they ticking?...\n\nAdds 10 bonus turns...";
    public override Rarity Rarity => Rarity.Legendary;
    public override void AddBonuses(Character character)
    {
        character.MaxTurns += 10;
        character.TurnsLeft += 10;
    }
}