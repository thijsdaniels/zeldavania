using Godot;

public partial class WaterfallDetector2D : TileDetector2D
{
    [ExportGroup("Waterfall Current Tuning")]
    [Export]
    private float _waterfallCurrentForce = 600f;

    [Export]
    private float _waterfallDisperseSpeed = 20f;

    [Export]
    private float _waterfallDisperseRadius = 48f;

    public float WaterfallCurrentForce => _waterfallCurrentForce;
    public float WaterfallDisperseSpeed => _waterfallDisperseSpeed;
    public float WaterfallDisperseRadius => _waterfallDisperseRadius;

    public bool TryGetWaterfallPlungeDispersal(Vector2 globalPos, out float horizontalVelocity, float maxDisperseSpeed = -1f)
    {
        horizontalVelocity = 0f;
        float baseSpeed = maxDisperseSpeed > 0f ? maxDisperseSpeed : _waterfallDisperseSpeed;

        var spaceState = GetWorld2D()?.DirectSpaceState;
        if (spaceState == null)
            return false;

        var query = new PhysicsShapeQueryParameters2D
        {
            CollisionMask = CollisionMask,
            CollideWithAreas = true,
            CollideWithBodies = true,
            Shape = new CircleShape2D { Radius = _waterfallDisperseRadius },
            Transform = new Transform2D(0, globalPos)
        };

        var results = spaceState.IntersectShape(query, 8);
        if (results == null || results.Count == 0)
            return false;

        foreach (var result in results)
        {
            if (result.TryGetValue("collider", out var colliderVar) && colliderVar.AsGodotObject() is TileMapLayer layer)
            {
                Vector2 localPos = layer.ToLocal(globalPos);
                Vector2I mapPos = layer.LocalToMap(localPos);

                float totalWaterfallX = 0f;
                int count = 0;

                int tileRadius = Mathf.CeilToInt(_waterfallDisperseRadius / 16f) + 1;
                for (int dx = -tileRadius; dx <= tileRadius; dx++)
                {
                    for (int dy = -4; dy <= 2; dy++)
                    {
                        Vector2I checkCell = new Vector2I(mapPos.X + dx, mapPos.Y + dy);
                        if (layer.GetCellSourceId(checkCell) == 12 || (layer.GetCellTileData(checkCell)?.GetCollisionPolygonsCount(4) ?? 0) > 0)
                        {
                            Vector2 cellCenterGlobal = layer.ToGlobal(layer.MapToLocal(checkCell));
                            totalWaterfallX += cellCenterGlobal.X;
                            count++;
                        }
                    }
                }

                if (count > 0)
                {
                    float avgWaterfallCenterX = totalWaterfallX / count;
                    float diffX = globalPos.X - avgWaterfallCenterX;
                    float outwardDir = (diffX >= 0) ? 1f : -1f;
                    float waterfallDist = Mathf.Abs(diffX);

                    float intensity = Mathf.Clamp(1.0f - (waterfallDist / _waterfallDisperseRadius), 0f, 1f);
                    if (intensity > 0f)
                    {
                        horizontalVelocity = outwardDir * baseSpeed * intensity;
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
