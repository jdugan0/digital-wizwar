using System.ComponentModel;
using System.Data;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class WallTile : TileData
{
    [Export]
    public TileSet tileSet;

    public override void Populate(TileState state)
    {
        state.Add(new Blocking { movementBlocking = true, losBlocking = true });
    }

    public override void Init(TileState state, Dictionary init)
    {
        var b = state.Get<Blocking>();
        var dir = (int)init["direction"];
        b.direction = dir;
        GD.Print((float)init["rotation_radians"]);
        state.RotationRadians = (float)init["rotation_radians"];
    }
}
