using System.Collections.Generic;
using Godot;
using GD = Godot.Collections.Dictionary;

public partial class NetworkGame : Node
{
    [Export]
    public TileViewManager ViewManager;

    public readonly GameState State = new();
    public readonly System.Collections.Generic.Dictionary<long, PlayerState> Players = new();
    private ENetMultiplayerPeer peer;

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

    public void Host(int port, int maxClients)
    {
        ResetSession();

        peer = new ENetMultiplayerPeer();
        var err = peer.CreateServer(port, maxClients);
        if (err != Error.Ok)
            return;

        Multiplayer.MultiplayerPeer = peer;
        EnsurePlayer(Multiplayer.GetUniqueId());
    }

    public void Join(string address, int port)
    {
        ResetSession();

        peer = new ENetMultiplayerPeer();
        var err = peer.CreateClient(address, port);
        if (err != Error.Ok)
            return;

        Multiplayer.MultiplayerPeer = peer;
        EnsurePlayer(Multiplayer.GetUniqueId());
    }

    public void Leave()
    {
        ResetSession();
        Multiplayer.MultiplayerPeer = null;
        peer = null;
        EnsurePlayer(Multiplayer.GetUniqueId());
    }

    private void ResetSession()
    {
        State.Clear();
        Players.Clear();
        ViewManager?.ClearAll();
    }

    public void StartGame()
    {
        if (!Multiplayer.IsServer())
            return;

        var all = new List<long> { Multiplayer.GetUniqueId() };
        foreach (var id in Multiplayer.GetPeers())
            all.Add(id);

        foreach (var peerId in all)
        {
            int entityId = State.NextEntityId++;
            var a = new SpawnAction(entityId, "Wizard", new Vector2I(0, 0), peerId);

            var res = GameLogic.Apply(State, a, 0);
            if (!res.Ok)
                continue;

            ViewManager.OnSpawned(State, a.EntityId);
            Rpc(nameof(RpcApplyAction), ActionCodec.ToEnvelope(a));
        }
    }

    private PlayerState EnsurePlayer(long peerId)
    {
        if (Players.TryGetValue(peerId, out var ps))
            return ps;

        ps = new PlayerState { PeerId = peerId };
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
