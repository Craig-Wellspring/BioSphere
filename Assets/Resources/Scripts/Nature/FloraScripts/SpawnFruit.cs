using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SpawnFruit : AdvancedMonoBehaviour
{
    [Header("Fruit Settings")]
    [Tooltip("Chooses randomly from list")]
    public List<GameObject> newFruit;

    public enum ParentType { None, Child, Sibling };
    public ParentType parentRelationship;

    public enum WhenToSpawn { OnSelfSpawn, OnHalfGrown , OnFullyGrown, OnTrigger };
    public WhenToSpawn whenToSpawn;

    [Tooltip("Spawn selected Entity when remaining Energy is too low to spawn selected Fruit")]
    public GameObject castOffEntity;



    [Header("Seed Settings")]
    public int seeds = 1;
    public float seedSuccessChance = 100;
    public bool divideRemainingEnergy = false;
    [Tooltip("When out of Seeds, deactive this Pod and activate selected Pod")]
    public GameObject nextSeedPod;


    [Header("Placing Settings")]
    public bool randomYRotation = false;
    public bool randomSpawnPosition = false;
    public float spawnArea = 1f;


    [Header("Triggers")]
    public bool triggerSpawn = false;
    [SerializeField] private bool drawDebugSeed = false;
    [SerializeField] private bool debugSeedCastRay = false;



    private FoodData rootFData;
    private GrowthData rootGrowthData;
    private LayerMask terrainLayerMask;
    
    
    public void Start()
    {
        rootFData = transform.root.GetComponentInChildren<FoodData>();
        rootGrowthData = transform.root.GetComponentInChildren<GrowthData>();
        terrainLayerMask = LayerMask.GetMask("Terrain");

        //Adjust spawn area
        if (randomSpawnPosition)
        {
            transform.localScale = new Vector3(spawnArea, 1, spawnArea);
            transform.localPosition = new Vector3(0f, 50f, 0f);
        }
    }


    public void Update()
    {
        if (seeds > 0 && newFruit.Count > 0)
        {
            //Select fruit to spawn
            GameObject fruitToSpawn = newFruit[Random.Range(0, newFruit.Count)];
            float energyRequired = fruitToSpawn.GetComponentInChildren<FoodData>().nutritionalValue;

            //Check if able to spawn
            if (rootFData.energyStored > energyRequired)
            {
                if ((whenToSpawn == WhenToSpawn.OnSelfSpawn) ||
                    (whenToSpawn == WhenToSpawn.OnHalfGrown && rootGrowthData.halfGrown) ||
                    (whenToSpawn == WhenToSpawn.OnFullyGrown && rootGrowthData.fullyGrown) ||
                    (whenToSpawn == WhenToSpawn.OnTrigger && triggerSpawn))
                {
                    //Spawn fruit if conditions met and remove seed
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
            //Deactivate self and Activate next SeedPod if no seeds remain
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

        castOff.GetComponentInChildren<FoodData>().nutritionalValue += rootFData.energyStored;
        rootFData.energyStored = 0;
    }


    private Vector3 GetSpawnLocation()
    {
        //Get desired spawn position
        Vector3 pos = transform.root.position;
        if (randomSpawnPosition)
        {
            Vector3 seedCloudPos = GetRandomPointOnCol(GetComponent<Collider>());
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
    
    private void DrawDebug()
    {
        //Find potential planting location
        Vector3 drawFromPos = GetRandomPointOnCol(GetComponent<Collider>());
        Vector3 drawSpherePoint = PointOnTerrainUnderPosition(drawFromPos);

        //Draw Sphere at potential planting location
        Debug.DrawRay(drawFromPos, GravityVector(drawFromPos), Color.red, 10);
        GameObject debugSphere = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), drawSpherePoint, transform.rotation);
        debugSphere.transform.localScale *= 0.2f;
        Destroy(debugSphere, 10);
    }
    #endregion
}
