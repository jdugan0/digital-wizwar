using System.Data;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class WallTile : TileData
{
    public override void Populate(TileState state)
    {
        state.Add(new Blocking { movementBlocking = true, losBlocking = true });
    }

    public override void Init(TileState state, Dictionary init)
    {
        state.Get<Blocking>().direction = (Vector2I)init["direction"];
    }
}
