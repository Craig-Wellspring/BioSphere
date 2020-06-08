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
    public bool divideRemainingEnergy = false;
    [Tooltip("When out of Seeds, deactive this Pod and activate selected Pod")]
    public GameObject nextSeedPod;
    [Tooltip("Spawn selected Entity when remaining Energy is too low to spawn selected Fruit")]
    public GameObject castOffEntity;

    /*[Tooltip("Species population size affects seed success chance")]
    public bool popBasedSeedChance = false;
    public GameObject basedOnPopOf;
    public int populationModifier = 800;*/

    [Header("Placing Settings")]
    public bool randomYRotation = false;
    public bool randomSpawnPosition = false;
    public float spawnArea = 1f;

    [Header("Triggers")]
    public bool triggerSpawn = false;
    public bool drawDebugSeed = false;

    private FoodData rootFData;
    
    
    public void Start()
    {
        rootFData = transform.root.GetComponentInChildren<FoodData>();

        //Adjust spawn area
        if (randomSpawnPosition)
        {
            transform.localScale = new Vector3(spawnArea, 1, spawnArea);
            transform.localPosition = new Vector3(0f, 50f, 0f);
        }

        /*
        //Adjust spawn chance
        if (popBasedSeedChance)
        {
            //success chance modifier gets lower as current pop gets higher
            seedSuccessChance = 100f * (1f / CurrentPopulation(basedOnPopOf.name)) * (populationModifier * 0.2f);
        }*/
    }


    public void Update()
    {
        if (seeds > 0 && newFruit.Count > 0)
        {
            GameObject fruitToSpawn = newFruit[Random.Range(0, newFruit.Count)];
            float energyRequired = fruitToSpawn.GetComponentInChildren<FoodData>().nutritionalValue;

            if (rootFData.energyStored > energyRequired)
            {
                if ((whenToSpawn == WhenToSpawn.OnSelfSpawn) ||
                    (whenToSpawn == WhenToSpawn.OnFullyGrown && transform.root.GetComponentInChildren<GenericGrow>().fullyGrown) ||
                    (whenToSpawn == WhenToSpawn.OnTrigger && triggerSpawn))
                {
                    if (Random.Range(1f, 100f) <= seedSuccessChance)
                        CreateFruit(fruitToSpawn);

                    seeds -= 1;
                    
                    triggerSpawn = false;
                }
            }
            else
            {
                //Spawn CastOff Entity if not enough Energy remains to spawn new Fruit
                if (castOffEntity != null && rootFData.energyStored > 0)
                    SpawnCastoff();
                seeds = 0;
            }
        }
        else
        {
            if (nextSeedPod != null)
                nextSeedPod.SetActive(true);

            this.gameObject.SetActive(false);
        }
    }

    private void OnValidate()
    {
        //Debug trigger
        if (drawDebugSeed)
        {
            DrawDebug();
            drawDebugSeed = false;
        }
    }

    #region Custom Functions

    private void CreateFruit(GameObject _newFruit)
    {

        FoodData newFData = _newFruit.GetComponentInChildren<FoodData>();
        Vector3 spawnPos = GetSpawnLocation();

        //Get Parent relationship
        Transform parent = null;
        if (parentRelationship == ParentType.Child)
            parent = transform;
        if (parentRelationship == ParentType.Sibling && transform.parent != null)
            parent = transform.parent;


        //Spawn Child Fruit
        if (spawnPos != Vector3.zero)
        {
            GameObject fruitToSpawn = (GameObject)Instantiate(_newFruit, spawnPos, SpawnOrientation(), parent);
            fruitToSpawn.name = _newFruit.name;

            //Random Rotation
            if (randomYRotation)
                fruitToSpawn.transform.RotateAround(fruitToSpawn.transform.position, fruitToSpawn.transform.up, Random.Range(1f, 360f));


            //Allocate energy
            float energyToGive;
            if (divideRemainingEnergy)
                energyToGive = rootFData.energyStored / seeds;
            else
                energyToGive = newFData.nutritionalValue;

            rootFData.energyStored -= energyToGive;
            fruitToSpawn.GetComponentInChildren<FoodData>().energyStored = energyToGive - newFData.nutritionalValue;
        }
        else
        {
            //Failed to find appropriate surface, recast seed
            CreateFruit(_newFruit);
        }
    }

    private void SpawnCastoff()
    {
        GameObject castOff = (GameObject)Instantiate(castOffEntity, transform.position, transform.rotation);
        castOff.name = castOffEntity.name;

        castOff.GetComponent<FoodData>().nutritionalValue += rootFData.energyStored;
        rootFData.energyStored = 0;
    }


    private Vector3 GetSpawnLocation()
    {
        //Get desired spawn position
        Vector3 pos = transform.root.position;
        if (randomSpawnPosition)
        {
            Vector3 seedCloudPos = GetRandomPointOnCol(GetComponent<BoxCollider>());
            pos = PointOnTerrainUnderPosition(seedCloudPos);
        }
        
        return pos;
    }

    private Quaternion SpawnOrientation()
    {
        //Get desired rotation
        Quaternion rot = Quaternion.FromToRotation(transform.root.up, -GravityVector(transform.root.position)) * transform.root.rotation;

        return rot;
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
    #endregion
}
