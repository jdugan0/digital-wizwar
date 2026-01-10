using Godot;

[GlobalClass]
public partial class NamedTileSet : Resource
{
    [Export]
    public string Type;

    [Export]
    public TileSet set;
}
