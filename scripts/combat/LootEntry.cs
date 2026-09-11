using Godot;

namespace Zeldavania.Combat;

[GlobalClass]
public partial class LootEntry : Resource
{
    [Export]
    public PackedScene CollectibleScene { get; set; }

    [Export]
    public int Weight { get; set; } = 10;
}
