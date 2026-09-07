using System.Collections.Generic;
using Godot;
using Zeldavania.Inventory;

namespace Zeldavania.UserInterface;

[GlobalClass]
public partial class InventoryMenu : Control
{
    [Export]
    private Inventory.Inventory _inventory;

    [Export]
    private Container _gridContainer;

    [Export]
    private Label _itemNameLabel;

    [Export]
    private Label _itemDescriptionLabel;

    [Export]
    private PackedScene _gridSlotPrefab;

    [Export]
    private int _totalSlots = 24;

    [Export]
    private int _columns = 6;

    private readonly List<InventoryGridSlot> _spawnedSlots = new();
    private int _focusedIndex = 0;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        Visible = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("Inventory"))
        {
            if (Visible)
            {
                Close();
            }
            else if (!GetTree().Paused)
            {
                Open();
            }
            GetViewport().SetInputAsHandled();
            return;
        }

        if (!Visible)
            return;

        if (@event.IsActionPressed(Controller.Left))
        {
            MoveFocus(-1);
            GetViewport().SetInputAsHandled();
        }
        else if (@event.IsActionPressed(Controller.Right))
        {
            MoveFocus(1);
            GetViewport().SetInputAsHandled();
        }
        else if (@event.IsActionPressed(Controller.Up))
        {
            MoveFocus(-_columns);
            GetViewport().SetInputAsHandled();
        }
        else if (@event.IsActionPressed(Controller.Down))
        {
            MoveFocus(_columns);
            GetViewport().SetInputAsHandled();
        }
        else if (@event.IsActionPressed(Controller.X))
        {
            AssignFocusedItemToSlot(ActionSlot.X);
            GetViewport().SetInputAsHandled();
        }
        else if (@event.IsActionPressed(Controller.Y))
        {
            AssignFocusedItemToSlot(ActionSlot.Y);
            GetViewport().SetInputAsHandled();
        }
        else if (@event.IsActionPressed(Controller.B))
        {
            AssignFocusedItemToSlot(ActionSlot.B);
            GetViewport().SetInputAsHandled();
        }
        else if (@event.IsActionPressed("ui_cancel") || @event.IsActionPressed("Start"))
        {
            Close();
            GetViewport().SetInputAsHandled();
        }
    }

    public void Bind(Inventory.Inventory inventory)
    {
        _inventory = inventory;
        if (Visible)
        {
            RefreshGrid();
        }
    }

    public void Open()
    {
        GetTree().Paused = true;
        Visible = true;
        RefreshGrid();
    }

    public void Close()
    {
        GetTree().Paused = false;
        Visible = false;
    }

    private void RefreshGrid()
    {
        if (_gridContainer == null)
            return;

        foreach (Node child in _gridContainer.GetChildren())
        {
            child.QueueFree();
        }
        _spawnedSlots.Clear();

        var items = _inventory?.GetUnlockedItems();
        _focusedIndex = Mathf.Clamp(_focusedIndex, 0, _totalSlots - 1);

        for (int i = 0; i < _totalSlots; i++)
        {
            EquipmentItem item = (items != null && i < items.Count) ? items[i] : null;
            ActionSlot assignedSlot = (_inventory != null && item != null)
                ? _inventory.GetSlotOfItem(item)
                : ActionSlot.None;
            bool isFocused = i == _focusedIndex;

            InventoryGridSlot slotInstance = null;
            if (_gridSlotPrefab != null)
            {
                slotInstance = _gridSlotPrefab.Instantiate<InventoryGridSlot>();
            }

            if (slotInstance != null)
            {
                _gridContainer.AddChild(slotInstance);
                slotInstance.SetData(item, assignedSlot, isFocused);
                _spawnedSlots.Add(slotInstance);
            }
        }

        UpdateDetailsBanner();
    }

    private void MoveFocus(int delta)
    {
        if (_spawnedSlots.Count == 0)
            return;

        int newIndex = Mathf.Clamp(_focusedIndex + delta, 0, _spawnedSlots.Count - 1);
        if (newIndex != _focusedIndex)
        {
            _focusedIndex = newIndex;
            UpdateSlotVisuals();
            UpdateDetailsBanner();
        }
    }

    private void UpdateSlotVisuals()
    {
        for (int i = 0; i < _spawnedSlots.Count; i++)
        {
            var slot = _spawnedSlots[i];
            ActionSlot assignedSlot = (_inventory != null && slot.Item != null)
                ? _inventory.GetSlotOfItem(slot.Item)
                : ActionSlot.None;
            slot.SetData(slot.Item, assignedSlot, i == _focusedIndex);
        }
    }

    private void UpdateDetailsBanner()
    {
        if (_focusedIndex >= 0 && _focusedIndex < _spawnedSlots.Count)
        {
            var item = _spawnedSlots[_focusedIndex].Item;
            if (item != null)
            {
                if (_itemNameLabel != null)
                    _itemNameLabel.Text = item.ItemName;
                if (_itemDescriptionLabel != null)
                    _itemDescriptionLabel.Text = item.Description;
                return;
            }
        }

        if (_itemNameLabel != null)
            _itemNameLabel.Text = "";
        if (_itemDescriptionLabel != null)
            _itemDescriptionLabel.Text = "";
    }

    private void AssignFocusedItemToSlot(ActionSlot slot)
    {
        if (_inventory == null || _focusedIndex < 0 || _focusedIndex >= _spawnedSlots.Count)
            return;

        var item = _spawnedSlots[_focusedIndex].Item;
        if (item != null)
        {
            _inventory.AssignItemToSlot(slot, item);
            UpdateSlotVisuals();
        }
    }
}
