using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SeedSpawner))]
public class FoodData : MonoBehaviour
{
    [Header("Values")]
    [Tooltip("On Destroy, entity will spawn Seedgrass with remaining Energy Stored")]
    public float energyStored;
    public float nutritionalValue = 1f;

    [Header("Settings")]
    public float chewRateModifier = 1f;
    [Tooltip("Destroy the parent entity when eaten")]
    public bool destroyParent = true;


    private bool spawnSeed = true;
    private SeedSpawner seedSpawner;

    private void Start()
    {
        seedSpawner = transform.root.GetComponentInChildren<SeedSpawner>();
    }

    private void OnApplicationQuit()
    {
        spawnSeed = false;
    }

    private void OnDisable()
    {
        if (spawnSeed && (energyStored > 0 || nutritionalValue > 0))
            seedSpawner.PlantSeed(energyStored + nutritionalValue);
    }

}
