using Godot;

[GlobalClass]
public partial class WizardTile : TileData
{
    public override void Populate(TileState state)
    {
        state.Add(new Moveable { MovementRemaining = 20 });
    }
}
