using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainColorizer : AdvancedMonoBehaviour
{
    // Settings
    [SerializeField] float colorIncrement = 1;


    // Private variables
    TerrainColor localTerrainColor;
    RaycastHit groundRayHit;

    int vert1Index;
    int vert2Index;
    int vert3Index;

    void OnEnable()
    {
        // Cache local terrain
        localTerrainColor = TerrainUnderPosition(transform.position).GetComponent<TerrainColor>();


        // Color terrain a little more green on spawn
        if (Physics.Raycast(transform.root.position + transform.root.position.normalized, -transform.root.position.normalized, out groundRayHit, 200, LayerMask.GetMask("Terrain")))
        {
            if (groundRayHit.collider.CompareTag("Ground"))
            {
                // Cache verts of mesh face under self
                vert1Index = localTerrainColor.terrainMesh.triangles[groundRayHit.triangleIndex * 3 + 0];
                vert2Index = localTerrainColor.terrainMesh.triangles[groundRayHit.triangleIndex * 3 + 1];
                vert3Index = localTerrainColor.terrainMesh.triangles[groundRayHit.triangleIndex * 3 + 2];

                // Color vertexes
                localTerrainColor.colorArray[vert1Index] = Color.Lerp(localTerrainColor.colorArray[vert1Index], localTerrainColor.terrainGradient.Evaluate(1), colorIncrement);
                localTerrainColor.colorArray[vert2Index] = Color.Lerp(localTerrainColor.colorArray[vert2Index], localTerrainColor.terrainGradient.Evaluate(1), colorIncrement);
                localTerrainColor.colorArray[vert3Index] = Color.Lerp(localTerrainColor.colorArray[vert3Index], localTerrainColor.terrainGradient.Evaluate(1), colorIncrement);
                
                // Update terrain coloration
                localTerrainColor.RefreshTerrainColor();
            }
        }
    }
    
    void OnDisable()
    {
        // Color vertexes
        localTerrainColor.colorArray[vert1Index] = Color.Lerp(localTerrainColor.colorArray[vert1Index], localTerrainColor.terrainGradient.Evaluate(0), colorIncrement * 2);
        localTerrainColor.colorArray[vert2Index] = Color.Lerp(localTerrainColor.colorArray[vert2Index], localTerrainColor.terrainGradient.Evaluate(0), colorIncrement * 2);
        localTerrainColor.colorArray[vert3Index] = Color.Lerp(localTerrainColor.colorArray[vert3Index], localTerrainColor.terrainGradient.Evaluate(0), colorIncrement * 2);

        // Update terrain coloration
        localTerrainColor.RefreshTerrainColor();
    }
}
