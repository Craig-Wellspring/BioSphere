using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VisualPerception))]
public class VPerceptionEditor : Editor
{
    void OnSceneGUI()
    {

    }

    void OnDrawGizmosSelected()
    {
        VisualPerception vPerception = (VisualPerception)target;
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(vPerception.transform.position, vPerception.perceptionRadius);
    }
}
