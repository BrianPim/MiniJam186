using CamLib.Editor;
using UnityEngine;

public class AssetWindowImplementation : CentralizedAssetWindowImplementation
{
    public override string[] SceneFolders => new[] { "Assets/_Game/Scenes" };

    public override IEditorPrefInstance[] Prefs => base.Prefs;

    public override string[] AssetDisplayPaths => new []
    {
        "Assets/_Game/Prefabs/Player.prefab",
    };
}
