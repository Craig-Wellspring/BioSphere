using UnityEngine;

public class HatchCreature : ObjectSpawner
{
    [Space(15)]
    public GameObject creatureToHatch;

    
    void SpawnCreature()
    {
        EnergyData eggEData = GetComponent<EnergyData>();
        FoodData eggFData = GetComponentInChildren<FoodData>();
        
        // Return nutritional value to energy storage
        eggEData.energyReserve += eggFData.nutritionalValue.x;
        eggFData.nutritionalValue.x = 0;

        // Spawn creature with energy storage
        GameObject newCreature = SpawnObject(creatureToHatch, eggEData, eggEData.energyReserve);


        // Switch SoulCam to hatched creature
        if (PlayerSoul.Cam.currentTarget == transform.root.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>())
            PlayerSoul.Cam.SwitchCamTo(newCreature.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>());

        // Copy parent genetics
        GetComponent<Genetics>().CopyGenes(newCreature.GetComponentInChildren<Genetics>());

        // Activate special effects
        Effects();
    }

    void Effects()
    {
        GameObject FXObj = GetComponentInChildren<ParticleSystem>(true).gameObject;
        FXObj.transform.position = transform.GetChild(0).position;
        FXObj.SetActive(true);
    }


    void CrackShell()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        Destroy(transform.root.gameObject, 2);
    }
}
