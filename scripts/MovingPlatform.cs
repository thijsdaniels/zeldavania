using Godot;

public partial class MovingPlatform : Path2D
{
    [Export]
    private float _velocity = 16;

    private PathFollow2D _follower;
    private Line2D _line;
    private Sprite2D _cog;

    public override void _Ready()
    {
        _follower = GetNode<PathFollow2D>("PathFollow2D");
        _line = GetNode<Line2D>("Line2D");
        _cog = GetNodeOrNull<Sprite2D>("PathFollow2D/AnimatableBody2D/Cog");

        foreach (Vector2 point in Curve.GetBakedPoints())
        {
            _line.AddPoint(point);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        float movement = _velocity * (float)delta;
        _follower.Progress += movement;

        if (_cog != null)
        {
            _cog.Rotation += movement / 3f;
        }
    }
}
