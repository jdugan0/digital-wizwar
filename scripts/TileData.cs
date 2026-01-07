using Godot;
using Godot.Collections;

[GlobalClass]
public partial class TileData : Resource
{
    [Export]
    public string Id;

    [Export]
    public SpriteFrames SpriteFrames;

    public virtual void Populate(TileState state) { }

    public virtual void Init(TileState state, Dictionary init) { }
}
