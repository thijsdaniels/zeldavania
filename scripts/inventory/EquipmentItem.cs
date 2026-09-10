using Godot;

namespace Zeldavania.Inventory;

[GlobalClass]
public partial class EquipmentItem : Node, IInventoryItem
{
    [Signal]
    public delegate void ItemChangedEventHandler();

    [Export]
    public string ItemId { get; set; } = "";

    [Export]
    public string ItemName { get; set; } = "";

    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; } = "";

    [Export]
    public Texture2D DefaultIcon { get; set; }

    [Export]
    public int Tier { get; set; } = 1;

    [Export]
    public int MaxTier { get; set; } = 1;

    [Export]
    public bool IsUnlocked { get; set; } = true;

    [Export]
    public bool IsConsumable { get; set; }

    [Export]
    public int MaxAmmo { get; set; }

    [Export]
    public int CurrentAmmo { get; set; }

    public virtual Texture2D Icon => DefaultIcon;

    public virtual string DisplayName =>
        (!IsUnlocked || Tier <= 0) ? $"[Locked] {ItemName}" : ItemName;

    public virtual string DisplayDescription =>
        (!IsUnlocked || Tier <= 0) ? "[Locked] Not yet obtained." : Description;

    public virtual State Use(Player player, string actionButton) => null;

    public void CycleTier()
    {
        if (MaxTier <= 1)
        {
            IsUnlocked = !IsUnlocked;
            Tier = IsUnlocked ? 1 : 0;
        }
        else if (!IsUnlocked || Tier <= 0)
        {
            IsUnlocked = true;
            Tier = 1;
        }
        else if (Tier < MaxTier)
        {
            Tier++;
        }
        else
        {
            IsUnlocked = false;
            Tier = 0;
        }

        EmitSignal(SignalName.ItemChanged);
    }
}
