using System.Collections.Generic;
using Godot;

public sealed class GameState
{
    public int NextEntityId = 1;

    public readonly Dictionary<int, TileState> Entities = new();
    public readonly Dictionary<Vector2I, List<int>> At = new();

    public readonly Dictionary<long, PlayerState> Players = new();

    public IReadOnlyList<int> GetAt(Vector2I pos) =>
        At.TryGetValue(pos, out var list) ? list : System.Array.Empty<int>();

    public void AddToCell(Vector2I pos, int entityId)
    {
        if (!At.TryGetValue(pos, out var list))
        {
            list = new List<int>();
            At[pos] = list;
        }
        list.Add(entityId);
    }

    public void Clear()
    {
        NextEntityId = 1;
        Entities.Clear();
        At.Clear();
    }

    public void RemoveFromCell(Vector2I pos, int entityId)
    {
        if (!At.TryGetValue(pos, out var list))
            return;

        var idx = list.IndexOf(entityId);
        if (idx >= 0)
            list.RemoveAt(idx);

        if (list.Count == 0)
            At.Remove(pos);
    }
}
