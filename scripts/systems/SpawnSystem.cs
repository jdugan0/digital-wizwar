using Godot;
using Godot.Collections;

public static class SpawnSystem
{
    public static ApplyResult TrySpawn(
        GameState gs,
        int entityId,
        string defId,
        Vector2I pos,
        long senderId,
        Dictionary init
    )
    {
        if (senderId != 1)
            return ApplyResult.Fail("Not server.");
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

        def.Init(ts, init);

        gs.Entities[entityId] = ts;
        gs.AddToCell(pos, entityId);
        if (gs.NextEntityId < entityId + 1)
            gs.NextEntityId = entityId + 1;

        return ApplyResult.Success();
    }

    public static ApplyResult TryDespawn(GameState gs, int entityId, long senderId)
    {
        if (!gs.Entities.TryGetValue(entityId, out var ts))
            return ApplyResult.Fail("Unknown entity");
        if (senderId != 1)
            return ApplyResult.Fail("Not server.");
        gs.RemoveFromCell(ts.GridPos, entityId);
        gs.Entities.Remove(entityId);

        return ApplyResult.Success();
    }

    static (int rotationDeg, bool mirrored) GetCellRotation(TileMapLayer layer, Vector2I cell)
    {
        bool h = layer.IsCellFlippedH(cell);
        bool v = layer.IsCellFlippedV(cell);
        bool t = layer.IsCellTransposed(cell);
        if (!t)
        {
            if (!h && !v)
                return (0, false);
            if (h && v)
                return (180, false);
            return (0, true);
        }
        else
        {
            if (!h && v)
                return (90, false);
            if (h && !v)
                return (270, false);
            return (0, true);
        }
    }

    public static ApplyResult TrySpawnBoard(
        GameState gs,
        int initalEntityId,
        Vector2I Pos,
        long senderId,
        int count
    )
    {
        TileMapLayer board = WorldData.instance.Board.Instantiate<TileMapLayer>();
        int entityId = initalEntityId;
        foreach (Vector2I cell in board.GetUsedCells())
        {
            Godot.TileData tileData = board.GetCellTileData(cell);
            if (tileData == null)
                continue;
            string defId = tileData.GetCustomData("type").ToString();
            Dictionary init = new Dictionary { };
            if (defId == "WallTile")
            {
                var (deg, mirrored) = GetCellRotation(board, cell);
                int rot = Mathf.RoundToInt(deg / 90.0f) % 4;
                init.Add(
                    "direction",
                    Blocking.RotateDir((int)tileData.GetCustomData("direction"), rot)
                );
                init.Add("texture", "WALL_NORMAL");
            }
            var r = TrySpawn(gs, entityId, defId, cell + Pos, senderId, init);
            entityId = gs.NextEntityId;
            if (!r.Ok)
            {
                return r;
            }
        }
        return ApplyResult.Success();
    }
}
