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
    private Label _tierBadge;

    [Export]
    private Texture2D _iconA;

    [Export]
    private Texture2D _iconB;

    [Export]
    private Texture2D _iconX;

    [Export]
    private Texture2D _iconY;

    public IInventoryItem Item { get; private set; }

    public void SetData(IInventoryItem item, ActionSlot assignedSlot, bool isFocused)
    {
        Item = item;

        if (_iconTexture != null)
        {
            _iconTexture.Texture = item?.Icon;
            _iconTexture.Visible = item?.Icon != null;

            if (item != null && (!item.IsUnlocked || item.Tier <= 0))
            {
                _iconTexture.Modulate = new Color(0.35f, 0.35f, 0.35f, 0.35f);
            }
            else
            {
                _iconTexture.Modulate = Colors.White;
            }
        }

        if (_focusPanel != null)
        {
            _focusPanel.Visible = isFocused;
        }

        if (_tierBadge != null)
        {
            if (item != null && item.IsUnlocked && item.Tier > 1)
            {
                _tierBadge.Visible = true;
                _tierBadge.Text = item.Tier switch
                {
                    2 => "II",
                    3 => "III",
                    4 => "IV",
                    _ => item.Tier.ToString()
                };
            }
            else
            {
                _tierBadge.Visible = false;
            }
        }

        bool showAssigned = item is EquipmentItem && item.IsUnlocked && item.Tier > 0;
        if (_assignedBadgeIcon != null)
        {
            if (showAssigned)
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
            else
            {
                _assignedBadgeIcon.Visible = false;
            }
        }
        else if (_assignedBadge != null)
        {
            if (showAssigned && assignedSlot != ActionSlot.None)
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
