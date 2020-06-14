using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SeedSpawner))]
public class FoodData : MonoBehaviour
{
    [Tooltip("On Destroy, entity will spawn Seedgrass with remaining Energy Stored")]
    public float energyStored;
    public float nutritionalValue = 1f;
    public float timeToEat = 5f;
    public float chewRateModifier = 1f;
    //public bool currentlyEdible = true;
    [Tooltip("Destroy the parent entity when eaten")]
    public bool destroyParent = true;

    private bool spawnSeed = true;
    private SeedSpawner seedSpawner;

    private void Start()
    {
        seedSpawner = GetComponent<SeedSpawner>();
    }

    private void OnApplicationQuit()
    {
        spawnSeed = false;
    }

    private void OnDisable()
    {
        if (spawnSeed && (energyStored > 0 || nutritionalValue > 0))
            seedSpawner.PlantSeed();
    }

}
