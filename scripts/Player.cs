using System;
using Godot;

public partial class Player : Node2D
{
    Tile wizardTile;
    Tile selected;

    public void ModifyGame()
    {
        if (Input.IsActionJustPressed("ENDTURN"))
        {
            GameManager.instance.NextTurn();
        }
        if (selected.getTileData() is Moveable m)
        {
            if (Input.IsActionJustReleased("UP"))
            {
                m.Move(Vector2I.Up, selected, this);
            }
            if (Input.IsActionJustReleased("DOWN"))
            {
                m.Move(Vector2I.Down, selected, this);
            }
            if (Input.IsActionJustReleased("LEFT"))
            {
                m.Move(Vector2I.Left, selected, this);
            }
            if (Input.IsActionJustReleased("RIGHT"))
            {
                m.Move(Vector2I.Right, selected, this);
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
