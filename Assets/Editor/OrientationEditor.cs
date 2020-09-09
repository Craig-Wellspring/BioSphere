using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Orientation))]
public class OrientationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Orientation orientation = (Orientation)target;

        if (GUILayout.Button("Snap Orient"))
            orientation.SnapOrient();

        EditorGUILayout.Space(10);
        DrawDefaultInspector();
    }
}
