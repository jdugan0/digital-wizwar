using Godot;

public partial class TileView : Sprite2D
{
    public int EntityId;

    public void Sync(TileState state)
    {
        GlobalPosition =
            new Vector2(state.GridPos.X, state.GridPos.Y) * WorldData.instance.gridScale
            - WorldData.instance.origin;
    }
}
