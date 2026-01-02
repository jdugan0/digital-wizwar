using Godot;
using System;
[GlobalClass]
public partial class TileData : Resource
{
    [Export] public string Id;
    [Export] public Texture2D texture;

    public virtual void OnEnter(Tile tile) { }
    public virtual void OnExit(Tile tile) { }

    public virtual void Init(Tile instance)
    {
        instance.Texture = texture;
    }
}
