using Godot;

public static class SpawnSystem
{
    public static ApplyResult TrySpawn(
        GameState gs,
        int entityId,
        string defId,
        Vector2I pos,
        long ownerId
    )
    {
        if (gs.Entities.ContainsKey(entityId))
            return ApplyResult.Fail("EntityId already exists");

        var def = TileDatabase.instance.Get(defId);
        if (def == null)
            return ApplyResult.Fail("Unknown DefId");

        var ts = new TileState
        {
            EntityId = entityId,
            DefId = defId,
            GridPos = pos,
        };

        def.Populate(ts);

        var owner = ts.Get<HasOwner>();
        if (owner != null)
            owner.OwnerId = ownerId;

        gs.Entities[entityId] = ts;
        gs.AddToCell(pos, entityId);
        gs.EnsureNextIdAtLeast(entityId + 1);

        return ApplyResult.Success();
    }

    public static ApplyResult TryDespawn(GameState gs, int entityId)
    {
        if (!gs.Entities.TryGetValue(entityId, out var ts))
            return ApplyResult.Fail("Unknown entity");

        gs.RemoveFromCell(ts.GridPos, entityId);
        gs.Entities.Remove(entityId);

        return ApplyResult.Success();
    }
}
