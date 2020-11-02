using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableFoliage : MonoBehaviour
{
    [SerializeField] List<FoliageVariantData> randomFoliage;

    void Start()
    {
        FoliageVariantData randomVariation = randomFoliage[Random.Range(0, randomFoliage.Count)];
        int totalBudCount = randomVariation.majorBudPositions.Count + randomVariation.minorBudPositions.Count;

        // Set object name
        name = randomVariation.foliageName;

        // Set trunk mesh and collider
        Transform trunk = transform.GetChild(0);
        trunk.GetComponent<MeshFilter>().mesh = randomVariation.trunkMesh;
        trunk.GetComponent<MeshCollider>().sharedMesh = randomVariation.trunkMesh;

        // Add leaf objects if multiple spawning positions in Variant
        if (totalBudCount > 1)
        {
            GrowthData growthData = trunk.GetComponent<GrowthData>();
            for (int i = 0; i < totalBudCount - 1; i++)
            {
                GameObject newLeaf = Instantiate(trunk.GetChild(0).gameObject, trunk.position, trunk.rotation, trunk);
                newLeaf.name = trunk.GetChild(0).name;
                newLeaf.transform.localScale = Vector3.zero;

                growthData.activateObjectsHG.Add(newLeaf);
            }
        }

        // Set mesh, collider, and nutritional value of each leaf object
        for (int i = 0; i < totalBudCount; i++)
        {
            Transform _targetLeaf = trunk.GetChild(i);

            if (i < randomVariation.majorBudPositions.Count)
            {
                Mesh randomMesh = randomVariation.majorBudMeshes[Random.Range(0, randomVariation.majorBudMeshes.Count)];
                _targetLeaf.GetComponentInChildren<MeshFilter>().mesh = randomMesh;
                _targetLeaf.GetComponent<MeshCollider>().sharedMesh = randomMesh;
                _targetLeaf.localPosition = randomVariation.majorBudPositions[i];
            }
            else
            {
                Mesh randomMesh = randomVariation.minorBudMeshes[Random.Range(0, randomVariation.minorBudMeshes.Count)];
                _targetLeaf.GetComponentInChildren<MeshFilter>().mesh = randomMesh;
                _targetLeaf.GetComponent<MeshCollider>().sharedMesh = randomMesh;
                _targetLeaf.localPosition = randomVariation.minorBudPositions[i - randomVariation.majorBudPositions.Count];
            }
        }
    }
}