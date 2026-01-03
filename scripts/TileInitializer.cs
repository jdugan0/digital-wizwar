using System;
using System.Collections.Generic;
using Godot;

public partial class TileInitializer : Node2D
{
    [Export]
    public PackedScene tile;

    [Export]
    public Wizard wizardData;
    public static TileInitializer instance;
    public static List<Tile> tiles = new List<Tile>();
    public override void _Ready()
    {
        instance = this;
    }

    public static Tile CreateTile(Node obj, Vector2I position, TileData data)
    {
        Tile n = instance.tile.Instantiate<Tile>();
        n.setPosition(position);
        n.setTileData(data);
        data.Init(n);
        obj.AddChild(n);
        tiles.Add(n);
        return n;
    }

    public static Tile CreateTile(Node obj, TileData data)
    {
        return CreateTile(obj, Vector2I.Zero, data);
    }
}
