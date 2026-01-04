using Godot;
using GD = Godot.Collections.Dictionary;

public partial class NetworkGame : Node
{
    [Export]
    public TileViewManager ViewManager;

    public readonly GameState State = new();
    public readonly System.Collections.Generic.Dictionary<long, PlayerState> Players = new();

    public override void _Ready()
    {
        ActionCodecBootstrap.RegisterAll();

        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;

        EnsurePlayer(Multiplayer.GetUniqueId());
    }

    private void OnPeerConnected(long id) => EnsurePlayer(id);

    private void OnPeerDisconnected(long id)
    {
        Players.Remove(id);
    }

    private PlayerState EnsurePlayer(long peerId)
    {
        if (Players.TryGetValue(peerId, out var ps))
            return ps;

        ps = new PlayerState { PeerId = peerId, SelectedTileEntityId = 0 };
        Players[peerId] = ps;
        return ps;
    }

    [Rpc(MultiplayerApi.RpcMode.Authority)]
    public void RpcSubmitAction(GD envelope)
    {
        var sender = Multiplayer.GetRemoteSenderId();
        var action = ActionCodec.FromEnvelope(envelope);

        var res = GameLogic.Apply(State, action, sender);
        if (!res.Ok)
            return;

        ApplyViewDelta(action);

        Rpc(nameof(RpcApplyAction), envelope);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void RpcApplyAction(GD envelope)
    {
        if (Multiplayer.IsServer())
            return;

        var action = ActionCodec.FromEnvelope(envelope);

        var res = GameLogic.Apply(State, action, 0);
        if (!res.Ok)
            return;

        ApplyViewDelta(action);
    }

    private void ApplyViewDelta(IAction action)
    {
        switch (action)
        {
            case SpawnAction a:
                ViewManager.OnSpawned(State, a.EntityId);
                break;
            case DespawnAction a:
                ViewManager.OnDespawned(a.EntityId);
                break;
            case MoveAction a:
                ViewManager.OnMoved(State, a.EntityId);
                break;
            case UndoAction a:
                ViewManager.OnMoved(State, a.EntityId);
                break;
        }
    }
}
