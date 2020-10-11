using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CreatureStats)), CanEditMultipleObjects]
public class CreatureDataEditor : Editor
{
    public override void OnInspectorGUI()
    {       
        CreatureStats cStats = (CreatureStats)target;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Push Stats to Origin"))
            cStats.PushOrPullOriginStats(true);

        if (GUILayout.Button("Pull Stats from Origin"))
            cStats.PushOrPullOriginStats(false);
            
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(15);
        DrawDefaultInspector();

    }
}
