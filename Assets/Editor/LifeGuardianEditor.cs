using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LifeGuardian))]
public class LifeGuardianEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LifeGuardian lifeGuardian = (LifeGuardian)target;
        if (GUILayout.Button("Plant Seed"))
            lifeGuardian.PlantSeed();
            
        EditorGUILayout.Space(10);
        DrawDefaultInspector();
    }
}
