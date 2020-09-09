using UnityEngine;

public class HatchCreature : ObjectSpawner
{
    public GameObject creatureToHatch;

    
    void SpawnCreature()
    {
        EnergyData eggEData = GetComponentInChildren<EnergyData>();
        SpawnObject(creatureToHatch, 0, false, null, eggEData.energyReserve + eggEData.nutritionalValue, eggEData);
    }


    void CrackShell()
    {
        Destroy(transform.root.gameObject);
    }
}
