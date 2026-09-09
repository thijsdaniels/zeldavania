using Godot;

namespace Zeldavania.Inventory;

[GlobalClass]
public partial class PassiveItem : Node, IInventoryItem
{
    [Export]
    public string ItemId { get; set; } = "";

    [Export]
    public string ItemName { get; set; } = "";

    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; } = "";

    [Export]
    public Texture2D DefaultIcon { get; set; }

    [Export]
    public Godot.Collections.Array<Texture2D> TierIcons { get; set; } = new();

    [Export]
    public int Tier { get; set; } = 1;

    [Export]
    public int MaxTier { get; set; } = 4;

    [Export]
    public bool IsUnlocked { get; set; } = true;

    public Texture2D Icon
    {
        get
        {
            if (TierIcons != null && Tier > 0 && Tier <= TierIcons.Count && TierIcons[Tier - 1] != null)
            {
                return TierIcons[Tier - 1];
            }
            return DefaultIcon;
        }
    }

    public string DisplayName
    {
        get
        {
            if (!IsUnlocked || Tier <= 0)
                return $"[Locked] {ItemName}";

            if (ItemId == "scale")
            {
                return Tier switch
                {
                    1 => "Silver Scale",
                    2 => "Golden Scale",
                    3 => "Zora's Flippers",
                    >= 4 => "Golden Flippers",
                    _ => ItemName
                };
            }

            return MaxTier > 1 ? $"{ItemName} (Tier {Tier})" : ItemName;
        }
    }

    public string DisplayDescription
    {
        get
        {
            if (!IsUnlocked || Tier <= 0)
                return "[Locked] Not yet obtained.";

            if (ItemId == "scale")
            {
                return Tier switch
                {
                    1 => "Silver Scale: Allows shallow diving into water (~1.75 tiles).",
                    2 => "Golden Scale: Allows diving deep into water (~4 tiles).",
                    3 => "Zora's Flippers: Allows free 360-degree swimming underwater. Press Down while paddling to dive.",
                    >= 4 => "Golden Flippers: Allows free 360-degree swimming, swim dashing (Hold A), and scaling waterfalls.",
                    _ => Description
                };
            }

            return Description;
        }
    }

    public void CycleTier()
    {
        if (MaxTier <= 1)
        {
            IsUnlocked = !IsUnlocked;
            Tier = IsUnlocked ? 1 : 0;
            return;
        }

        if (!IsUnlocked || Tier <= 0)
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
    }
}
