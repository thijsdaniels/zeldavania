using Godot;
using Zeldavania.Inventory;

namespace Zeldavania.Objects;

[GlobalClass]
public partial class ArrowCollectible2D : Collectible2D
{
    [Export]
    public int Amount { get; set; } = 10;

    public override void _Ready()
    {
        base._Ready();

        var sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
        if (sprite != null)
        {
            sprite.Hframes = 3;
            sprite.Frame = Amount switch
            {
                <= 10 => 0,
                <= 20 => 1,
                _ => 2,
            };
        }
    }

    protected override bool OnCollect(Node2D collector)
    {
        var inventory = collector.GetNodeOrNull<Inventory.Inventory>("Inventory")
                     ?? collector.FindChild("Inventory") as Inventory.Inventory;

        if (inventory == null)
        {
            return false;
        }

        var bow = inventory.GetNodeOrNull<BowItem>("Bow")
               ?? inventory.FindChild("Bow") as BowItem;

        if (bow != null)
        {
            inventory.AddAmmo(bow, Amount);
            return true;
        }

        return false;
    }
}
