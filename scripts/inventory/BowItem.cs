using Godot;

namespace Zeldavania.Inventory;

[GlobalClass]
public partial class BowItem : EquipmentItem
{
    [Export]
    private PlayerAiming _aimingState;

    public override State Use(Player player, string actionButton)
    {
        if (CurrentAmmo <= 0 || _aimingState == null)
        {
            return null;
        }

        _aimingState.TriggerAction = actionButton;
        return _aimingState;
    }
}
