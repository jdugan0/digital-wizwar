using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class Moveable : TileData
{
    public void Move(Vector2I direction, Tile instance, long sender)
    {
        if ((long)instance.properties["mover"] != sender)
        {
            return;
        }
        int movementRemaining = (int)instance.properties["movementRemaining"];
        if (movementRemaining <= 0)
        {
            return;
        }
        Godot.Collections.Array<Vector2I> x = instance
            .properties["history"]
            .As<Godot.Collections.Array<Vector2I>>();
        x.Add(instance.GetGridPosition());
        instance.SetGridPosition(instance.GetGridPosition() + direction);
        instance.properties["movementRemaining"] = movementRemaining - 1;
    }

    public void Undo(Tile instance)
    {
        Godot.Collections.Array<Vector2I> x = instance
            .properties["history"]
            .As<Godot.Collections.Array<Vector2I>>();
        if (x.Count <= 0)
        {
            return;
        }
        Vector2I last = x[x.Count - 1];
        x.RemoveAt(x.Count - 1);
        instance.SetGridPosition(last);

        int movementRemaining = (int)instance.properties["movementRemaining"];
        instance.properties["movementRemaining"] = movementRemaining + 1;
    }

    public void ClearHistory(Tile instance)
    {
        instance.properties["history"].As<Godot.Collections.Array<Vector2I>>().Clear();
    }

    public void SetMovementRemaining(Tile instance, int move)
    {
        instance.properties["movementRemaining"] = move;
    }

    public override void Init(Tile instance)
    {
        base.Init(instance);
        instance.properties.Add("movementRemaining", 20);
        instance.properties.Add("history", new Godot.Collections.Array<Vector2I>());
        instance.properties.Add("mover", 0L);
    }

    public static long GetMover(Tile t) => (long)t.properties["mover"];

    public static void SetMover(Tile t, long id) => t.properties["mover"] = id;
}
