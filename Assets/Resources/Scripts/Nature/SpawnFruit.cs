using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SpawnFruit : MonoBehaviour
{
    [Header("Fruit Settings")]
    [Tooltip("Chooses randomly from list")]
    public List<GameObject> newFruit;

    public enum ParentType { None, Child, Sibling };
    public ParentType parentRelationship;

    public enum WhenToSpawn { OnSelfSpawn, OnFullyGrown, OnTrigger };
    public WhenToSpawn whenToSpawn;

    [Header("Seed Settings")]
    public int seeds = 1;
    public float seedSuccessChance = 100;
    [Tooltip("Species population size affects seed success chance")]
    public bool popBasedSeedChance = false;
    public GameObject basedOnPopOf;
    public int populationModifier = 800;

    [Header("Placing Settings")]
    public bool randomYRotation = false;
    public bool randomSpawnPosition = false;
    public float spawnArea = 1f;

    [Header("Triggers")]
    public bool triggerSpawn = false;
    public bool drawDebugSeed = false;

    //private BoxCollider seedPodArea;
    
    
    public void Start()
    {
        /*
        seedPodArea = GetComponent<BoxCollider>();
        seedPodArea.size.Set(spawnArea, 0.001f, spawnArea);
        seedPodArea.center.Set(0, 50, 0);
        */

        //Adjust spawn area
        if (randomSpawnPosition)
        {
            transform.localScale = new Vector3(spawnArea, 1, spawnArea);
            transform.localPosition = new Vector3(0f, 100f, 0f);
        }

        //Adjust spawn chance
        if (popBasedSeedChance)
        {
            //success chance modifier gets lower as current pop gets higher
            seedSuccessChance = 100f * (1f / CurrentPopulation(basedOnPopOf.name)) * (populationModifier * 0.2f);
        }
    }


    public void Update()
    {

        /*if (seedSuccessChance > 200)
        {
            seedSuccessChance = 100;
            seeds += 1;
        }*/

        if (seeds > 0 && newFruit.Count > 0)
        {
            if (whenToSpawn == WhenToSpawn.OnSelfSpawn)
            {
                seeds -= 1;
                if (Random.Range(1f, 100f) <= seedSuccessChance)
                    CreateFruit(newFruit[Random.Range(0, newFruit.Count)]);
            }

            if (whenToSpawn == WhenToSpawn.OnFullyGrown && transform.root.GetComponentInChildren<GenericGrow>().fullyGrown)
            {
                seeds -= 1;
                if (Random.Range(1f, 100f) <= seedSuccessChance)
                    CreateFruit(newFruit[Random.Range(0, newFruit.Count)]);
            }

            if (whenToSpawn == WhenToSpawn.OnTrigger && triggerSpawn)
            {
                seeds -= 1;
                triggerSpawn = false;
                if (Random.Range(1f, 100f) <= seedSuccessChance)
                    CreateFruit(newFruit[Random.Range(0, newFruit.Count)]);
            }
        }

        if (drawDebugSeed)
        {
            DrawDebug();
            drawDebugSeed = false;
        }
    }


    private void CreateFruit(GameObject _newFruit)
    {
        //Get desired spawn position
        Vector3 newPos = transform.root.position;
        if (randomSpawnPosition)
        {
            Vector3 seedCloudPos = GetRandomPointOnCol(GetComponent<BoxCollider>());
            newPos = PointOnTerrainUnderPosition(seedCloudPos);
        }

        //Get desired rotation
        Quaternion newRot = Quaternion.FromToRotation(transform.root.up, -GravityVector(transform.root.position)) * transform.root.rotation;
        

        //Get Parent relationship
        Transform parent = null;
        if (parentRelationship == ParentType.Child)
            parent = transform;
        if (parentRelationship == ParentType.Sibling && transform.parent != null)
            parent = transform.parent;


        //Spawn Child Fruit
        if (newPos != Vector3.zero)
        {
            GameObject fruitToSpawn = (GameObject)Instantiate(_newFruit, newPos, newRot, parent);
            fruitToSpawn.name = _newFruit.name;
            if (randomYRotation)
                fruitToSpawn.transform.RotateAround(fruitToSpawn.transform.position, fruitToSpawn.transform.up, Random.Range(1f, 360f));
        }
        else
        {
            //Failed to find appropriate surface, recast seed
            CreateFruit(_newFruit);
        }
    }


    #region Custom Functions
    private void DrawDebug()
    {
        //Find potential planting location
        Vector3 drawFromPos = GetRandomPointOnCol(GetComponent<BoxCollider>());
        Vector3 drawSpherePoint = PointOnTerrainUnderPosition(drawFromPos);

        //Draw Sphere at potential planting location
        Debug.DrawRay(drawFromPos, GravityVector(drawFromPos), Color.red, 10);
        GameObject debugSphere = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), drawSpherePoint, transform.rotation);
        debugSphere.transform.localScale *= 0.2f;
        Destroy(debugSphere, 10);
    }


    private Vector3 GetRandomPointOnCol(Collider collider)
    {
        return new Vector3(
            Random.Range(collider.bounds.min.x, collider.bounds.max.x),
            Random.Range(collider.bounds.min.y, collider.bounds.max.y),
            Random.Range(collider.bounds.min.z, collider.bounds.max.z));
    }


    private Vector3 PointOnTerrainUnderPosition(Vector3 fromPos)
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(fromPos, GravityVector(fromPos), out hit, 5000, 1 << 26))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                return hit.point;
            }
            else
            {
                //Debug.Log("SeedCast Ray didn't hit [Ground]");
                return Vector3.zero;
            }
        }
        else
        {
            //Debug.Log("SeedCast Ray didn't hit [Anything]");
            return Vector3.zero;
        }
    }


    private Vector3 GravityVector(Vector3 fromPos)
    {
        Vector3 gravityUp = (fromPos - PlanetCore.Core.transform.position).normalized;
        return -gravityUp;
    }


    int CurrentPopulation(string nameOfSpecies)
    {
        int currentPopulation = 0;
        foreach (CensusData speciesType in CensusMaster.Census.listOfSpecies)
        {
            if (speciesType.speciesName.Contains(nameOfSpecies))
            {
                currentPopulation = speciesType.populationSize;
                break;
            }
        }
        return currentPopulation;
    }
    #endregion
}
