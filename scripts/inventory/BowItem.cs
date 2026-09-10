using Godot;

namespace Zeldavania.Inventory;

public enum ArrowVariant
{
    Standard,
    Fire,
    Ice,
    Light,
}

[GlobalClass]
public partial class BowItem : EquipmentItem
{
    [Export]
    private PlayerAiming _aimingState;

    [Export]
    public Texture2D FireBowIcon { get; set; }

    [Export]
    public Texture2D IceBowIcon { get; set; }

    [Export]
    public Texture2D LightBowIcon { get; set; }

    public ArrowVariant ActiveVariant =>
        Tier switch
        {
            2 => ArrowVariant.Fire,
            3 => ArrowVariant.Ice,
            4 => ArrowVariant.Light,
            _ => ArrowVariant.Standard,
        };

    public override Texture2D Icon =>
        ActiveVariant switch
        {
            ArrowVariant.Fire when FireBowIcon != null => FireBowIcon,
            ArrowVariant.Ice when IceBowIcon != null => IceBowIcon,
            ArrowVariant.Light when LightBowIcon != null => LightBowIcon,
            _ => DefaultIcon,
        };

    public override string DisplayName
    {
        get
        {
            if (!IsUnlocked || Tier <= 0)
            {
                return $"[Locked] {ItemName}";
            }

            return ActiveVariant switch
            {
                ArrowVariant.Fire => "Fire Bow",
                ArrowVariant.Ice => "Ice Bow",
                ArrowVariant.Light => "Light Bow",
                _ => ItemName,
            };
        }
    }

    public override string DisplayDescription
    {
        get
        {
            if (!IsUnlocked || Tier <= 0)
            {
                return "[Locked] Not yet obtained.";
            }

            return ActiveVariant switch
            {
                ArrowVariant.Fire =>
                    "Fires blazing fire arrows that ignite flammable objects.",
                ArrowVariant.Ice => "Fires freezing ice arrows.",
                ArrowVariant.Light => "Fires sacred arrows of light.",
                _ => Description,
            };
        }
    }

    public override State Use(Player player, string actionButton)
    {
        if (CurrentAmmo <= 0 || _aimingState == null)
        {
            return null;
        }

        _aimingState.TriggerAction = actionButton;
        return _aimingState;
    }
}
