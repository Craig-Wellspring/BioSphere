using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CreatureData)), CanEditMultipleObjects]
public class CreatureDataEditor : Editor
{
    public override void OnInspectorGUI()
    {           
        DrawDefaultInspector();

        CreatureData cData = (CreatureData)target;

        EditorGUILayout.Space(15);
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Pull Stats from Origin"))
            cData.PullStatsFromOrigin();

        if (GUILayout.Button("Push Stats to Origin"))
            cData.PushStatsToOrigin();
            
        EditorGUILayout.EndHorizontal();

    }
}
