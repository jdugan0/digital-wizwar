using System;
using Godot;
using Godot.Collections;

public partial class Tile : Sprite2D
{
    private Vector2I gridPosition;

    [Export]
    private TileData data;
    public Dictionary<string, Variant> properties = new Dictionary<string, Variant>();

    public void Init(Vector2I gridPos, TileData tileData)
    {
        data = tileData;
        data.Init(this);
        SetGridPosition(gridPos);
    }

    public void SetGridPosition(Vector2I newPos)
    {
        gridPosition = newPos;
        GlobalPosition =
            new Vector2(gridPosition.X, gridPosition.Y) * WorldData.instance.gridScale
            - WorldData.instance.origin;
    }

    public Vector2I GetGridPosition()
    {
        return gridPosition;
    }

    public void SetTileData(TileData data)
    {
        this.data = data;
    }

    public TileData GetTileData()
    {
        return data;
    }
}
