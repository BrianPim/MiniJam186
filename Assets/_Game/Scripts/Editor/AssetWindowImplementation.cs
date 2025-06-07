using CamLib.Editor;
using UnityEngine;

public class AssetWindowImplementation : CentralizedAssetWindowImplementation
{
    public override string[] SceneFolders => new[] { "Assets/_Game/Scenes" };

    public override IEditorPrefInstance[] Prefs => base.Prefs;

    public override string[] AssetDisplayPaths => new []
    {
        "Assets/_Game/Prefabs/Player.prefab",
        "Assets/_Game/Prefabs/Enemies/Enemy_Brain.prefab",
        "Assets/_Game/Prefabs/Enemies/Enemy_Hand.prefab",
        "Assets/_Game/Prefabs/Enemies/Enemy_Jetpack.prefab",
        "Assets/_Game/Prefabs/Enemies/Enemy_NukeHead.prefab",
        "Assets/_Game/Prefabs/Enemies/Enemy_Surfer.prefab",
        "Assets/_Game/Prefabs/Projectiles/LaserProjectile.prefab",
    };
}
