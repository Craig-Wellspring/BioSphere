using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[RequireComponent(typeof(ObjectSpawner))]
public class SpawnFruit : AdvancedMonoBehaviour
{
    [Header("Fruit Settings")]
    [Tooltip("Chooses randomly from list")]
    public List<GameObject> newFruit;


    public enum WhenToSpawn { OnSelfSpawn, OnHalfGrown , OnFullyGrown, OnTrigger };
    public WhenToSpawn whenToSpawn;




    [Header("Seed Settings")]
    public int seeds = 1;
    public float seedSuccessChance = 100;
    public bool divideRemainingEnergy = false;
    [Tooltip("When out of Seeds, deactive this Pod and activate selected Pod")]
    public GameObject nextSeedPod;
    [Space(10)]
    [Tooltip("Spawn selected Entity when remaining Energy is too low to spawn selected Fruit")]
    public GameObject castOffEntity;
    public float castOffDistance;


    [Header("Placing Settings")]
    [Tooltip("Scales the size of the random spawn area. 0 is not random. Will reset Transform.")]
    public float randomSpawnArea = 0;
    public bool randomYRotation = false;
    public enum ParentType { None, Child, Sibling };
    public ParentType parentRelationship;


    [Header("Triggers")]
    public bool triggerSpawn = false;


    private ObjectSpawner spawner;
    private EnergyData rootEData;
    private GrowthData rootGrowthData;
    
    
    public void Start()
    {
        spawner = GetComponent<ObjectSpawner>();
        rootEData = transform.root.GetComponentInChildren<EnergyData>();
        rootGrowthData = transform.root.GetComponentInChildren<GrowthData>();
    }


    public void Update()
    {
        if (seeds > 0 && newFruit.Count > 0)
        {
            //Select fruit to spawn
            GameObject fruitToSpawn = newFruit[Random.Range(0, newFruit.Count)];
            float energyRequired = fruitToSpawn.GetComponentInChildren<EnergyData>().nutritionalValue;

            //Check if able to spawn
            if (rootEData.energyReserve > energyRequired)
            {
                if ((whenToSpawn == WhenToSpawn.OnSelfSpawn) ||
                    (whenToSpawn == WhenToSpawn.OnHalfGrown && rootGrowthData.halfGrown) ||
                    (whenToSpawn == WhenToSpawn.OnFullyGrown && rootGrowthData.fullyGrown) ||
                    (whenToSpawn == WhenToSpawn.OnTrigger && triggerSpawn))
                {
                    //Spawn fruit if conditions met and remove seed
                    if (Random.Range(1f, 100f) <= seedSuccessChance)
                    {
                        //Get Parent relationship
                        Transform parent = null;
                        if (parentRelationship == ParentType.Child)
                            parent = transform;
                        if (parentRelationship == ParentType.Sibling && transform.parent != null)
                            parent = transform.parent;

                        //Decide how much Energy to pass down
                        float energyToGive;
                        if (divideRemainingEnergy)
                            energyToGive = rootEData.energyReserve / seeds;
                        else
                            energyToGive = fruitToSpawn.GetComponentInChildren<EnergyData>().nutritionalValue;


                        spawner.SpawnObject(fruitToSpawn, randomSpawnArea, randomYRotation, parent, energyToGive, rootEData);
                    }

                    seeds -= 1;
                    
                    triggerSpawn = false;
                }
            }
            else
            {
                //Spawn CastOff Entity if not enough Energy remains to spawn new Fruit
                if (castOffEntity != null && rootEData.energyReserve > 0)
                {
                    spawner.SpawnObject(castOffEntity, castOffDistance, true, null, rootEData.energyReserve, rootEData);
                }
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
}
