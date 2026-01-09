using Godot;

public partial class TileView : Area2D
{
    public int EntityId;

    [Export]
    AnimatedSprite2D sprite;

    bool mouse;

    public override void _Process(double delta)
    {
        if (mouse && Input.IsActionJustPressed("CLICK"))
        {
            OnClick();
        }
    }

    public void ApplyDefinition(TileData def)
    {
        sprite.SpriteFrames = def.SpriteFrames;
    }

    public void Sync(TileState state)
    {
        GlobalPosition =
            new Vector2(state.GridPos.X, state.GridPos.Y) * WorldData.instance.gridScale
            - WorldData.instance.origin;
        sprite.Animation = state.AnimationName;
        sprite.Rotation = state.RotationRadians;
    }

    public void OnClick()
    {
        LocalPlayerController p =
            GetTree().GetFirstNodeInGroup("PlayerController") as LocalPlayerController;
        p.SelectedTileEntityId = EntityId;
    }

    public void MouseEnter()
    {
        mouse = true;
    }

    public void MouseExit()
    {
        mouse = false;
    }
}
