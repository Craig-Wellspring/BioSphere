using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CloudSpawner))]
public class CloudSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CloudSpawner cloudSpawner = (CloudSpawner)target;
        if (GUILayout.Button("Spawn Cloud"))
            cloudSpawner.SpawnCloud(true);
            
        EditorGUILayout.Space(10);
        DrawDefaultInspector();
    }
}
