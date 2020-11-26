using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Reproduction))]
public class ReproductionEditor : Editor
{
    public override void OnInspectorGUI()
    {           
        Reproduction reproduction = (Reproduction)target;
            
        DrawDefaultInspector();
        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Draw Debug"))
            reproduction.DrawDebug();
        if (GUILayout.Button("Lay Egg"))
            reproduction.LayEggButton();
            
        EditorGUILayout.EndHorizontal();
    }
}
