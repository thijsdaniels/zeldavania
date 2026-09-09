using Godot;
using Zeldavania.Extensions;

public partial class PlayerFalling : State
{
    [Export]
    private CharacterBody2D _body;

    [Export]
    private Player _player;

    [Export]
    private AnimatedSprite2D _sprite;

    [ExportGroup("Gravity")]
    [Export]
    private float _gravity = 500;

    [Export]
    private float _terminalVelocity = 220;

    [ExportGroup("Movement")]
    [Export]
    private float _acceleration = 1200;

    [Export]
    private float _deceleration = 100;

    [Export]
    private float _maximumVelocity = 70;

    [ExportGroup("Jumping & Aerial Tuning")]
    [Export]
    private State _jumpingState;

    [Export]
    private float _coyoteTime = 0.10f;

    [Export]
    private float _jumpBufferTime = 0.10f;

    [Export]
    private float _jumpCutMultiplier = 0.5f;

    [ExportGroup("Air Jumping")]
    [Export]
    private State _airJumpingState;

    [Export]
    private int _airJumps = 1;

    [ExportGroup("Landing")]
    [Export]
    private State _landingState;

    [ExportGroup("Wall Sliding")]
    [Export]
    private State _wallSlidingState;

    [ExportGroup("Swimming")]
    [Export]
    private State _swimmingState;

    [Export]
    private WaterDetector2D _waterDetector;

    [Export]
    private WaterfallDetector2D _waterfallDetector;

    [ExportGroup("Climbing")]
    [Export]
    private State _climbingState;

    [Export]
    private TileDetector2D _ladderDetector;

    private int _airJumpsRemaining;
    private float _dropGraceTimer;
    private float _inputLockoutTimer;
    private float _lockedDirection;
    private float _coyoteTimer;
    private float _jumpBufferTimer;
    private bool _isAscendingFromJump;

    public void SetInputLockout(float duration, float lockedDirection)
    {
        _inputLockoutTimer = duration;
        _lockedDirection = lockedDirection;
    }

    public void EnableCoyoteTime()
    {
        _coyoteTimer = _coyoteTime;
    }

    public void NotifyJumpAscent()
    {
        _isAscendingFromJump = true;
        _coyoteTimer = 0f;
    }

    public override void _Ready()
    {
        _airJumpsRemaining = _airJumps;
        if (_player == null && _body is Player player)
        {
            _player = player;
        }
    }

    public override void Enter()
    {
        UpdateAnimation();

        if (Input.IsActionPressed(Controller.Down))
        {
            _dropGraceTimer = 0.15f;
        }
    }

    public override void Exit()
    {
        _dropGraceTimer = 0;
        _inputLockoutTimer = 0;
        _isAscendingFromJump = false;
        _body.SetCollisionMaskValue(2, true);
    }

    public override void UpdatePhysics(double delta)
    {
        if (_dropGraceTimer > 0)
        {
            _dropGraceTimer -= (float)delta;
        }

        if (_coyoteTimer > 0)
        {
            _coyoteTimer -= (float)delta;
        }

        if (_jumpBufferTimer > 0)
        {
            _jumpBufferTimer -= (float)delta;
        }

        if (_isAscendingFromJump && _body.Velocity.Y < 0)
        {
            if (Input.IsActionJustReleased(Controller.A) || !Input.IsActionPressed(Controller.A))
            {
                _body.Velocity = new Vector2(_body.Velocity.X, _body.Velocity.Y * _jumpCutMultiplier);
                _isAscendingFromJump = false;
            }
        }
        else if (_body.Velocity.Y >= 0)
        {
            _isAscendingFromJump = false;
        }

        bool ignorePlatforms = Input.IsActionPressed(Controller.Down) || _dropGraceTimer > 0;
        _body.SetCollisionMaskValue(2, !ignorePlatforms);

        switch (true)
        {
            case true when this.TryTriggerItemAction(_player, out State targetState):
                Transition(targetState);
                break;

            case true
                when _jumpingState != null
                    && _coyoteTimer > 0
                    && Input.IsActionJustPressed(Controller.A):
                _coyoteTimer = 0;
                Transition(_jumpingState);
                break;

            case true
                when Input.IsActionJustPressed(Controller.A)
                    && _airJumpsRemaining > 0:
                _airJumpsRemaining--;
                Transition(_airJumpingState);
                break;

            case true when Input.IsActionJustPressed(Controller.A):
                _jumpBufferTimer = _jumpBufferTime;
                break;

            case true when _body.IsOnFloor():
                _airJumpsRemaining = _airJumps;
                _coyoteTimer = 0;
                if (_jumpBufferTimer > 0 && _jumpingState != null)
                {
                    _jumpBufferTimer = 0;
                    Transition(_jumpingState);
                }
                else
                {
                    Transition(_landingState);
                }
                break;

            case true when _wallSlidingState != null
                && _body.IsOnWall()
                && _body.Velocity.Y >= 0
                && (_body.GetWallNormal().X * Controller.GetHorizontalDirection()) < 0:
                _airJumpsRemaining = _airJumps;
                _coyoteTimer = 0;
                Transition(_wallSlidingState);
                break;

            case true when _waterDetector != null && _waterDetector.IsOverlapping && _body.Velocity.Y >= 0:
                _coyoteTimer = 0;
                Transition(_swimmingState);
                break;

            case true
                when _ladderDetector.IsOverlapping
                    && (
                        Input.IsActionPressed(Controller.Up)
                        || Input.IsActionPressed(Controller.Down)
                    ):
                _coyoteTimer = 0;
                Transition(_climbingState);
                break;

            default:
                Fall(delta);
                Move(delta);
                UpdateAnimation();
                break;
        }
    }

    private void UpdateAnimation()
    {
        if (_body.Velocity.Y < 0)
            _sprite.Play("Jump");
        else
            _sprite.Play("Fall");
    }

    private void Fall(double delta)
    {
        if (_body.Velocity.Y < _terminalVelocity)
            _body.Velocity += new Vector2(0, _gravity * (float)delta);
    }

    private void Move(double delta)
    {
        float direction = Controller.GetHorizontalDirection();

        if (_inputLockoutTimer > 0)
        {
            _inputLockoutTimer -= (float)delta;
            if (Mathf.Sign(direction) == Mathf.Sign(_lockedDirection))
            {
                direction = 0;
            }
        }

        _body.MoveWithInertia(
            direction: direction,
            acceleration: _acceleration * (float)delta,
            deceleration: _deceleration * (float)delta,
            limit: _maximumVelocity
        );

        _sprite.SynchronizeAnimation(-direction);
    }

    private bool IsInWaterOrCurrent()
    {
        return (_waterDetector != null && _waterDetector.IsOverlapping)
            || (_waterfallDetector != null && _waterfallDetector.IsOverlapping);
    }
}
