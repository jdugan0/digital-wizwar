using Godot;

[GlobalClass]
public partial class TileData : Resource
{
    [Export]
    public string Id;

    [Export]
    public Texture2D Texture;

    public virtual void Populate(TileState state) { }
}
