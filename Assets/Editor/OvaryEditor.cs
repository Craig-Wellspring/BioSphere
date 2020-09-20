using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Ovary))]
public class OvaryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Ovary ovary = (Ovary)target;

        if (GUILayout.Button("Lay Egg"))
            ovary.LayEggButton();
        if (GUILayout.Button("Plant Seed"))
            ovary.PlantSeedButton();
            
        EditorGUILayout.Space(10);
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Draw Debug"))
            ovary.DrawDebug();
    }
}
