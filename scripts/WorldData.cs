using Godot;

public partial class WorldData : Node
{
    [Export]
    public Vector2 origin;

    [Export]
    public float gridScale;

    public static WorldData instance;

    public override void _Ready()
    {
        instance = this;
    }
}
