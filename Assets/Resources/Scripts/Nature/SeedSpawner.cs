using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedSpawner : AdvancedMonoBehaviour
{
    [SerializeField] private GameObject newSeed;
    public bool spawnSeed = true;

    [Header("Debug")]
    [SerializeField] private bool drawDebugSeed = false;
    
    float collectedEnergy;

    

    private void OnValidate()
    {
        //Debug trigger
        if (drawDebugSeed)
        {
            DrawDebug();
            drawDebugSeed = false;
        }
    }
    
    private void OnApplicationQuit()
    {
        spawnSeed = false;
    }

    private void OnDisable()
    {
        if (spawnSeed)
            CollectAndPlant();
    }

    //// Collect all Energy remaining then Plant a Seed with half the Energy and return the other half to the Global Deficit \\\\
    public void CollectAndPlant()
    {
        CollectEnergy();

        if (collectedEnergy > 0)
        {
            Servius.Server.transform.GetComponent<Panspermia>().globalEnergyReserve += collectedEnergy * 0.5f;
            Servius.Server.transform.GetComponent<Panspermia>().CheckForLaunch();

            PlantSeed(collectedEnergy * 0.5f);
            collectedEnergy = 0;
        }
    }

    private void CollectEnergy()
    {
        Metabolism metabolism = transform.root.GetComponentInChildren<Metabolism>();
        if (metabolism != null)
            if (metabolism.storedEnergy > 0)
            {
                collectedEnergy += metabolism.storedEnergy;
                metabolism.SpendEnergy(metabolism.storedEnergy);
            }

        FoodData fData = transform.root.GetComponentInChildren<FoodData>(true);
        if (fData != null)
            if (fData.energyStored > 0 || fData.nutritionalValue > 0)
            {
                collectedEnergy += fData.energyStored + fData.nutritionalValue;
                fData.energyStored = 0;
                fData.nutritionalValue = 0;
            }
    }


    //// Spawn default Seed \\\\
    public void PlantSeed(float _passDownEnergy)
    {
        //Find seed planting location
        Quaternion newRot = Quaternion.FromToRotation(transform.root.up, (transform.root.position - Vector3.zero).normalized) * transform.root.rotation;
        //Plant Seedgrass
        GameObject newFruit = (GameObject)Instantiate(newSeed, PointOnTerrainUnderPosition(transform.position), newRot);
        newFruit.name = newSeed.name;

        //Pass on remaining energy
        FoodData seedFData = newFruit.GetComponentInChildren<FoodData>();
        if (_passDownEnergy > seedFData.nutritionalValue)
            seedFData.energyStored = _passDownEnergy - seedFData.nutritionalValue;
        else
            seedFData.nutritionalValue = _passDownEnergy;
    }

    //// Spawn custom Seed \\\\
    public void PlantSeed(float _passDownEnergy, GameObject _customSeed)
    {
        //Find Seed planting location
        Quaternion newRot = Quaternion.FromToRotation(transform.root.up, (transform.root.position - Vector3.zero).normalized) * transform.root.rotation;
        //Plant new Seed
        GameObject newFruit = (GameObject)Instantiate(_customSeed, PointOnTerrainUnderPosition(transform.position), newRot);
        newFruit.name = _customSeed.name;

        //Pass on remaining energy
        FoodData seedFData = newFruit.GetComponentInChildren<FoodData>();
        if (_passDownEnergy > seedFData.nutritionalValue)
            seedFData.energyStored = _passDownEnergy - seedFData.nutritionalValue;
        else
            seedFData.nutritionalValue = _passDownEnergy;
    }


    private void DrawDebug()
    {
        //Find potential planting location
        Vector3 drawFromPos = transform.position;
        Vector3 drawSpherePoint = PointOnTerrainUnderPosition(transform.position);

        Debug.Log("drawFromPos: " + drawFromPos + ", drawSpherePoint: " + drawSpherePoint);

        //Draw Sphere at potential planting location
        Debug.DrawRay(drawFromPos, -(drawFromPos - Vector3.zero), Color.green, 10);
        GameObject debugSphere = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), drawSpherePoint, transform.rotation);
        debugSphere.transform.localScale *= 0.2f;
        Destroy(debugSphere, 10);
    }
}
