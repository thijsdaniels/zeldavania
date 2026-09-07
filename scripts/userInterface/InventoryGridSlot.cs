using Godot;
using Zeldavania.Inventory;

namespace Zeldavania.UserInterface;

[GlobalClass]
public partial class InventoryGridSlot : Control
{
    [Export]
    private TextureRect _iconTexture;

    [Export]
    private Panel _focusPanel;

    [Export]
    private Label _assignedBadge;

    [Export]
    private TextureRect _assignedBadgeIcon;

    [Export]
    private Texture2D _iconA;

    [Export]
    private Texture2D _iconB;

    [Export]
    private Texture2D _iconX;

    [Export]
    private Texture2D _iconY;

    public EquipmentItem Item { get; private set; }

    public void SetData(EquipmentItem item, ActionSlot assignedSlot, bool isFocused)
    {
        Item = item;

        if (_iconTexture != null)
        {
            _iconTexture.Texture = item?.Icon;
            _iconTexture.Visible = item?.Icon != null;
        }

        if (_focusPanel != null)
        {
            _focusPanel.Visible = isFocused;
        }

        if (_assignedBadgeIcon != null)
        {
            _assignedBadgeIcon.Texture = assignedSlot switch
            {
                ActionSlot.X => _iconX,
                ActionSlot.Y => _iconY,
                ActionSlot.B => _iconB,
                _ => null
            };
            _assignedBadgeIcon.Visible = _assignedBadgeIcon.Texture != null;
        }
        else if (_assignedBadge != null)
        {
            if (assignedSlot != ActionSlot.None)
            {
                _assignedBadge.Visible = true;
                _assignedBadge.Text = assignedSlot.ToString();
            }
            else
            {
                _assignedBadge.Visible = false;
            }
        }
    }
}
