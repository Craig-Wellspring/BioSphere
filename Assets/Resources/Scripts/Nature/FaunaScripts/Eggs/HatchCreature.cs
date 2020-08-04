using UnityEngine;

public class HatchCreature : ObjectSpawner
{
    public GameObject creatureToHatch;

    
    private void SpawnCreature()
    {
        EnergyData eggEData = GetComponentInChildren<EnergyData>();
        SpawnObject(creatureToHatch, 0, false, null, eggEData.energyReserve + eggEData.nutritionalValue, eggEData);
        eggEData.nutritionalValue = 0;
    }


    private void CrackShell()
    {
        Destroy(transform.root.gameObject);
    }
}
