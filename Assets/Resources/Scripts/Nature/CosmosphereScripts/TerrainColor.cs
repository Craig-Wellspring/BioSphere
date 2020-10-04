using UnityEngine;

[ExecuteInEditMode]
public class TerrainColor : MonoBehaviour
{
    [HideInInspector] public Mesh terrainMesh;  
    [HideInInspector] public Color[] colorArray;

    void Start()
    {
        // Cache
        terrainMesh = GetComponent<MeshFilter>().sharedMesh;

        // Initialize color array
        colorArray = new Color[terrainMesh.vertices.Length];

        for (int i = 0; i < terrainMesh.vertices.Length; i++)
            colorArray[i] = GetComponentInParent<SingleGradient>().gradient.Evaluate(0);

        RefreshTerrainColor();
    }

    public void RefreshTerrainColor()
    {
        terrainMesh.colors = colorArray;
    }
}
