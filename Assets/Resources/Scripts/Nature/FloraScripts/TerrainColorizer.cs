using UnityEngine;

public class TerrainColorizer : AdvancedMonoBehaviour
{
    TerrainColor localTerrainColor;
    SingleGradient globalTerrainColor;

    int vert1Index;
    int vert2Index;
    int vert3Index;

    Color vert1ColorChange;
    Color vert2ColorChange;
    Color vert3ColorChange;

    void OnEnable()
    {
        // Cache local terrain
        localTerrainColor = TerrainUnderPosition(transform.root.position + transform.root.up).GetComponent<TerrainColor>();
        globalTerrainColor = localTerrainColor.GetComponentInParent<SingleGradient>();
    }


    public void ColorizeTerrain(float _increment)
    {
        // Color terrain a little more green on spawn
        if (Physics.Raycast(transform.root.position + transform.root.up, GravityVector(transform.root.position), out RaycastHit groundRayHit, 5, LayerMask.GetMask("Geosphere")))
        {
            if (groundRayHit.collider.CompareTag("Ground"))
            {
                // Cache verts of mesh face under self
                vert1Index = localTerrainColor.terrainMesh.triangles[groundRayHit.triangleIndex * 3 + 0];
                vert2Index = localTerrainColor.terrainMesh.triangles[groundRayHit.triangleIndex * 3 + 1];
                vert3Index = localTerrainColor.terrainMesh.triangles[groundRayHit.triangleIndex * 3 + 2];

                // Color vertexes
                Color newColor1 = Color.Lerp(localTerrainColor.colorArray[vert1Index], globalTerrainColor.gradient.Evaluate(1), _increment);
                vert1ColorChange = newColor1 - localTerrainColor.colorArray[vert1Index];
                localTerrainColor.colorArray[vert1Index] = newColor1;

                Color newColor2 = Color.Lerp(localTerrainColor.colorArray[vert2Index], globalTerrainColor.gradient.Evaluate(1), _increment);
                vert2ColorChange = newColor2 - localTerrainColor.colorArray[vert2Index];
                localTerrainColor.colorArray[vert2Index] = newColor2;

                Color newColor3 = Color.Lerp(localTerrainColor.colorArray[vert3Index], globalTerrainColor.gradient.Evaluate(1), _increment);
                vert3ColorChange = newColor3 - localTerrainColor.colorArray[vert3Index];
                localTerrainColor.colorArray[vert3Index] = newColor3;

                // Update terrain coloration
                localTerrainColor.RefreshTerrainColor();
            }
            else
            {
                Debug.LogError("Colorizer Raycast hit something not tagged as Ground.");
            }
        }
        else
        {
            Debug.LogError("Colorizer Raycast hit nothing.");
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