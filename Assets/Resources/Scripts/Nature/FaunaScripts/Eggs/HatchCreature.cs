using UnityEngine;

public class HatchCreature : ObjectSpawner
{
    public GameObject creatureToHatch;

    
    void SpawnCreature()
    {
        EnergyData eggEData = GetComponentInChildren<EnergyData>();
        SpawnObject(creatureToHatch, 0, false, null, eggEData, eggEData.energyReserve + eggEData.nutritionalValue);

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
