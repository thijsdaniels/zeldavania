using Godot;

namespace Zeldavania.Inventory;

[GlobalClass]
public partial class SwordItem : EquipmentItem
{
    [Export]
    private State _attackState;

    public override State Use(Player player, string actionButton) => _attackState;
}
