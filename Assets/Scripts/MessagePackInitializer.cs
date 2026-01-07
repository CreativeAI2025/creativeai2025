using MessagePack;
using MessagePack.Resolvers;
using MessagePack.Unity;
using UnityEngine;

public static class MessagePackInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        var resolver = CompositeResolver.Create(
            GeneratedResolver.Instance, // 修理したファイル内のクラス
            UnityResolver.Instance,    // Vector2Int などの Unity 型解決に必須
            StandardResolver.Instance
        );

        var options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
        MessagePackSerializer.DefaultOptions = options;

        Debug.Log("MessagePack Initialized successfully!");
    }
}