using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectSpawner), true), CanEditMultipleObjects]
public class ObjectSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ObjectSpawner objectSpawner = (ObjectSpawner)target;

        if (GUILayout.Button("Draw Debug"))
            objectSpawner.DrawDebug();

        EditorGUILayout.Space(10);
        DrawDefaultInspector();
    }
}
