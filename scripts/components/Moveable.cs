using System.Collections.Generic;
using Godot;

public sealed class Moveable : HasOwner
{
    public int MovementRemaining;
    public readonly List<Vector2I> History = new();
}
