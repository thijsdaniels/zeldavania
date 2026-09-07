using Godot;
using Zeldavania.Inventory;

namespace Zeldavania.UserInterface;

[GlobalClass]
public partial class ItemActionSlot : Control
{
    [Export]
    private ActionSlot _slot = ActionSlot.None;

    public ActionSlot Slot => _slot;

    [Export]
    private Label _buttonLabel;

    [Export]
    private TextureRect _iconTexture;

    [Export]
    private Label _ammoLabel;

    [Export]
    private Control _ammoContainer;

    public override void _Ready()
    {
        if (_buttonLabel != null && _slot != ActionSlot.None)
        {
            _buttonLabel.Text = _slot.ToString();
        }
    }

    public void SetItem(EquipmentItem item)
    {
        if (item == null)
        {
            if (_iconTexture != null)
            {
                _iconTexture.Texture = null;
                _iconTexture.Visible = false;
            }
            if (_ammoContainer != null)
            {
                _ammoContainer.Visible = false;
            }
        }
        else
        {
            if (_iconTexture != null)
            {
                _iconTexture.Texture = item.Icon;
                _iconTexture.Visible = item.Icon != null;
            }
            UpdateAmmo(item);
        }
    }

    public void UpdateAmmo(EquipmentItem item)
    {
        if (item != null && item.IsConsumable)
        {
            if (_ammoContainer != null)
            {
                _ammoContainer.Visible = true;
            }
            if (_ammoLabel != null)
            {
                _ammoLabel.Text = item.CurrentAmmo.ToString();
            }
        }
        else
        {
            if (_ammoContainer != null)
            {
                _ammoContainer.Visible = false;
            }
        }
    }
}
