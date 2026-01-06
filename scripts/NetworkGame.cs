using System.Collections.Generic;
using System.Security.Cryptography;
using Godot;
using GDD = Godot.Collections.Dictionary;

public partial class NetworkGame : Node
{
    [Export]
    public TileViewManager ViewManager;

    public readonly GameState State = new();
    private ENetMultiplayerPeer peer;

    public override void _Ready()
    {
        ActionCodecBootstrap.RegisterAll();

        Multiplayer.PeerConnected += OnPeerConnected;
        Multiplayer.PeerDisconnected += OnPeerDisconnected;

        EnsurePlayer(Multiplayer.GetUniqueId());
    }

    private void OnPeerConnected(long id)
    {
        EnsurePlayer(id);
        GD.Print("Connected: " + id);
    }

    private void OnPeerDisconnected(long id)
    {
        State.Players.Remove(id);
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
        State.Players.Clear();
        ViewManager?.ClearAll();
    }

    public void StartGame()
    {
        if (!Multiplayer.IsServer())
            return;

        var all = new List<long> { Multiplayer.GetUniqueId() };
        foreach (var id in Multiplayer.GetPeers())
            all.Add(id);
        GD.Print("ATTEMPT" + all.Count);
        int count = 0;
        foreach (var peerId in all)
        {
            GD.Print(count);
            var a = new SpawnAction(
                "WizardTile",
                State.NextEntityId,
                new Vector2I(count, 0),
                peerId
            );

            var res = GameLogic.Apply(State, a, 1);
            if (!res.Ok)
            {
                GD.Print(res.Error);
                continue;
            }

            ViewManager.OnSpawned(State, a.EntityId);
            Rpc(nameof(RpcApplyAction), ActionCodec.ToEnvelope(a), 1);
            count++;
        }
    }

    private PlayerState EnsurePlayer(long peerId)
    {
        if (State.Players.TryGetValue(peerId, out var ps))
            return ps;

        ps = new PlayerState { PeerId = peerId };
        State.Players[peerId] = ps;
        return ps;
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    public void RpcSubmitAction(GDD envelope)
    {
        if (!Multiplayer.IsServer())
            return;
        GD.Print("Called Server");
        var sender = Multiplayer.GetRemoteSenderId();
        var action = ActionCodec.FromEnvelope(envelope);

        var res = GameLogic.Apply(State, action, sender);
        if (!res.Ok)
        {
            GD.Print(res.Error);
            return;
        }

        ApplyViewDelta(action);

        Rpc(nameof(RpcApplyAction), envelope, sender);
    }

    [Rpc]
    public void RpcApplyAction(GDD envelope, int sender)
    {
        if (Multiplayer.IsServer())
            return;
        GD.Print("Called Local");
        var action = ActionCodec.FromEnvelope(envelope);

        var res = GameLogic.Apply(State, action, sender);
        if (!res.Ok)
        {
            GD.Print(res.Error);
            return;
        }

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
