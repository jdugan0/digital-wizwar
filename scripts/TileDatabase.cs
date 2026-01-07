using Godot;
using Godot.Collections;

public partial class TileDatabase : Node
{
    public static TileDatabase instance;

    [Export]
    public Array<TileData> tileDataList = new Array<TileData>();

    private readonly Dictionary<string, TileData> byId = new();

    public override void _Ready()
    {
        instance = this;

        byId.Clear();
        foreach (var td in tileDataList)
        {
            if (td == null)
                continue;

            if (string.IsNullOrEmpty(td.Id))
                continue;

            byId[td.Id] = td;
        }
    }

    public TileData Get(string id)
    {
        if (id == null)
            return null;

        return byId.TryGetValue(id, out var td) ? td : null;
    }
}
