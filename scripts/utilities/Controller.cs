using Godot;

public static class Controller
{
    public static readonly string Up = "Up";
    public static readonly string Down = "Down";
    public static readonly string Left = "Left";
    public static readonly string Right = "Right";
    public static readonly string A = "A";
    public static readonly string B = "B";
    public static readonly string X = "X";
    public static readonly string Y = "Y";

    public static Vector2 GetDirection()
    {
        return Input.GetVector(Left, Right, Up, Down);
    }

    public static float GetHorizontalDirection()
    {
        return Input.GetAxis(Left, Right);
    }

    public static float GetVerticalDirection()
    {
        return Input.GetAxis(Up, Down);
    }
}
