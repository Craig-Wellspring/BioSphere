using UnityEngine;

public class HatchCreature : ObjectSpawner
{
    public GameObject creatureToHatch;

    
    void SpawnCreature()
    {
        EnergyData eggEData = GetComponentInChildren<EnergyData>();
        FoodData eggFData = GetComponentInChildren<FoodData>();
        
        // Return nutritional value to energy storage
        eggEData.energyReserve += eggFData.nutritionalValue;
        eggFData.nutritionalValue = 0;

        // Spawn creature with energy storage
        GameObject newCreature = SpawnObject(creatureToHatch, eggEData, eggEData.energyReserve);

        // Copy parent genetics
        GetComponent<Genetics>().CopyGenes(newCreature.GetComponentInChildren<Genetics>());

        // Activate special effects
        Effects();
    }

    void Effects()
    {
        GetComponentInChildren<ParticleSystem>(true).gameObject.SetActive(true);
    }


    void CrackShell()
    {
        Destroy(transform.root.gameObject);
    }
}
