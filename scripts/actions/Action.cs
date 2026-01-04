using Godot;

public interface IAction { }

public sealed record SpawnAction(int EntityId, string DefId, Vector2I Pos, long OwnerId) : IAction;

public sealed record MoveAction(int EntityId, Vector2I Dir) : IAction;

public sealed record DespawnAction(int EntityId) : IAction;

public readonly record struct ApplyResult(bool Ok, string Error)
{
    public static ApplyResult Success() => new(true, "");

    public static ApplyResult Fail(string error) => new(false, error);
}
