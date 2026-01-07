using System;
using System.Collections.Generic;
using Godot;

public sealed class TileState
{
    public int EntityId;
    public string DefId;
    public Vector2I GridPos;

    public string AnimationName = "default";

    private readonly Dictionary<Type, IComponent> components = new();

    public void Add<T>(T component)
        where T : IComponent
    {
        components[typeof(T)] = component;
    }

    public T Get<T>()
        where T : class, IComponent
    {
        components.TryGetValue(typeof(T), out var c);
        return c as T;
    }

    public bool Has<T>()
        where T : IComponent
    {
        return components.ContainsKey(typeof(T));
    }
}
