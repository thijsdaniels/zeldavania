using Godot;
using Zeldavania.Combat;

public partial class Enemy : CharacterBody2D
{
    [Export]
    public Area2D HearingArea;

    [Export]
    public Area2D VisionArea;

    [Export]
    public TileDetector2D WaterDetector;

    [Export]
    public Damageable Damageable;

    [Export]
    public Hurtbox Hurtbox;

    [Export]
    public FiniteStateMachine StateMachine;

    public Player Target;

    [Signal]
    public delegate void OnAlertedEventHandler(Player player);

    [Signal]
    public delegate void OnTargetSpottedEventHandler(Player target);

    [Signal]
    public delegate void OnTargetLostEventHandler(Player target);

    public override void _Ready()
    {
        if (HearingArea != null)
            HearingArea.BodyEntered += OnHearingAreaBodyEntered;
        if (VisionArea != null)
        {
            VisionArea.BodyEntered += OnVisionAreaBodyEntered;
            VisionArea.BodyExited += OnVisionAreaBodyExited;
        }
    }

    public void OnHearingAreaBodyEntered(Node body)
    {
        if (body is Player)
            EmitSignal(SignalName.OnAlerted, body);
    }

    public void OnVisionAreaBodyEntered(Node body)
    {
        if (Target == null && body is Player player)
        {
            Target = player;
            EmitSignal(SignalName.OnTargetSpotted, player);
        }
    }

    public void OnVisionAreaBodyExited(Node body)
    {
        if (body == Target)
        {
            Target = null;
            EmitSignal(SignalName.OnTargetLost, body as Player);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
    }
}
