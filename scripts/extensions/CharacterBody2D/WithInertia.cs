using Godot;

public static class WithInertia
{
    public static void MoveWithInertia(
        this CharacterBody2D body,
        Vector2 direction,
        Vector2 acceleration,
        Vector2 deceleration,
        Vector2 limit
    )
    {
        if (direction.Length() > 0)
            Accelerate(body, direction, acceleration, limit);
        else
            Decelerate(body, deceleration);
    }

    public static void MoveWithInertia(
        this CharacterBody2D body,
        Vector2 direction,
        float acceleration,
        float deceleration,
        float limit
    )
    {
        MoveWithInertia(
            body: body,
            direction: direction,
            acceleration: new Vector2(acceleration, acceleration),
            deceleration: new Vector2(deceleration, deceleration),
            limit: new Vector2(limit, limit)
        );
    }

    public static void MoveWithInertia(
        this CharacterBody2D body,
        float direction,
        float acceleration,
        float deceleration,
        float limit
    )
    {
        MoveWithInertia(
            body: body,
            direction: new Vector2(direction, 0),
            acceleration: new Vector2(acceleration, 0),
            deceleration: new Vector2(deceleration, 0),
            limit: new Vector2(limit, 0)
        );
    }

    public static void Accelerate(
        this CharacterBody2D body,
        Vector2 direction,
        Vector2 acceleration,
        Vector2 limit
    )
    {
        Vector2 normalizedDirection = direction.Normalized();

        body.Velocity = new Vector2(
            Mathf.MoveToward(
                body.Velocity.X,
                limit.X * normalizedDirection.X,
                acceleration.X
            ),
            Mathf.MoveToward(
                body.Velocity.Y,
                limit.Y * normalizedDirection.Y,
                acceleration.Y
            )
        );
    }

    public static void Accelerate(
        this CharacterBody2D body,
        Vector2 direction,
        float acceleration,
        float limit
    )
    {
        Accelerate(
            body: body,
            direction: direction,
            acceleration: new Vector2(acceleration, acceleration),
            limit: new Vector2(limit, limit)
        );
    }

    public static void Accelerate(
        this CharacterBody2D body,
        float direction,
        float acceleration,
        float limit
    )
    {
        Accelerate(
            body: body,
            direction: new Vector2(direction, 0),
            acceleration: new Vector2(acceleration, 0),
            limit: new Vector2(limit, 0)
        );
    }

    public static void Decelerate(
        this CharacterBody2D body,
        Vector2 deceleration
    )
    {
        body.Velocity = new Vector2(
            Mathf.MoveToward(body.Velocity.X, 0, deceleration.X),
            Mathf.MoveToward(body.Velocity.Y, 0, deceleration.Y)
        );
    }

    public static void Decelerate(this CharacterBody2D body, float deceleration)
    {
        Decelerate(
            body: body,
            deceleration: new Vector2(deceleration, deceleration)
        );
    }
}
