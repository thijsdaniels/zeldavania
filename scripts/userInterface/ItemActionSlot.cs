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
    private TextureRect _buttonIcon;

    [Export]
    private Texture2D _iconA;

    [Export]
    private Texture2D _iconB;

    [Export]
    private Texture2D _iconX;

    [Export]
    private Texture2D _iconY;

    [Export]
    private TextureRect _iconTexture;

    [Export]
    private Label _ammoLabel;

    [Export]
    private Control _ammoContainer;

    public override void _Ready()
    {
        UpdateSlotBadge();
    }

    private void UpdateSlotBadge()
    {
        if (_buttonIcon != null)
        {
            _buttonIcon.Texture = _slot switch
            {
                ActionSlot.X => _iconX,
                ActionSlot.Y => _iconY,
                ActionSlot.B => _iconB,
                _ => null
            };
            _buttonIcon.Visible = _buttonIcon.Texture != null;
        }

        if (_buttonLabel != null && _slot != ActionSlot.None)
        {
            _buttonLabel.Text = _slot.ToString();
            _buttonLabel.Visible = _buttonIcon == null || _buttonIcon.Texture == null;
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
