using UnityEngine;
using System.Collections.Generic;

public class SpawnFruit : ObjectSpawner
{
    [Header("Fruit Settings")]

    [Tooltip("Chooses randomly from list")]
    [SerializeField] List<GameObject> newFruit;

    public enum WhenToSpawn { OnSelfSpawn, OnHalfGrown, OnFullyGrown, OnTrigger };
    public WhenToSpawn whenToSpawn;



    [Header("Placing Settings")]
    [SerializeField] ParentType parentRelationship;
    enum ParentType { None, Child, Sibling };
    [SerializeField] bool spawnOnSurface = false;
    [Space(10)]
    [Tooltip("Scales the size of the random spawn area. 0 is not random.")]
    [SerializeField] float randomSpawnArea = 0;
    [SerializeField] bool randomYRotation = false;
    [SerializeField] bool spawnScale0 = false;



    [Header("Seed Settings")]
    public int seeds = 1;
    [SerializeField] float seedSuccessChance = 100;
    [SerializeField] EnergyDistribution energyDistribution;
    enum EnergyDistribution { Minimum, Maximum, DivideRemaining };



    [Header("Exhaustion Settings")]

    [Tooltip("Spawn selected Entity when remaining Energy is too low to spawn selected Fruit")]
    [SerializeField] GameObject castOffEntity;
    [SerializeField] float castOffDistance;
    [Space(10)]
    [Tooltip("When out of Seeds, activate indicated Object")]
    [SerializeField] GameObject objectToActivate;
    [Tooltip("When out of Seeds, return remaining Energy stored to the Source")]
    [SerializeField] bool returnLeftoverEnergy = true;
    [Tooltip("When out of Seeds, destroy this Object")]
    [SerializeField] bool selfDestruct = false;



    [Header("Triggers")]
    public bool triggerSpawn = false;


    EnergyData rootEData;
    GrowthData rootGrowthData;

    public void Start()
    {
        rootEData = transform.root.GetComponentInChildren<EnergyData>();
        rootGrowthData = transform.root.GetComponentInChildren<GrowthData>();
    }


    public void Update()
    {
        if (seeds > 0 && newFruit.Count > 0)
        {
            // Select fruit to spawn
            GameObject fruitToSpawn = newFruit[Random.Range(0, newFruit.Count)];

            float energyRequired = 0;
            FoodData[] fruitFData = fruitToSpawn.GetComponentsInChildren<FoodData>(true);
            foreach (FoodData fData in fruitFData)
                energyRequired += fData.nutritionalValue;

            // Check if able to spawn
            if (rootEData.energyReserve >= energyRequired)
            {
                if ((whenToSpawn == WhenToSpawn.OnSelfSpawn) ||
                    (whenToSpawn == WhenToSpawn.OnHalfGrown && rootGrowthData.halfGrown) ||
                    (whenToSpawn == WhenToSpawn.OnFullyGrown && rootGrowthData.fullyGrown) ||
                    (whenToSpawn == WhenToSpawn.OnTrigger && triggerSpawn))
                {
                    // Spawn fruit if conditions are met and remove seed
                    if (Random.Range(1f, 100f) <= seedSuccessChance)
                    {
                        //Get Parent relationship
                        Transform parent = null;
                        switch (parentRelationship)
                        {
                            case (ParentType.None):
                                break;

                            case (ParentType.Child):
                                parent = transform;
                                break;

                            case (ParentType.Sibling):
                                parent = transform.parent;
                                break;
                        }

                        // Decide how much Energy to pass down
                        float energyToGive = 0;
                        switch (energyDistribution)
                        {
                            case (EnergyDistribution.Minimum):
                                energyToGive = energyRequired;
                                break;

                            case (EnergyDistribution.Maximum):
                                energyToGive = rootEData.energyReserve;
                                break;

                            case (EnergyDistribution.DivideRemaining):
                                energyToGive = System.Math.Max(rootEData.energyReserve / seeds, energyRequired);
                                break;
                        }

                        // Spawn Fruit
                        GameObject _spawnedFruit = SpawnObject(fruitToSpawn, rootEData, energyToGive, 0, parent, randomYRotation, randomSpawnArea);


                        // Adjust scale
                        if (spawnScale0)
                            _spawnedFruit.GetComponentInChildren<Animator>().transform.localScale = Vector3.zero;
                
                        // Position randomly on spawning surface
                        if (spawnOnSurface)
                        {
                            Mesh objectMesh = transform.GetComponentInChildren<MeshFilter>().mesh;
                            int randomVertIndex = Random.Range(0, objectMesh.vertices.Length);
                            Vector3 spawnPoint = transform.TransformPoint(objectMesh.vertices[randomVertIndex]);
                            Quaternion spawnRotation = Quaternion.FromToRotation(_spawnedFruit.transform.up, (transform.root.GetComponentInChildren<MeshRenderer>().bounds.center - _spawnedFruit.transform.position).normalized) * _spawnedFruit.transform.rotation;

                            _spawnedFruit.transform.position = spawnPoint;
                            _spawnedFruit.transform.rotation = spawnRotation;
                        }
                    }

                    seeds -= 1;

                    triggerSpawn = false;
                }
            }
            else  // If not enough Energy to spawn new Fruit
            {
                // Spawn CastOff Entity
                if (castOffEntity != null && rootEData.energyReserve > 0)
                    SpawnObject(castOffEntity, rootEData, rootEData.energyReserve, 0, null, false, castOffDistance);

                // Clear remaining seeds
                seeds = 0;
            }
        }
        else  // If no seeds remain
        {
            // Return leftover Energy to Source
            if (returnLeftoverEnergy)
            {
                Servius.Server.GetComponent<GlobalLifeSource>().energyReserve += rootEData.energyReserve;
                rootEData.energyReserve = 0;
            }

            // Activate next Object
            if (objectToActivate != null)
                objectToActivate.SetActive(true);

            // Destroy self
            if (selfDestruct)
                Destroy(this.gameObject);
        }
    }
}
