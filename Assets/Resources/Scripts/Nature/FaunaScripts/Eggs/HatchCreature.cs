using UnityEngine;

public class HatchCreature : ObjectSpawner
{
    public GameObject creatureToHatch;

    
    void SpawnCreature()
    {
        EnergyData eggEData = GetComponentInChildren<EnergyData>();
        NutritionalValue eggNV = GetComponentInChildren<NutritionalValue>();
        
        // Return nutritional value to energy storage
        eggEData.energyReserve += eggNV.nutritionalValue;
        eggNV.nutritionalValue = 0;

        // Spawn creature with energy storage
        SpawnObject(creatureToHatch, eggEData, eggEData.energyReserve);

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
