using Godot;

[GlobalClass]
public partial class WizardTile : TileData
{
    public override void Populate(TileState state)
    {
        state.Add(new Moveable { OwnerId = -1, MovementRemaining = 20 });
    }
}
