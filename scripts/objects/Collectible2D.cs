using Godot;

namespace Zeldavania.Objects;

[GlobalClass]
public abstract partial class Collectible2D : Area2D
{
    [Signal]
    public delegate void CollectedEventHandler(Node2D collector);

    [Export]
    public float LifetimeSeconds { get; set; } = 15.0f;

    [Export]
    public bool HasLifetime { get; set; } = false;

    [Export]
    public float BlinkThresholdSeconds { get; set; } = 4.0f;

    [Export]
    public AudioStream CollectSound { get; set; }

    [Export]
    public Vector2 InitialVelocity { get; set; } = new Vector2(0, -120);

    [Export]
    public float FallGravity { get; set; } = 350.0f;

    [Export(PropertyHint.Layers2DPhysics)]
    public uint GroundCollisionMask { get; set; } = 3; // Layer 1 (Solids) | Layer 2 (Ledges / One-Way)

    [Export]
    public float HoverOffset { get; set; } = 10.0f;

    [Export]
    public float BobAmplitude { get; set; } = 2.0f;

    [Export]
    public float BobSpeed { get; set; } = 3.5f;

    private Vector2 _velocity;
    private bool _isAirborne = true;
    private float _lifetimeTimer;
    private float _bobTimer;
    private float _restY;
    private bool _isCollected;

    private CanvasItem _visual;
    private AudioStreamPlayer2D _audioPlayer;

    public override void _Ready()
    {
        _velocity = InitialVelocity;
        _restY = Position.Y;
        _visual = (CanvasItem)GetNodeOrNull<Sprite2D>("Sprite2D")
               ?? (CanvasItem)GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
        _audioPlayer = GetNodeOrNull<AudioStreamPlayer2D>("AudioStreamPlayer2D");

        BodyEntered += HandleBodyEntered;
        AreaEntered += HandleAreaEntered;

        // Check if already overlapping on spawn
        foreach (Node2D body in GetOverlappingBodies())
        {
            HandleBodyEntered(body);
            if (_isCollected)
            {
                break;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isCollected)
        {
            return;
        }

        float dt = (float)delta;

        if (_isAirborne)
        {
            _velocity.Y += FallGravity * dt;
            Vector2 motion = _velocity * dt;

            // Check ground collision with terrain (Solids + One-way Ledges)
            var spaceState = GetWorld2D()?.DirectSpaceState;
            if (spaceState != null && motion.Y > 0)
            {
                var query = PhysicsRayQueryParameters2D.Create(
                    GlobalPosition,
                    GlobalPosition + motion + new Vector2(0, HoverOffset),
                    GroundCollisionMask
                );
                query.CollideWithAreas = false;
                query.CollideWithBodies = true;

                var result = spaceState.IntersectRay(query);
                if (result.Count > 0)
                {
                    Vector2 hitPosition = (Vector2)result["position"];
                    GlobalPosition = new Vector2(GlobalPosition.X + motion.X, hitPosition.Y - HoverOffset);
                    _isAirborne = false;
                    _restY = Position.Y;
                    _velocity = Vector2.Zero;
                }
                else
                {
                    Position += motion;
                }
            }
            else
            {
                Position += motion;
            }
        }
        else
        {
            // Gentle hovering bobbing
            _bobTimer += dt * BobSpeed;
            Position = new Vector2(Position.X, _restY + Mathf.Sin(_bobTimer) * BobAmplitude);
        }

        // Lifetime expiration handling
        if (HasLifetime)
        {
            _lifetimeTimer += dt;
            float remaining = LifetimeSeconds - _lifetimeTimer;

            if (remaining <= BlinkThresholdSeconds)
            {
                // Rapid blinking
                float blinkRate = remaining <= 1.5f ? 20.0f : 10.0f;
                bool isVisible = Mathf.Sin(_lifetimeTimer * blinkRate) > 0;
                if (_visual != null)
                {
                    _visual.Visible = isVisible;
                }
            }

            if (_lifetimeTimer >= LifetimeSeconds)
            {
                QueueFree();
            }
        }
    }

    private void HandleBodyEntered(Node2D body)
    {
        Collect(body);
    }

    private void HandleAreaEntered(Area2D area)
    {
        Node parent = area.GetParent();
        if (parent is Node2D parentNode)
        {
            Collect(parentNode);
        }
        else
        {
            Collect(area);
        }
    }

    public bool Collect(Node2D collector)
    {
        if (_isCollected)
        {
            return false;
        }

        bool success = OnCollect(collector);
        if (success)
        {
            _isCollected = true;
            EmitSignal(SignalName.Collected, collector);
            SetDeferred(Area2D.PropertyName.Monitoring, false);
            SetDeferred(Area2D.PropertyName.Monitorable, false);

            if (_visual != null)
            {
                _visual.Visible = false;
            }

            PlayCollectSoundAndFree();
        }

        return success;
    }

    private void PlayCollectSoundAndFree()
    {
        AudioStream stream = CollectSound;
        if (stream == null && _audioPlayer?.Stream != null)
        {
            stream = _audioPlayer.Stream;
        }

        if (stream != null)
        {
            var tempAudio = new AudioStreamPlayer2D();
            tempAudio.Stream = stream;
            tempAudio.GlobalPosition = GlobalPosition;
            tempAudio.Autoplay = true;
            tempAudio.Finished += () => tempAudio.QueueFree();
            GetParent()?.AddChild(tempAudio);
        }

        QueueFree();
    }

    protected abstract bool OnCollect(Node2D collector);
}
