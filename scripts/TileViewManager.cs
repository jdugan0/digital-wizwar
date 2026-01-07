using System.Collections.Generic;
using Godot;

public partial class TileViewManager : Node
{
    [Export]
    public PackedScene TileViewScene;

    private readonly Dictionary<int, TileView> views = new();

    public void OnSpawned(GameState gs, int entityId)
    {
        if (views.ContainsKey(entityId))
            return;

        if (!gs.Entities.TryGetValue(entityId, out var ts))
            return;

        var def = TileDatabase.instance.Get(ts.DefId);
        var view = TileViewScene.Instantiate<TileView>();
        view.EntityId = entityId;
        view.ApplyDefinition(def);
        AddChild(view);
        views[entityId] = view;
        view.Sync(ts);
    }

    public void ClearAll()
    {
        foreach (var v in views.Values)
            v.QueueFree();
        views.Clear();
    }

    public void OnDespawned(int entityId)
    {
        if (!views.TryGetValue(entityId, out var view))
            return;

        views.Remove(entityId);
        view.QueueFree();
    }

    public void OnMoved(GameState gs, int entityId)
    {
        if (!views.TryGetValue(entityId, out var view))
            return;

        if (!gs.Entities.TryGetValue(entityId, out var ts))
            return;

        view.Sync(ts);
    }
}
