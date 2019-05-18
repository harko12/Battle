using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PrefabPath))]
public class PrefabPathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();
        if (GUILayout.Button("Update Path"))
        {
            var pPath = target as PrefabPath;
            pPath.UpdatePath();
            EditorUtility.SetDirty(target);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
