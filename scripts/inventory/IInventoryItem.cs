using Godot;

namespace Zeldavania.Inventory;

public interface IInventoryItem
{
    string ItemId { get; }
    string DisplayName { get; }
    string DisplayDescription { get; }
    Texture2D Icon { get; }
    bool IsUnlocked { get; set; }
    int Tier { get; set; }
    int MaxTier { get; set; }
    void CycleTier();
}
