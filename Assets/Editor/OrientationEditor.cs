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

        if (GUILayout.Button("Snap To Ground"))
            orientation.SnapToGround();

        EditorGUILayout.Space(10);
        DrawDefaultInspector();
    }
}
