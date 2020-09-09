using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GlobalLifeSource))]
public class GlobalLifeSourceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space(10);
        
        GlobalLifeSource lifeSource = (GlobalLifeSource)target;

        if (GUILayout.Button("Spawn Meteor"))
            lifeSource.SpawnMeteor();

    }
}
