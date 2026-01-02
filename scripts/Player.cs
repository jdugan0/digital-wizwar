using Godot;
using System;

public partial class Player : Node2D
{
    Tile wizardTile;
    Tile selected;
    public override void _Ready()
    {

    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustReleased("UP"))
        {
            GD.Print(wizardTile != null);
            ((Wizard)wizardTile.getTileData()).Move(Vector2I.Up, wizardTile);
        }
    }


    public void init(Tile wizardTile)
    {
        this.wizardTile = wizardTile;
        selected = wizardTile;
    }

}
