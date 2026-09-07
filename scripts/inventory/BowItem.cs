using Godot;

namespace Zeldavania.Inventory;

[GlobalClass]
public partial class BowItem : EquipmentItem
{
    [Export]
    private State _aimingState;

    public override State GetActionState() => _aimingState;

    public override bool CanUse(Player player) => CurrentAmmo > 0;
}
