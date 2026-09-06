using Godot;

public partial class BatRoosting : State
{
    [ExportGroup("Dependencies")]
    [Export]
    private Enemy _enemy;

    [Export]
    private AnimatedSprite2D _sprite;

    [ExportGroup("Transitions")]
    [Export]
    private State _onWake;

    [ExportGroup("Tuning")]
    [Export]
    private string _animation = "Sleeping";

    public Vector2 RoostPosition { get; set; } = Vector2.Zero;

    private bool _hasInitializedRoost;

    public override void _Ready()
    {
        if (!_hasInitializedRoost && _enemy != null)
        {
            RoostPosition = _enemy.GlobalPosition;
            _hasInitializedRoost = true;
        }
    }

    public override void Enter()
    {
        if (!_hasInitializedRoost && _enemy != null)
        {
            RoostPosition = _enemy.GlobalPosition;
            _hasInitializedRoost = true;
        }

        if (_enemy != null)
        {
            _enemy.Velocity = Vector2.Zero;
            _enemy.OnAlerted += HandleAlerted;
            _enemy.OnTargetSpotted += HandleTargetSpotted;

            if (_enemy.VisionArea != null)
            {
                _enemy.VisionArea.Monitoring = false;
            }
        }

        if (_sprite != null && !string.IsNullOrEmpty(_animation))
        {
            _sprite.Play(_animation);
        }
    }

    public override void Exit()
    {
        if (_enemy != null)
        {
            _enemy.OnAlerted -= HandleAlerted;
            _enemy.OnTargetSpotted -= HandleTargetSpotted;

            if (_enemy.VisionArea != null)
            {
                _enemy.VisionArea.Monitoring = true;
            }
        }
    }

    private void HandleAlerted(Player player)
    {
        if (_enemy != null && _enemy.Target == null)
        {
            _enemy.Target = player;
        }

        if (_onWake != null)
        {
            Transition(_onWake);
        }
    }

    private void HandleTargetSpotted(Player player)
    {
        if (_onWake != null)
        {
            Transition(_onWake);
        }
    }
}
