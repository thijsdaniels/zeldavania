using Godot;

public static class DropThrough
{
    public static bool IsOnOneWayPlatform(
        this CharacterBody2D body,
        int platformLayer = 2,
        float testDistance = 2f
    )
    {
        if (!body.IsOnFloor())
            return false;

        uint originalMask = body.CollisionMask;
        body.SetCollisionMaskValue(platformLayer, false);

        bool hasSolidGround = body.TestMove(
            body.GlobalTransform,
            new Vector2(0, testDistance)
        );

        body.CollisionMask = originalMask;

        return !hasSolidGround;
    }
}
