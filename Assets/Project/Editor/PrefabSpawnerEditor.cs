using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[InitializeOnLoad]
[CustomEditor(typeof(PrefabSpawner), true)]
public class PrefabSpawnerEditor : Editor
{
    private GameObject preview;
    private void Awake()
    {
        PreviewPrefab();
    }

    void PreviewPrefab()
    {
        var ppScript = target as PrefabSpawner;
        preview = ppScript.preview;
        if (preview == null)
        {
            preview = GameObject.Instantiate(ppScript.prefab.PrefabObject);
            var t = ppScript.transform;
            preview.hideFlags = HideFlags.HideAndDontSave;
            preview.transform.SetParent(t);
            preview.transform.position = t.position;
            preview.transform.rotation = t.rotation;
            preview.transform.localScale = t.localScale;
            ppScript.preview = preview;
        }
    }


}
