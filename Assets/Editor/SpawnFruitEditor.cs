using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpawnFruit))]
public class SpawnFruitEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SpawnFruit spawnFruit = (SpawnFruit)target;

        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Trigger Spawn"))
            spawnFruit.TriggerSpawn();
        EditorGUILayout.Space(10);
        if (GUILayout.Button("Draw Debug"))
            spawnFruit.DrawDebug();
    }
}
