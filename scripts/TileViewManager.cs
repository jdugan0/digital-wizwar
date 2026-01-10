using System.Collections.Generic;
using Godot;
using GD = Godot.Collections.Dictionary<string, Godot.SpriteFrames>;

public partial class TileViewManager : Node
{
    [Export]
    public PackedScene TileViewScene;

    private readonly Dictionary<int, TileView> views = new();

    [Export]
    public GD spriteFrames = new();

    [Export]
    public NamedTileSet[] sprites;
    public static TileViewManager instance;

    public void AddAllTilesToSpriteFrames(
        TileSet tileSet,
        SpriteFrames spriteFrames,
        string animPrefix = "tile"
    )
    {
        if (tileSet == null || spriteFrames == null)
            return;
        for (int sourceId = 0; sourceId < tileSet.GetSourceCount(); sourceId++)
        {
            Godot.GD.Print("Source count: " + tileSet.GetSourceCount());
            var source = tileSet.GetSource(tileSet.GetSourceId(sourceId));

            if (source is TileSetAtlasSource atlasSource)
            {
                Texture2D atlasTexture = atlasSource.Texture;
                if (atlasTexture == null)
                    continue;

                for (int x = 0; x < atlasSource.GetTilesCount(); x++)
                {
                    Vector2I atlasCoords = atlasSource.GetTileId(x);
                    for (
                        int altId = 0;
                        altId < atlasSource.GetAlternativeTilesCount(atlasCoords);
                        altId++
                    )
                    {
                        Rect2I region = atlasSource.GetTileTextureRegion(atlasCoords, altId);

                        var tileTex = new AtlasTexture { Atlas = atlasTexture, Region = region };

                        string animName =
                            $"{animPrefix}_{tileSet.GetSourceId(sourceId)}_{atlasCoords.X}_{atlasCoords.Y}_{altId}";
                        Godot.GD.Print(animName);

                        if (!spriteFrames.HasAnimation(animName))
                            spriteFrames.AddAnimation(animName);
                        spriteFrames.AddFrame(animName, tileTex);
                    }
                }
            }
        }
    }

    public override void _Ready()
    {
        instance = this;
        foreach (NamedTileSet s in sprites)
        {
            if (spriteFrames.ContainsKey(s.Type))
            {
                AddAllTilesToSpriteFrames(s.set, spriteFrames[s.Type]);
            }
        }
    }

    public void OnSpawned(GameState gs, int entityId)
    {
        if (views.ContainsKey(entityId))
            return;

        if (!gs.Entities.TryGetValue(entityId, out var ts))
            return;

        var def = TileDatabase.instance.Get(ts.DefId);
        var view = TileViewScene.Instantiate<TileView>();
        view.EntityId = entityId;
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
