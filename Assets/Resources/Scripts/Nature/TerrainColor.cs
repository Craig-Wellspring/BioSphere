using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainColor : MonoBehaviour
{
    [HideInInspector] public Mesh terrainMesh;
    [HideInInspector] public Gradient terrainGradient;    
    [HideInInspector] public Color[] colorArray;

    void Start()
    {
        // Cache
        terrainMesh = GetComponent<MeshFilter>().sharedMesh;
        terrainGradient = GetComponentInParent<GeoColor>().terrainColorGradient;

        // Initialize color array
        colorArray = new Color[terrainMesh.vertices.Length];

        for (int i = 0; i < terrainMesh.vertices.Length; i++)
            colorArray[i] = terrainGradient.Evaluate(0);

        RefreshTerrainColor();
    }

    public void RefreshTerrainColor()
    {
        terrainMesh.colors = colorArray;
    }
}
