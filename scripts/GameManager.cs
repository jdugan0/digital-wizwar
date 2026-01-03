using System;
using System.Collections.Generic;
using Godot;

public partial class GameManager : Node2D
{
    [Export]
    public Vector2 origin;

    [Export]
    public float gridScale;
    public int playerCount = 4;

    [Export]
    public PackedScene player;
    private int turnCount = 0;
    public static GameManager instance;
    private List<Player> players = new List<Player>();

    public override void _Ready()
    {
        instance = this;
        for (int i = 0; i < playerCount; i++)
        {
            // init board and wizards. first board initalized is origin.
            Player p = player.Instantiate<Player>();
            Tile w = TileInitializer.CreateTile(
                this,
                new Vector2I((int)Mathf.Sin(i * Mathf.Pi / 2), (int)Mathf.Cos(i * Mathf.Pi / 2)),
                TileInitializer.instance.wizardData
            );
            p.init(w);
            ((Moveable)w.getTileData()).SetMover(p, w);
            players.Add(p);
            AddChild(p);
        }
    }

    public override void _Process(double delta)
    {
        players[turnCount % players.Count].ModifyGame();
    }

    public bool CheckTurn(Player p)
    {
        int i = players.IndexOf(p);
        return i == turnCount % players.Count;
    }

    public void NextTurn()
    {
        turnCount++;
        GD.Print("tuncount: " + turnCount);
        foreach (Tile t in TileInitializer.tiles)
        {
            if (t.getTileData() is Moveable m)
            {
                m.ClearHistory(t);
                GD.Print("remaining: " + t.properties["movementRemaining"]);
            }
        }
    }
}
