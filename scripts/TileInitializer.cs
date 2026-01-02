using Godot;
using System;

public partial class TileInitializer : Node
{
    [Export] public PackedScene tile;
    [Export] public Wizard wizardData;
    public static TileInitializer instance;
    public override void _Ready()
    {
        instance = this;
    }

    public static Tile CreateTile(Node obj, Vector2I position, TileData data)
    {
        Tile n = instance.tile.Instantiate<Tile>();
        obj.AddChild(n);
        return n;
    }
    public static Tile CreateTile(Node obj, TileData data)
    {
        return CreateTile(obj, Vector2I.Zero, data);
    }
}
