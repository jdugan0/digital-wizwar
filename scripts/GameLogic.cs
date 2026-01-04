public static class GameLogic
{
    public static ApplyResult Apply(GameState gs, IAction action, long senderId)
    {
        return action switch
        {
            SpawnAction a => SpawnSystem.TrySpawn(gs, a.EntityId, a.DefId, a.Pos, a.OwnerId),
            MoveAction a => MovementSystem.TryMove(gs, a.EntityId, a.Dir, senderId),
            DespawnAction a => SpawnSystem.TryDespawn(gs, a.EntityId),
            UndoAction a => MovementSystem.TryUndo(gs, a.EntityId, senderId),
            _ => ApplyResult.Fail("Unknown action"),
        };
    }
}
