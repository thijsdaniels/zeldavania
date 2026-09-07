using Godot;
using Zeldavania.Inventory;

namespace Zeldavania.Extensions;

public static class StateItemExtensions
{
    private static readonly (ActionSlot Slot, string ActionName)[] ActionSlots =
    {
        (ActionSlot.X, Controller.X),
        (ActionSlot.Y, Controller.Y),
        (ActionSlot.B, Controller.B),
    };

    public static bool TryTriggerItemAction(this State state, Player player, out State targetState)
    {
        targetState = null;
        if (player == null || player.Inventory == null)
            return false;

        foreach (var (slot, actionName) in ActionSlots)
        {
            if (Input.IsActionJustPressed(actionName))
            {
                var item = player.Inventory.GetItemInSlot(slot);
                if (item != null && item.CanUse(player))
                {
                    var actionState = item.GetActionState();
                    if (actionState != null)
                    {
                        if (actionState is IActionTriggerable triggerable)
                        {
                            if (!triggerable.TryInitialize(player, actionName))
                            {
                                continue;
                            }
                        }

                        targetState = actionState;
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
