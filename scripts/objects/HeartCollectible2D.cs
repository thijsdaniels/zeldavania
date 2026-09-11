using Godot;
using Zeldavania.Combat;

namespace Zeldavania.Objects;

[GlobalClass]
public partial class HeartCollectible2D : Collectible2D
{
    [Export]
    public int HealAmount { get; set; } = 4;

    protected override bool OnCollect(Node2D collector)
    {
        var damageable = collector.GetNodeOrNull<Damageable>("Damageable")
                      ?? collector.FindChild("Damageable") as Damageable;

        if (damageable != null)
        {
            damageable.Heal(HealAmount);
            return true;
        }

        return false;
    }
}
