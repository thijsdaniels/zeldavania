using Godot;
using Zeldavania.Inventory;

namespace Zeldavania.UserInterface;

[GlobalClass]
public partial class ItemActionCluster : Control
{
    [Export]
    private ItemActionSlot _slotY;

    [Export]
    private ItemActionSlot _slotX;

    [Export]
    private ItemActionSlot _slotB;

    private Inventory.Inventory _inventory;

    public void Bind(Inventory.Inventory inventory)
    {
        if (_inventory != null)
        {
            _inventory.ItemAssigned -= HandleItemAssigned;
            _inventory.AmmoChanged -= HandleAmmoChanged;
        }

        _inventory = inventory;

        if (_inventory != null)
        {
            _inventory.ItemAssigned += HandleItemAssigned;
            _inventory.AmmoChanged += HandleAmmoChanged;

            _slotX?.SetItem(_inventory.GetItemInSlot(ActionSlot.X));
            _slotY?.SetItem(_inventory.GetItemInSlot(ActionSlot.Y));
            _slotB?.SetItem(_inventory.GetItemInSlot(ActionSlot.B));
        }
    }

    public override void _ExitTree()
    {
        if (_inventory != null)
        {
            _inventory.ItemAssigned -= HandleItemAssigned;
            _inventory.AmmoChanged -= HandleAmmoChanged;
        }
    }

    private void HandleItemAssigned(int slotInt, EquipmentItem item)
    {
        ActionSlot slot = (ActionSlot)slotInt;
        switch (slot)
        {
            case ActionSlot.X:
                _slotX?.SetItem(item);
                break;
            case ActionSlot.Y:
                _slotY?.SetItem(item);
                break;
            case ActionSlot.B:
                _slotB?.SetItem(item);
                break;
        }
    }

    private void HandleAmmoChanged(EquipmentItem item, int currentAmmo, int maxAmmo)
    {
        if (_inventory == null || item == null)
            return;

        if (_inventory.GetItemInSlot(ActionSlot.X) == item)
            _slotX?.UpdateAmmo(item);
        if (_inventory.GetItemInSlot(ActionSlot.Y) == item)
            _slotY?.UpdateAmmo(item);
        if (_inventory.GetItemInSlot(ActionSlot.B) == item)
            _slotB?.UpdateAmmo(item);
    }
}
