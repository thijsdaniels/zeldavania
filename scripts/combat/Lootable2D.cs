using Godot;
using Zeldavania.Objects;
using Zeldavania.Utilities;

namespace Zeldavania.Combat;

[GlobalClass]
public partial class Lootable2D : Node2D
{
    [Signal]
    public delegate void LootDroppedEventHandler(Collectible2D collectible);

    [Signal]
    public delegate void LootCollectedEventHandler(Node2D collector);

    [Export]
    public Godot.Collections.Array<LootEntry> DropTable { get; set; } = new();

    [Export]
    public int EmptyDropWeight { get; set; } = 0;

    [Export]
    public bool DropOnDestroy { get; set; } = true;

    [Export]
    public Damageable DamageableNode { get; set; }

    [Export]
    public bool LootOnInteract { get; set; } = false;

    [Export]
    public Interactable2D InteractableNode { get; set; }

    [Export]
    public Vector2 PopVelocityMin { get; set; } = new Vector2(0, -75);

    [Export]
    public Vector2 PopVelocityMax { get; set; } = new Vector2(0, -60);

    [Export]
    public bool SpawnWithLifetime { get; set; } = true;

    private bool _hasLooted;

    public override void _Ready()
    {
        if (DropOnDestroy)
        {
            if (DamageableNode == null)
            {
                DamageableNode = GetNodeOrNull<Damageable>("Damageable")
                              ?? GetParent()?.GetNodeOrNull<Damageable>("Damageable");
            }

            if (DamageableNode != null)
            {
                DamageableNode.OnDepleted += HandleDamageableDepleted;
            }
        }

        if (LootOnInteract)
        {
            if (InteractableNode == null)
            {
                InteractableNode = GetNodeOrNull<Interactable2D>("Interactable2D")
                                ?? GetParent()?.GetNodeOrNull<Interactable2D>("Interactable2D");
            }

            if (InteractableNode != null)
            {
                InteractableNode.Interacted += HandleInteracted;
            }
        }
    }

    public override void _ExitTree()
    {
        if (DamageableNode != null)
        {
            DamageableNode.OnDepleted -= HandleDamageableDepleted;
        }

        if (InteractableNode != null)
        {
            InteractableNode.Interacted -= HandleInteracted;
        }
    }

    private void HandleDamageableDepleted()
    {
        if (_hasLooted)
        {
            return;
        }

        _hasLooted = true;
        SpawnDrop();
    }

    private void HandleInteracted(Player player)
    {
        if (_hasLooted)
        {
            return;
        }

        _hasLooted = true;
        LootDirectly(player);
    }

    public PackedScene RollDrop()
    {
        int totalWeight = EmptyDropWeight;
        foreach (var entry in DropTable)
        {
            if (entry?.CollectibleScene != null && entry.Weight > 0)
            {
                totalWeight += entry.Weight;
            }
        }

        if (totalWeight <= 0)
        {
            return null;
        }

        int roll = GD.RandRange(0, totalWeight - 1);
        int current = 0;

        foreach (var entry in DropTable)
        {
            if (entry?.CollectibleScene != null && entry.Weight > 0)
            {
                current += entry.Weight;
                if (roll < current)
                {
                    return entry.CollectibleScene;
                }
            }
        }

        return null; // Empty drop roll
    }

    public Collectible2D SpawnDrop()
    {
        PackedScene scene = RollDrop();
        if (scene == null)
        {
            return null;
        }

        Node instance = scene.Instantiate();
        if (instance is Collectible2D collectible)
        {
            collectible.GlobalPosition = GlobalPosition;

            float vx = (float)GD.RandRange(PopVelocityMin.X, PopVelocityMax.X);
            float vy = (float)GD.RandRange(PopVelocityMin.Y, PopVelocityMax.Y);
            collectible.InitialVelocity = new Vector2(vx, vy);
            collectible.HasLifetime = SpawnWithLifetime;

            // Spawn into world level / parent environment
            Node spawnTarget = GetTree()?.CurrentScene ?? GetParent();
            spawnTarget?.AddChild(collectible);

            EmitSignal(SignalName.LootDropped, collectible);
            return collectible;
        }

        instance.QueueFree();
        return null;
    }

    public bool LootDirectly(Node2D collector)
    {
        PackedScene scene = RollDrop();
        if (scene == null)
        {
            return false;
        }

        Node instance = scene.Instantiate();
        if (instance is Collectible2D collectible)
        {
            collectible.GlobalPosition = GlobalPosition;
            GetTree()?.CurrentScene?.AddChild(collectible);
            bool collected = collectible.Collect(collector);
            if (collected)
            {
                EmitSignal(SignalName.LootCollected, collector);
            }
            return collected;
        }

        instance.QueueFree();
        return false;
    }
}
