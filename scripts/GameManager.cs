using Godot;
using System;

public partial class GameManager : Node2D
{
    [Export] public Vector2 origin;
    [Export] public float gridScale;
    public int playerCount = 1;
    [Export] public PackedScene player;
    private int turnCount = 0;
    public static GameManager instance;
    public override void _Ready()
    {
        instance = this;
        for (int i = 0; i < playerCount; i++)
        {
            // init board and wizards. first board initalized is origin.
            Player p = player.Instantiate<Player>();
            Tile w = TileInitializer.CreateTile(this, TileInitializer.instance.wizardData);
            p.init(w);
            AddChild(p);
        }
    }

}
