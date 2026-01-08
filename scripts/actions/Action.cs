using Godot;
using Godot.Collections;

public interface IAction { }

public sealed record SpawnAction(string DefId, int EntityId, Vector2I Pos, Dictionary Init)
    : IAction;

public sealed record SpawnBoardAction(int initalEntityId, Vector2I Pos, int count) : IAction;

public sealed record MoveAction(int EntityId, Vector2I Dir) : IAction;

public sealed record UndoAction(int EntityId) : IAction;

public sealed record NextTurnAction() : IAction;

public sealed record DespawnAction(int EntityId) : IAction;

public readonly record struct ApplyResult(bool Ok, string Error)
{
    public static ApplyResult Success() => new(true, "");

    public static ApplyResult Fail(string error) => new(false, error);
}
