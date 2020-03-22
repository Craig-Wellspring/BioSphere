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

    private Transform planetCore;


    public void Start()
    {
        planetCore = PlanetCore.Core.transform;

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
                if (RandomRoll() <= seedSuccessChance)
                    CreateFruit(newFruit[Random.Range(0, newFruit.Count)]);
            }

            if (whenToSpawn == WhenToSpawn.OnFullyGrown && GetComponent<GenericGrow>().fullyGrown)
            {
                seeds -= 1;
                if (RandomRoll() <= seedSuccessChance)
                    CreateFruit(newFruit[Random.Range(0, newFruit.Count)]);
            }

            if (whenToSpawn == WhenToSpawn.OnTrigger && triggerSpawn)
            {
                seeds -= 1;
                triggerSpawn = false;
                if (RandomRoll() <= seedSuccessChance)
                    CreateFruit(newFruit[Random.Range(0, newFruit.Count)]);
            }
        }
    }

    private void CreateFruit(GameObject _newFruit)
    {
            //Get desired spawn location
            Vector3 newPos = transform.position;
            if (randomSpawnPosition)
            {
                Vector3 randomPoint = GetRandomPoint();
                newPos = new Vector3(randomPoint.x, transform.position.y, randomPoint.z);
            }

        //Get desired rotation
        Vector3 gravityUp = (transform.position - planetCore.position).normalized;
        Quaternion newRot = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        if (randomYRotation)
            newRot = Quaternion.Euler(newRot.x, (Random.Range(1f, 360f)), newRot.z);
        //Quaternion newRot = transform.rotation;


            //Get Parent relationship
            Transform parent = null;
            if (parentRelationship == ParentType.Child)
                parent = transform;
            if (parentRelationship == ParentType.Sibling && transform.parent != null)
                parent = transform.parent;


            //Spawn Child Fruit
            GameObject tmpFruit = (GameObject)Instantiate(_newFruit, newPos, newRot, parent);
            tmpFruit.name = _newFruit.name;
    }

    Vector3 GetRandomPoint()
    {
        float xRandom = 0;
        float zRandom = 0;

        xRandom = (float)Random.Range(transform.position.x - spawnArea, transform.position.x + spawnArea);
        zRandom = (float)Random.Range(transform.position.z - spawnArea, transform.position.z + spawnArea);

        return new Vector3(xRandom, 0.0f, zRandom);
    }

    private float RandomRoll()
    {
        return Random.Range(1f, 100f);
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
}
