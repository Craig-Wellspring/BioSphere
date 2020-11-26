using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Evolution))]
public class EvolutionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Evolution evolution = (Evolution)target;

        DrawDefaultInspector();
        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Draw Debug"))
            evolution.DrawDebug();
        if (GUILayout.Button("Plant Seed"))
            evolution.PlantSeedButton();

        EditorGUILayout.EndHorizontal();

    }
}
