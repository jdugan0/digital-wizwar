public static class GameLogic
{
    public static ApplyResult Apply(GameState gs, IAction action, long senderId)
    {
        return action switch
        {
            SpawnAction a => SpawnSystem.TrySpawn(gs, a.EntityId, a.DefId, a.Pos, senderId, a.Init),
            MoveAction a => MovementSystem.TryMove(gs, a.EntityId, a.Dir, senderId),
            DespawnAction a => SpawnSystem.TryDespawn(gs, a.EntityId, senderId),
            UndoAction a => MovementSystem.TryUndo(gs, a.EntityId, senderId),
            NextTurnAction a => TurnSystem.TryNextTurn(gs, senderId),
            SpawnBoardAction a => SpawnSystem.TrySpawnBoard(gs, a.initalEntityId, a.Pos, senderId),
            _ => ApplyResult.Fail("Unknown action"),
        };
    }
}
