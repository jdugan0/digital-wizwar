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
        if (selected.getTileData() is Moveable m)
        {
            if (Input.IsActionJustReleased("UP"))
            {
                m.Move(Vector2I.Up, selected);
            }
            if (Input.IsActionJustReleased("DOWN"))
            {
                m.Move(Vector2I.Down, selected);
            }
            if (Input.IsActionJustReleased("LEFT"))
            {
                m.Move(Vector2I.Left, selected);
            }
            if (Input.IsActionJustReleased("RIGHT"))
            {
                m.Move(Vector2I.Right, selected);
            }
            if (Input.IsActionJustPressed("UNDO"))
            {
                m.Undo(selected);
            }
        }
    }


    public void init(Tile wizardTile)
    {
        this.wizardTile = wizardTile;
        selected = wizardTile;
    }

}
