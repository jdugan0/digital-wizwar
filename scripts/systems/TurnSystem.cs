using Godot;

public static class TurnSystem
{
    public static ApplyResult TryNextTurn(GameState gs, long senderId)
    {
        if (gs.turnOrder[gs.turnCount % gs.turnOrder.Count] != senderId)
            return ApplyResult.Fail("Not your turn.");
        gs.turnCount++;
        return ApplyResult.Success();
    }
}
