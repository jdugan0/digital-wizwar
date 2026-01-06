using Godot;

public static class MovementSystem
{
    public static ApplyResult TryMove(GameState gs, int entityId, Vector2I dir, long senderId)
    {
        if (!gs.Entities.TryGetValue(entityId, out var ts))
            return ApplyResult.Fail("Unknown entity");

        var move = ts.Get<Moveable>();
        if (move == null)
            return ApplyResult.Fail("Not moveable");

        if (move.OwnerId != senderId)
            return ApplyResult.Fail(
                "Not your piece, sender: " + senderId + " owner: " + move.OwnerId
            );

        if (move.MovementRemaining <= 0)
            return ApplyResult.Fail("No movement remaining");

        var from = ts.GridPos;
        var to = from + dir;

        move.History.Add(from);
        move.MovementRemaining--;

        gs.RemoveFromCell(from, entityId);
        ts.GridPos = to;
        gs.AddToCell(to, entityId);

        return ApplyResult.Success();
    }

    public static ApplyResult TryUndo(GameState gs, int entityId, long senderId)
    {
        if (!gs.Entities.TryGetValue(entityId, out var ts))
            return ApplyResult.Fail("Unknown entity");

        var move = ts.Get<Moveable>();
        if (move == null)
            return ApplyResult.Fail("Not moveable");

        if (move.OwnerId != senderId)
            return ApplyResult.Fail("Not your piece");

        if (move.History.Count == 0)
            return ApplyResult.Fail("No history");

        var from = ts.GridPos;
        var to = move.History[^1];

        move.History.RemoveAt(move.History.Count - 1);
        move.MovementRemaining++;

        gs.RemoveFromCell(from, entityId);
        ts.GridPos = to;
        gs.AddToCell(to, entityId);

        return ApplyResult.Success();
    }
}
