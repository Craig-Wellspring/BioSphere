using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainColorizer : AdvancedMonoBehaviour
{
    // Settings
    [SerializeField] float colorIncrement = 1;

    // Private variables
    TerrainColor localTerrainColor;

    int vert1Index;
    int vert2Index;
    int vert3Index;
    
    Color vert1ColorChange;
    Color vert2ColorChange;
    Color vert3ColorChange;

    void OnEnable()
    {
        // Cache local terrain
        localTerrainColor = TerrainUnderPosition(transform.position).GetComponent<TerrainColor>();


        // Color terrain a little more green on spawn
        if (Physics.Raycast(transform.root.position + transform.root.position.normalized, -transform.root.position.normalized, out RaycastHit groundRayHit, 200, LayerMask.GetMask("Terrain")))
        {
            if (groundRayHit.collider.CompareTag("Ground"))
            {
                // Cache verts of mesh face under self
                vert1Index = localTerrainColor.terrainMesh.triangles[groundRayHit.triangleIndex * 3 + 0];
                vert2Index = localTerrainColor.terrainMesh.triangles[groundRayHit.triangleIndex * 3 + 1];
                vert3Index = localTerrainColor.terrainMesh.triangles[groundRayHit.triangleIndex * 3 + 2];

                // Color vertexes
                Color newColor1 = Color.Lerp(localTerrainColor.colorArray[vert1Index], localTerrainColor.terrainGradient.Evaluate(1), colorIncrement);
                vert1ColorChange = newColor1 - localTerrainColor.colorArray[vert1Index];
                localTerrainColor.colorArray[vert1Index] = newColor1;

                Color newColor2 = Color.Lerp(localTerrainColor.colorArray[vert2Index], localTerrainColor.terrainGradient.Evaluate(1), colorIncrement);
                vert2ColorChange = newColor2 - localTerrainColor.colorArray[vert2Index];
                localTerrainColor.colorArray[vert2Index] = newColor2;

                Color newColor3 = Color.Lerp(localTerrainColor.colorArray[vert3Index], localTerrainColor.terrainGradient.Evaluate(1), colorIncrement);
                vert3ColorChange = newColor3 - localTerrainColor.colorArray[vert3Index];
                localTerrainColor.colorArray[vert3Index] = newColor3;
                
                // Update terrain coloration
                localTerrainColor.RefreshTerrainColor();
            }
        }
    }
    
    void OnDisable()
    {
        // Color vertexes
        localTerrainColor.colorArray[vert1Index] = localTerrainColor.colorArray[vert1Index] - vert1ColorChange;
        localTerrainColor.colorArray[vert2Index] = localTerrainColor.colorArray[vert2Index] - vert2ColorChange;
        localTerrainColor.colorArray[vert3Index] = localTerrainColor.colorArray[vert3Index] - vert3ColorChange;

        // Update terrain coloration
        localTerrainColor.RefreshTerrainColor();
    }
}