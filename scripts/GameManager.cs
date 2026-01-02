using Godot;
using System;

public partial class GameManager : Node
{
    public static Vector2 origin;
    public static Vector2 scale;
    public static int playerCount = 1;
    [Export] public PackedScene player;
    private int turnCount = 0;
    public override void _Ready()
    {
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
