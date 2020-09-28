using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectVariant : MonoBehaviour
{
    [SerializeField] List<GameObject> prefabList;

    void Start()
    {
        // Spawn random variant object
        int randomIndex = Random.Range(0, prefabList.Count);
        GameObject newObject = Instantiate(prefabList[randomIndex], transform.position, transform.rotation);
        newObject.name = this.name;


        // Pass down energy to random variant if applicable
        EnergyData eData = GetComponent<EnergyData>();
        if (eData != null)
        {
            EnergyData newEData = newObject.GetComponentInChildren<EnergyData>(true);
            newEData.energyReserve = eData.energyReserve;
            eData.energyReserve = 0;
        }
        NutritionalValue nv = GetComponent<NutritionalValue>();
        if (nv != null)
        {
            NutritionalValue newNV = newObject.GetComponentInChildren<NutritionalValue>(true);
            newNV.nutritionalValue = nv.nutritionalValue;
            nv.nutritionalValue = 0;
        }

        // Destroy spawner object
        Destroy(this.gameObject);
    }
}
