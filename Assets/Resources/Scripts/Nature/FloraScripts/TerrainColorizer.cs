using UnityEngine;

public class TerrainColorizer : MonoBehaviour
{
    int vert1Index;
    int vert2Index;
    int vert3Index;

    Color vert1ColorChange;
    Color vert2ColorChange;
    Color vert3ColorChange;


    TerrainColor localTerrain;

    void OnEnable()
    {
        // Cache local terrain
        localTerrain = UtilityFunctions.GroundBelowPosition(transform.root.position + transform.root.up).terrainObject.GetComponent<TerrainColor>();
    }


    public void ColorizeTerrain(float _increment)
    {
        if (UtilityFunctions.AboveSeaLevel(transform.root.position))
        {
            // Color terrain a little more green on spawn
            if (Physics.Raycast(transform.root.position + transform.root.up, UtilityFunctions.GravityVector(transform.root.position), out RaycastHit groundRayHit, 5, LayerMask.GetMask("Geosphere")))
            {
                // Cache verts of mesh face under self
                vert1Index = localTerrain.terrainMesh.triangles[groundRayHit.triangleIndex * 3 + 0];
                vert2Index = localTerrain.terrainMesh.triangles[groundRayHit.triangleIndex * 3 + 1];
                vert3Index = localTerrain.terrainMesh.triangles[groundRayHit.triangleIndex * 3 + 2];

                // Color vertexes
                Color newColor1 = Color.Lerp(localTerrain.colorArray[vert1Index], localTerrain.terrainGradient.Evaluate(1), _increment);
                vert1ColorChange = newColor1 - localTerrain.colorArray[vert1Index];
                localTerrain.colorArray[vert1Index] = newColor1;

                Color newColor2 = Color.Lerp(localTerrain.colorArray[vert2Index], localTerrain.terrainGradient.Evaluate(1), _increment);
                vert2ColorChange = newColor2 - localTerrain.colorArray[vert2Index];
                localTerrain.colorArray[vert2Index] = newColor2;

                Color newColor3 = Color.Lerp(localTerrain.colorArray[vert3Index], localTerrain.terrainGradient.Evaluate(1), _increment);
                vert3ColorChange = newColor3 - localTerrain.colorArray[vert3Index];
                localTerrain.colorArray[vert3Index] = newColor3;

                // Update terrain coloration
                localTerrain.RefreshTerrainColor();
            }
            else
                Debug.LogError("Colorizer Raycast hit nothing.");
        }
    }

    void OnDisable()
    {
        // Color vertexes
        localTerrain.colorArray[vert1Index] = localTerrain.colorArray[vert1Index] - vert1ColorChange;
        localTerrain.colorArray[vert2Index] = localTerrain.colorArray[vert2Index] - vert2ColorChange;
        localTerrain.colorArray[vert3Index] = localTerrain.colorArray[vert3Index] - vert3ColorChange;

        // Update terrain coloration
        localTerrain.RefreshTerrainColor();
    }
}