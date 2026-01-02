using Godot;
using System;
[GlobalClass]
public partial class Moveable : TileData
{
    public void Move(Vector2I direction, Tile instance)
    {
        int movementRemaining = (int)instance.properties["movementRemaining"];
        if (movementRemaining <= 0)
        {
            return;
        }

        instance.setPosition(instance.getPosition() + direction);
        instance.properties["movementRemaining"] = movementRemaining - 1;
    }

    public override void Init(Tile instance)
    {
        base.Init(instance);
        instance.properties.Add("movementRemaining", 3);
    }

}
