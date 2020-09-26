using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Ovary))]
public class OvaryEditor : Editor
{
    public override void OnInspectorGUI()
    {           
        DrawDefaultInspector();

        Ovary ovary = (Ovary)target;

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Draw Debug"))
            ovary.DrawDebug();
            
        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Lay Egg"))
            ovary.LayEggButton();
        if (GUILayout.Button("Plant Seed"))
            ovary.PlantSeedButton();
            
        EditorGUILayout.EndHorizontal();

    }
}
