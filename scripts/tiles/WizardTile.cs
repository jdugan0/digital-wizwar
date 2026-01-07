using Godot;
using Godot.Collections;

[GlobalClass]
public partial class WizardTile : TileData
{
    public override void Populate(TileState state)
    {
        state.Add(new Moveable { MovementRemaining = 20 });
    }

    public override void Init(TileState state, Dictionary init)
    {
        state.Get<Moveable>().OwnerId = (long)init["ownerId"];
    }
}
