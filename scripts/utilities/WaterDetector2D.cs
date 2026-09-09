using Godot;

public partial class WaterDetector2D : TileDetector2D
{
    public bool TryGetTopSurfaceY(Vector2 globalPos, out float surfaceY, out bool isUnderCeiling)
    {
        surfaceY = globalPos.Y;
        isUnderCeiling = false;

        var bodies = GetOverlappingBodies();
        foreach (var body in bodies)
        {
            if (body is TileMapLayer layer)
            {
                Vector2 localPos = layer.ToLocal(globalPos);
                Vector2I mapPos = layer.LocalToMap(localPos);

                Vector2I checkPos = mapPos;
                if (layer.GetCellSourceId(checkPos) == -1)
                {
                    if (layer.GetCellSourceId(new Vector2I(checkPos.X, checkPos.Y + 1)) != -1)
                    {
                        checkPos.Y += 1;
                    }
                }

                if (layer.GetCellSourceId(checkPos) != -1)
                {
                    while (layer.GetCellSourceId(new Vector2I(checkPos.X, checkPos.Y - 1)) != -1)
                    {
                        checkPos.Y--;
                    }

                    Vector2 cellCenterLocal = layer.MapToLocal(checkPos);
                    Vector2 tileSize = layer.TileSet != null ? (Vector2)layer.TileSet.TileSize : new Vector2(16, 16);
                    Vector2 topEdgeLocal = new Vector2(cellCenterLocal.X, cellCenterLocal.Y - (tileSize.Y * 0.5f));
                    surfaceY = layer.ToGlobal(topEdgeLocal).Y;

                    Vector2I atlasCoords = layer.GetCellAtlasCoords(checkPos);
                    isUnderCeiling = (atlasCoords.Y != 0);
                    return true;
                }
            }
        }
        return false;
    }
}
