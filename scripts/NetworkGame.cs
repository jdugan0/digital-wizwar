using System.Collections.Generic;
using System.Security.Cryptography;
using Godot;
using Godot.Collections;
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
    }

    private void ResetSession()
    {
        State.Clear();
        State.Players.Clear();
        ViewManager?.ClearAll();
    }

    public void StartGame()
    {
        if (!Multiplayer.IsServer() || State.started)
            return;

        var all = new List<long> { Multiplayer.GetUniqueId() };
        foreach (var id in Multiplayer.GetPeers())
            all.Add(id);
        int count = 0;
        foreach (var peerId in all)
        {
            var a = new SpawnAction(
                "WizardTile",
                State.NextEntityId,
                new Vector2I(count, 0),
                new Dictionary { ["ownerId"] = peerId }
            );
            Rpc(nameof(RpcSubmitAction), ActionCodec.ToEnvelope(a));
            State.turnOrder.Add(peerId);
            count++;
        }

        MakeBoard();
        State.started = true;
        Rpc(nameof(SetTurnOrder), State.turnOrder);
    }

    private void MakeBoard()
    {
        var a = new SpawnAction(
            "WallTile",
            State.NextEntityId,
            new Vector2I(1, 1),
            new Dictionary { ["direction"] = Vector2.Down }
        );

        Rpc(nameof(RpcSubmitAction), ActionCodec.ToEnvelope(a));
    }

    [Rpc(TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void SetTurnOrder(Array<long> turnOrder)
    {
        if (State.started || Multiplayer.IsServer())
        {
            return;
        }
        State.turnOrder = turnOrder;
        State.started = true;
    }

    private PlayerState EnsurePlayer(long peerId)
    {
        if (State.Players.TryGetValue(peerId, out var ps))
            return ps;

        ps = new PlayerState { PeerId = peerId };
        State.Players[peerId] = ps;
        return ps;
    }

    [Rpc(
        MultiplayerApi.RpcMode.AnyPeer,
        CallLocal = true,
        TransferMode = MultiplayerPeer.TransferModeEnum.Reliable
    )]
    public void RpcSubmitAction(GDD envelope)
    {
        if (!Multiplayer.IsServer())
            return;
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

    [Rpc(TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void RpcApplyAction(GDD envelope, int sender)
    {
        if (Multiplayer.IsServer())
            return;
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
