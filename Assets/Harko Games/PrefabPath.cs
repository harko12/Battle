using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabPath", menuName = "Prefab with Path")]
public class PrefabPath : ScriptableObject
{
    public GameObject PrefabObject;
    public string ResourcesPath = "Assets/Project/Resources/";
    [Header("Calculated paths")]
    public string AssetPath;
    public string PathInResources;

    public void UpdatePath()
    {
#if UNITY_EDITOR
        var parent = PrefabObject.transform.root.gameObject;
        AssetPath = AssetDatabase.GetAssetPath(parent);
        PathInResources = AssetPath.Replace(ResourcesPath, "").Replace(".prefab", "");
#endif
    }
}
