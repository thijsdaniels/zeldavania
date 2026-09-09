using Godot;
using Zeldavania.Extensions;

public partial class PlayerWallSliding : State
{
    [Export]
    private CharacterBody2D _body;

    [Export]
    private Player _player;

    [Export]
    private AnimatedSprite2D _sprite;

    [ExportGroup("Wall Sliding")]
    [Export]
    private float _slideGravity = 400f;

    [Export]
    private float _slideTerminalVelocity = 45f;

    [Export]
    private float _coyoteTime = 0.08f;

    [ExportGroup("Transitions")]
    [Export]
    private State _fallingState;

    [Export]
    private State _landingState;

    [Export]
    private State _wallJumpingState;

    [Export]
    private State _swimmingState;

    [Export]
    private WaterDetector2D _waterDetector;

    private float _coyoteTimer;
    private float _wallNormalX;

    public override void _Ready()
    {
        if (_player == null && _body is Player player)
        {
            _player = player;
        }
    }

    public override void Enter()
    {
        _coyoteTimer = _coyoteTime;

        if (_body.IsOnWall())
        {
            _wallNormalX = _body.GetWallNormal().X;
        }
        else
        {
            float inputDir = Controller.GetHorizontalDirection();
            _wallNormalX = inputDir != 0 ? -inputDir : (_sprite.FlipH ? 1f : -1f);
        }

        _sprite.Play("WallSlide");
        _sprite.FlipH = _wallNormalX < 0;
    }

    public override void UpdatePhysics(double delta)
    {
        switch (true)
        {
            case true when this.TryTriggerItemAction(_player, out State targetState):
                Transition(targetState);
                break;

            case true when Input.IsActionJustPressed(Controller.A):
                if (_wallJumpingState is PlayerWallJumping wallJumping)
                {
                    wallJumping.SetWallNormal(_wallNormalX);
                }
                Transition(_wallJumpingState);
                break;

            case true when _waterDetector != null && _waterDetector.IsOverlapping:
                Transition(_swimmingState);
                break;

            case true when _body.IsOnFloor():
                Transition(_landingState);
                break;

            default:
                float inputDir = Controller.GetHorizontalDirection();
                bool isHoldingIntoWall = (inputDir * _wallNormalX) < 0;

                if (_body.IsOnWall() && isHoldingIntoWall)
                {
                    _coyoteTimer = _coyoteTime;
                    _wallNormalX = _body.GetWallNormal().X;
                    _sprite.FlipH = _wallNormalX < 0;
                }
                else
                {
                    _coyoteTimer -= (float)delta;
                    if (_coyoteTimer <= 0)
                    {
                        Transition(_fallingState);
                        return;
                    }
                }

                Slide(delta);
                break;
        }
    }

    private void Slide(double delta)
    {
        float targetY = _slideTerminalVelocity;
        float newY = Mathf.MoveToward(
            _body.Velocity.Y,
            targetY,
            _slideGravity * (float)delta
        );

        // Keep a slight inward horizontal pressure to maintain collision against the wall
        _body.Velocity = new Vector2(-_wallNormalX * 20f, newY);
    }
}
