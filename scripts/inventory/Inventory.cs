using System.Collections.Generic;
using Godot;

namespace Zeldavania.Inventory;

[GlobalClass]
public partial class Inventory : Node
{
    [Signal]
    public delegate void ItemAssignedEventHandler(int slot, EquipmentItem item);

    [Signal]
    public delegate void AmmoChangedEventHandler(EquipmentItem item, int currentAmmo, int maxAmmo);

    [Export]
    private EquipmentItem _slotXItem;

    [Export]
    private EquipmentItem _slotYItem;

    [Export]
    private EquipmentItem _slotBItem;

    private readonly List<EquipmentItem> _registeredItems = new();

    public override void _Ready()
    {
        _registeredItems.Clear();
        foreach (Node child in GetChildren())
        {
            if (child is EquipmentItem item && !_registeredItems.Contains(item))
            {
                _registeredItems.Add(item);
            }
        }
    }

    public EquipmentItem GetItemInSlot(ActionSlot slot)
    {
        return slot switch
        {
            ActionSlot.X => _slotXItem,
            ActionSlot.Y => _slotYItem,
            ActionSlot.B => _slotBItem,
            _ => null,
        };
    }

    public ActionSlot GetSlotOfItem(EquipmentItem item)
    {
        if (item == null)
            return ActionSlot.None;

        if (_slotXItem == item)
            return ActionSlot.X;
        if (_slotYItem == item)
            return ActionSlot.Y;
        if (_slotBItem == item)
            return ActionSlot.B;

        return ActionSlot.None;
    }

    public void AssignItemToSlot(ActionSlot targetSlot, EquipmentItem item)
    {
        if (targetSlot == ActionSlot.None)
            return;

        ActionSlot currentSlotOfNewItem = GetSlotOfItem(item);
        EquipmentItem existingItemInTargetSlot = GetItemInSlot(targetSlot);

        if (currentSlotOfNewItem != ActionSlot.None && currentSlotOfNewItem != targetSlot)
        {
            // Swap: move existing item to the other slot
            SetSlotItem(currentSlotOfNewItem, existingItemInTargetSlot);
            SetSlotItem(targetSlot, item);

            EmitSignal(SignalName.ItemAssigned, (int)currentSlotOfNewItem, existingItemInTargetSlot);
            EmitSignal(SignalName.ItemAssigned, (int)targetSlot, item);
        }
        else
        {
            SetSlotItem(targetSlot, item);
            EmitSignal(SignalName.ItemAssigned, (int)targetSlot, item);
        }
    }

    private void SetSlotItem(ActionSlot slot, EquipmentItem item)
    {
        switch (slot)
        {
            case ActionSlot.X:
                _slotXItem = item;
                break;
            case ActionSlot.Y:
                _slotYItem = item;
                break;
            case ActionSlot.B:
                _slotBItem = item;
                break;
        }
    }

    public bool ConsumeAmmo(EquipmentItem item, int amount = 1)
    {
        if (item == null || !item.IsConsumable)
            return true;

        if (item.CurrentAmmo >= amount)
        {
            item.CurrentAmmo -= amount;
            EmitSignal(SignalName.AmmoChanged, item, item.CurrentAmmo, item.MaxAmmo);
            return true;
        }

        return false;
    }

    public void AddAmmo(EquipmentItem item, int amount)
    {
        if (item == null || !item.IsConsumable)
            return;

        item.CurrentAmmo = Mathf.Clamp(item.CurrentAmmo + amount, 0, item.MaxAmmo);
        EmitSignal(SignalName.AmmoChanged, item, item.CurrentAmmo, item.MaxAmmo);
    }

    public void UnassignItem(EquipmentItem item)
    {
        if (item == null)
            return;

        ActionSlot slot = GetSlotOfItem(item);
        if (slot != ActionSlot.None)
        {
            SetSlotItem(slot, null);
            EmitSignal(SignalName.ItemAssigned, (int)slot, (EquipmentItem)null);
        }
    }

    public IReadOnlyList<IInventoryItem> GetAllItems()
    {
        var allItems = new List<IInventoryItem>();
        foreach (Node child in GetChildren())
        {
            if (child is IInventoryItem item)
            {
                allItems.Add(item);
            }
        }
        return allItems;
    }

    public IReadOnlyList<EquipmentItem> GetUnlockedItems()
    {
        var unlocked = new List<EquipmentItem>();
        foreach (Node child in GetChildren())
        {
            if (child is EquipmentItem item && item.IsUnlocked)
            {
                unlocked.Add(item);
            }
        }
        return unlocked;
    }

    public int GetPassiveTier(string itemId)
    {
        foreach (Node child in GetChildren())
        {
            if (child is PassiveItem passive && passive.ItemId == itemId && passive.IsUnlocked)
            {
                return passive.Tier;
            }
        }
        return 0;
    }

    public bool HasPassive(string itemId, int minTier = 1)
    {
        return GetPassiveTier(itemId) >= minTier;
    }
}
