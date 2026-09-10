using Godot;

namespace Zeldavania.Combat;

public enum BurnEndBehavior
{
    Extinguish,
    DestroyEntity,
    TriggerSignalOnly,
}

[GlobalClass]
public partial class Flammable2D : Area2D
{
    [Signal]
    public delegate void IgnitedEventHandler();

    [Signal]
    public delegate void ExtinguishedEventHandler();

    [Signal]
    public delegate void BurnCompletedEventHandler();

    [Export]
    public bool IsBurning { get; set; } = false;

    [Export]
    public float BurnDuration { get; set; } = 0f;

    [Export]
    public BurnEndBehavior OnBurnComplete { get; set; } =
        BurnEndBehavior.Extinguish;

    [Export]
    public bool ExtinguishInWater { get; set; } = true;

    [Export]
    private Node2D _flameVisuals;

    private float _currentBurnTimer = 0f;

    public override void _Ready()
    {
        AreaEntered += HandleAreaEntered;
        BodyEntered += HandleBodyEntered;

        UpdateVisuals();
    }

    public override void _ExitTree()
    {
        AreaEntered -= HandleAreaEntered;
        BodyEntered -= HandleBodyEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsBurning)
        {
            return;
        }

        if (BurnDuration > 0f)
        {
            _currentBurnTimer += (float)delta;
            if (_currentBurnTimer >= BurnDuration)
            {
                CompleteBurn();
                return;
            }
        }

        PropagateIgnition();

        if (ExtinguishInWater)
        {
            CheckWaterCollision();
        }
    }

    public void Ignite()
    {
        if (IsBurning)
        {
            return;
        }

        IsBurning = true;
        _currentBurnTimer = 0f;
        UpdateVisuals();
        EmitSignal(SignalName.Ignited);
    }

    public void Extinguish()
    {
        if (!IsBurning)
        {
            return;
        }

        IsBurning = false;
        _currentBurnTimer = 0f;
        UpdateVisuals();
        EmitSignal(SignalName.Extinguished);
    }

    private void CompleteBurn()
    {
        EmitSignal(SignalName.BurnCompleted);

        switch (OnBurnComplete)
        {
            case BurnEndBehavior.Extinguish:
                Extinguish();
                break;
            case BurnEndBehavior.DestroyEntity:
                var target = GetParent() ?? this;
                target.QueueFree();
                break;
            case BurnEndBehavior.TriggerSignalOnly:
                break;
        }
    }

    private void HandleAreaEntered(Area2D area)
    {
        if (area is Flammable2D otherFlammable)
        {
            if (IsBurning && !otherFlammable.IsBurning)
            {
                otherFlammable.Ignite();
            }
            else if (!IsBurning && otherFlammable.IsBurning)
            {
                Ignite();
            }
        }
    }

    private void HandleBodyEntered(Node2D body)
    {
        if (ExtinguishInWater && IsBurning)
        {
            // Check if body is in Liquids layer (Layer 3, mask bit 4)
            if (body is TileMapLayer layer && (CollisionMask & 4) != 0)
            {
                Extinguish();
            }
        }
    }

    private void PropagateIgnition()
    {
        var areas = GetOverlappingAreas();
        foreach (var area in areas)
        {
            if (area is Flammable2D other && !other.IsBurning)
            {
                other.Ignite();
            }
        }
    }

    private void CheckWaterCollision()
    {
        var bodies = GetOverlappingBodies();
        foreach (var body in bodies)
        {
            if (body is TileMapLayer)
            {
                // Verify layer overlap
                Extinguish();
                break;
            }
        }
    }

    private void UpdateVisuals()
    {
        if (_flameVisuals != null)
        {
            _flameVisuals.Visible = IsBurning;
            if (_flameVisuals is GpuParticles2D gpuParticles)
            {
                gpuParticles.Emitting = IsBurning;
            }
            else if (_flameVisuals is CpuParticles2D cpuParticles)
            {
                cpuParticles.Emitting = IsBurning;
            }
        }

        foreach (Node child in GetChildren())
        {
            if (child is GpuParticles2D gpu)
            {
                gpu.Emitting = IsBurning;
                gpu.Visible = IsBurning;
            }
            else if (child is CpuParticles2D cpu)
            {
                cpu.Emitting = IsBurning;
                cpu.Visible = IsBurning;
            }
        }
    }
}
