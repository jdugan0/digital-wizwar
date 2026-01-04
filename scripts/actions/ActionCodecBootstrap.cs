using Godot;
using GD = Godot.Collections.Dictionary;

public static class ActionCodecBootstrap
{
    public static void RegisterAll()
    {
        ActionCodec.Register<SpawnAction>(
            nameof(SpawnAction),
            d => new SpawnAction(
                (int)d["entityId"],
                (string)d["defId"],
                (Vector2I)d["pos"],
                (long)d["ownerId"]
            ),
            a => new GD
            {
                ["entityId"] = a.EntityId,
                ["defId"] = a.DefId,
                ["pos"] = a.Pos,
                ["ownerId"] = a.OwnerId,
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
    }
}
