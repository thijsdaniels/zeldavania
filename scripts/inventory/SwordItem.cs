using Godot;

namespace Zeldavania.Inventory;

[GlobalClass]
public partial class SwordItem : EquipmentItem
{
    [Export]
    private State _attackState;

    public override State GetActionState() => _attackState;

    public override bool CanUse(Player player) => true;
}
