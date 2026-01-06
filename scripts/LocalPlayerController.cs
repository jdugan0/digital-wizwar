using Godot;

public partial class LocalPlayerController : Node
{
    [Export]
    public NetworkGame Game;

    public int SelectedTileEntityId;

    public override void _Process(double delta)
    {
        var localId = Multiplayer.GetUniqueId();
        if (!Game.State.Players.TryGetValue(localId, out var playerState))
            return;

        if (SelectedTileEntityId == 0)
            return;
        Vector2I dir = HandleMove();
        if (dir != Vector2I.Zero)
        {
            var action = new MoveAction(SelectedTileEntityId, dir);
            SendAction(action);
        }
        else if (Input.IsActionJustPressed("UNDO"))
        {
            var action = new UndoAction(SelectedTileEntityId);
            SendAction(action);
        }
    }

    private void SendAction(IAction action)
    {
        var env = ActionCodec.ToEnvelope(action);
        Game.RpcId(1, nameof(NetworkGame.RpcSubmitAction), env);
    }

    private Vector2I HandleMove()
    {
        var dir = Vector2I.Zero;

        if (Input.IsActionJustPressed("UP"))
            dir = Vector2I.Up;
        else if (Input.IsActionJustPressed("DOWN"))
            dir = Vector2I.Down;
        else if (Input.IsActionJustPressed("LEFT"))
            dir = Vector2I.Left;
        else if (Input.IsActionJustPressed("RIGHT"))
            dir = Vector2I.Right;
        return dir;
    }
}
