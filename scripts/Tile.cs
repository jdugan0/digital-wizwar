using Godot;
using Godot.Collections;
using System;

public partial class Tile : Sprite2D
{
    private Vector2I gridPosition;
    [Export] private TileData data;
    public Dictionary<string, Variant> properties = new Dictionary<string, Variant>();
    public void setPosition(Vector2I newPos)
    {
        gridPosition = newPos;
        GlobalPosition = new Vector2(gridPosition.X, gridPosition.Y) * GameManager.instance.gridScale - GameManager.instance.origin;
    }

    public Vector2I getPosition()
    {
        return gridPosition;
    }

    public void setTileData(TileData data)
    {
        this.data = data;
    }

    public TileData getTileData()
    {
        return data;
    }
}
