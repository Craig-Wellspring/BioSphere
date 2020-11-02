using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Reproduction))]
public class ReproductionEditor : Editor
{
    public override void OnInspectorGUI()
    {           
        DrawDefaultInspector();

        Reproduction reproduction = (Reproduction)target;
            
        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Plant Seed"))
            reproduction.PlantSeedButton();
        if (GUILayout.Button("Lay Egg"))
            reproduction.LayEggButton();
            
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Draw Debug"))
            reproduction.DrawDebug();
    }
}
