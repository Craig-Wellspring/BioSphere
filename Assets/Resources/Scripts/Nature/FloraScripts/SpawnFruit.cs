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
    [Tooltip("Spawn seed on a random vertex of this object's mesh.")]
    [SerializeField] bool spawnOnSurface = false;
    [Space(10)]
    [Tooltip("Must spawn seed above sea level.")]
    [SerializeField] bool aboveWaterOnly = true;
    [Tooltip("Scales the size of the random spawn area. 0 is not random.")]
    [SerializeField] float randomSpawnArea = 0;
    [SerializeField] bool randomYRotation = false;
    [SerializeField] bool spawnScale0 = false;



    [Header("Seed Settings")]
    [Tooltip("Current / Maximum")]
    public Vector2 seeds;
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


    // Cache
    EnergyData rootEData;
    GrowthData growthData;

    void Start()
    {
        rootEData = transform.root.GetComponent<EnergyData>();
        growthData = GetComponent<GrowthData>();


        if (growthData)
        {
            growthData.halfGrownTrigger += HalfGrown;
            growthData.fullyGrownTrigger += FullyGrown;
        }


        if (whenToSpawn == WhenToSpawn.OnSelfSpawn)
            TriggerSpawn();
    }

    void OnEnable()
    {
        seeds.x = seeds.y;
    }

    void HalfGrown()
    {
        if (whenToSpawn == WhenToSpawn.OnHalfGrown)
            TriggerSpawn();
    }

    void FullyGrown()
    {
        if (whenToSpawn == WhenToSpawn.OnFullyGrown)
            TriggerSpawn();
    }

    public void TriggerSpawn()
    {
        if (newFruit.Count > 0)
        {
            if (seeds.x > 0)
            {
                // Select fruit to spawn
                GameObject fruitToSpawn = newFruit[Random.Range(0, newFruit.Count)];

                float energyRequired = 0;
                FoodData[] fruitFData = fruitToSpawn.GetComponentsInChildren<FoodData>(true);
                foreach (FoodData fData in fruitFData)
                    energyRequired += fData.nutritionalValue.y;


                // Check if able to spawn
                if (rootEData.energyReserve >= energyRequired)
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
                                energyToGive = Mathf.Max(rootEData.energyReserve / seeds.x, energyRequired);
                                break;
                        }

                        // Spawn Fruit
                        GameObject _spawnedFruit = SpawnObject(fruitToSpawn, rootEData, energyToGive, parent, randomYRotation, randomSpawnArea, aboveWaterOnly);


                        // Adjust scale
                        if (spawnScale0)
                            _spawnedFruit.GetComponentInChildren<Animator>().transform.localScale = Vector3.zero;

                        // Position randomly on spawning surface
                        if (spawnOnSurface)
                        {
                            MeshFilter objectMesh = GetComponent<MeshFilter>();
                            if (!objectMesh)
                                objectMesh = transform.GetComponentInChildren<MeshFilter>();

                            Vector3 spawnPoint = transform.TransformPoint(objectMesh.mesh.vertices[Random.Range(0, objectMesh.mesh.vertices.Length)]);
                            _spawnedFruit.transform.position = spawnPoint;

                            Quaternion spawnRotation = Quaternion.FromToRotation(_spawnedFruit.transform.up, (objectMesh.GetComponent<MeshRenderer>().bounds.center - _spawnedFruit.transform.position).normalized) * _spawnedFruit.transform.rotation;
                            _spawnedFruit.transform.rotation = spawnRotation;
                        }
                    }

                    seeds.x--;
                    TriggerSpawn();
                }
                else  // If not enough Energy to spawn new Fruit
                {
                    // Spawn CastOff Entity
                    if (castOffEntity != null && rootEData.energyReserve > 0)
                        SpawnObject(castOffEntity, rootEData, rootEData.energyReserve, null, false, castOffDistance, true);

                    // Clear remaining seeds
                    seeds.x = 0;
                }
            }

            // When no seeds remain return leftover Energy to Source
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
