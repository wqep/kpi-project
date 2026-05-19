using Lib.Core.BaseClasses;
using Lib.Core.Models.Skills.SpecialSkills;

namespace Lib.Core.Models.Items.Epic;

public class Grape : BaseItem
{
    public override string Name => "\"Grape\"";
    public override string Description => "It looks like an eye...\n\nAdds Frenzy spell to your arsenal...";
    public override Rarity Rarity => Rarity.Epic;
    public override void AddBonuses(Character character)
    {
        character.Skills.Add(new Frenzy(3));
    }
}