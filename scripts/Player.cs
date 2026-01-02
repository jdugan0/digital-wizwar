using Godot;
using System;

public partial class Player : Node
{
    Tile wizardTile;
    Tile selected;
    public override void _Ready()
    {

    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }


    public void init(Tile wizardTile)
    {
        this.wizardTile = wizardTile;
        selected = wizardTile;
    }

}
