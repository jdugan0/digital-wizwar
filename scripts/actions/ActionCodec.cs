using System;
using System.Collections.Generic;
using Godot;
using GD = Godot.Collections.Dictionary;

public static class ActionCodec
{
    private static readonly Dictionary<string, Func<GD, IAction>> decode = new();
    private static readonly Dictionary<Type, Func<IAction, GD>> encode = new();

    public static void Register<T>(string type, Func<GD, T> dec, Func<T, GD> enc)
        where T : IAction
    {
        decode[type] = d => dec(d);
        encode[typeof(T)] = a => enc((T)a);
    }

    public static GD ToEnvelope(IAction action)
    {
        var t = action.GetType();
        if (!encode.TryGetValue(t, out var enc))
            throw new InvalidOperationException($"No encoder for {t.Name}");

        return new GD { ["type"] = t.Name, ["payload"] = enc(action) };
    }

    public static IAction FromEnvelope(GD env)
    {
        var type = (string)env["type"];
        var payload = (GD)env["payload"];

        if (!decode.TryGetValue(type, out var dec))
            throw new InvalidOperationException($"No decoder for {type}");

        return dec(payload);
    }
}
