using Godot;
using GD = Godot.Collections.Dictionary;

public static class ActionCodecBootstrap
{
    public static void RegisterAll()
    {
        ActionCodec.Register<SpawnAction>(
            nameof(SpawnAction),
            d => new SpawnAction(
                (string)d["defId"],
                (int)d["entityId"],
                (Vector2I)d["pos"],
                (GD)d["init"]
            ),
            a => new GD
            {
                ["entityId"] = a.EntityId,
                ["defId"] = a.DefId,
                ["pos"] = a.Pos,
                ["init"] = a.Init,
            }
        );

        ActionCodec.Register<MoveAction>(
            nameof(MoveAction),
            d => new MoveAction((int)d["entityId"], (Vector2I)d["dir"]),
            a => new GD { ["entityId"] = a.EntityId, ["dir"] = a.Dir }
        );

        ActionCodec.Register<UndoAction>(
            nameof(UndoAction),
            d => new UndoAction((int)d["entityId"]),
            a => new GD { ["entityId"] = a.EntityId }
        );

        ActionCodec.Register<DespawnAction>(
            nameof(DespawnAction),
            d => new DespawnAction((int)d["entityId"]),
            a => new GD { ["entityId"] = a.EntityId }
        );

        ActionCodec.Register<NextTurnAction>(
            nameof(NextTurnAction),
            d => new NextTurnAction(),
            a => new GD { }
        );
    }
}
