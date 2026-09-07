using Godot;

namespace Zeldavania.Inventory;

[GlobalClass]
public partial class EquipmentItem : Node
{
    [Export]
    public string ItemId { get; set; } = "";

    [Export]
    public string ItemName { get; set; } = "";

    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; } = "";

    [Export]
    public Texture2D Icon { get; set; }

    [Export]
    public bool IsConsumable { get; set; }

    [Export]
    public int MaxAmmo { get; set; }

    [Export]
    public int CurrentAmmo { get; set; }

    public virtual State Use(Player player, string actionButton) => null;
}
