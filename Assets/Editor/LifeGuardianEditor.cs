using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LifeGuardian))]
public class LifeGuardianEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LifeGuardian lifeGuardian = (LifeGuardian)target;

        if (GUILayout.Button("Plant Seed"))
            lifeGuardian.PlantSeedFromSource();
            
        EditorGUILayout.Space(10);
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Draw Debug"))
            lifeGuardian.DrawDebug();
    }
}
