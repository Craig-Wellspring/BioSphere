using UnityEngine;

public class Reproduction : ObjectSpawner
{
    [Header("Egg Settings")]
    [SerializeField] EggData eggData = null;
    [Space(10)]
    [Tooltip("Maximum energy passed on to Egg offspring. Energy beyond threshold will be returned to Source.")]
    [SerializeField] float maxEggEnergy = 50f;
    [SerializeField] GameObject eggType = null;
    [SerializeField, Range(0, 1)] float energyAsNV = 0.2f;
    [SerializeField, Range(0, 3)] float incubationSpeed = 0.5f;
    [SerializeField] bool canHatch = true;


    [Header("Debug")]
    [SerializeField] bool logEggLaying = false;



    #region Eggs
    //// Spawn Egg \\\\
    public GameObject SpawnEgg(float _energyEndowed, GameObject _customOffspring = null)
    {
        EnergyData eData = GetComponentInParent<EnergyData>();

        // Return excess energy to source if more than maximum
        if (_energyEndowed > maxEggEnergy)
        {
            eData.ReturnEnergyToSource(_energyEndowed - maxEggEnergy);
            _energyEndowed = maxEggEnergy;
        }

        // Spawn egg
        GameObject newEgg = SpawnObject(eggType, eData, _energyEndowed);

        // Assign name and pass on genetics
        newEgg.name = eggData.eggName;
        GetComponent<Genetics>().PassDownGenes(newEgg.GetComponent<Genetics>());
        GetComponent<Genetics>().numberOfChildren++;

        // Move nutritional value to egg yolk
        float eggNV = _energyEndowed * energyAsNV;
        newEgg.GetComponent<EnergyData>().RemoveEnergy(eggNV);
        newEgg.GetComponentInChildren<FoodData>().AddNV(eggNV);

        // Set offspring
        newEgg.GetComponentInChildren<HatchCreature>().creatureToHatch = _customOffspring ? _customOffspring : eggData.offspringCreature;

        // Set color
        newEgg.GetComponentInChildren<MeshRenderer>().material.color = eggData.eggColor;

        // Set size
        newEgg.transform.GetChild(0).transform.localScale = new Vector3(eggData.eggSize, eggData.eggSize, eggData.eggSize);
        newEgg.GetComponentInChildren<Rigidbody>().mass = eggData.eggSize;

        // Set incubation speed
        newEgg.GetComponentInChildren<Animator>().SetFloat("IncubationSpeed", incubationSpeed);

        // Set hatchability
        newEgg.GetComponentInChildren<Animator>().SetBool("CanHatch", canHatch);


        // Debug
        if (logEggLaying)
            Debug.Log(transform.root.name + " laid an Egg " + "[" + _customOffspring.name + "]");

        
        return newEgg;
    }

    public void LayEggButton()
    {
        EnergyData eData = GetComponentInParent<EnergyData>();

        if (eData.energyReserve < maxEggEnergy)
        {
            if (Servius.Server.GetComponent<GlobalLifeSource>().energyReserve > maxEggEnergy)
            {
                float energyToLeech = maxEggEnergy - eData.energyReserve;
                eData.energyReserve += energyToLeech;
                Servius.Server.GetComponent<GlobalLifeSource>().energyReserve -= energyToLeech;

            }
            else Debug.LogWarning("Not enough Energy remaining in Global Pool to Spawn Egg");
        }

        if (eData.energyReserve >= maxEggEnergy)
            SpawnEgg(maxEggEnergy);
    }
    #endregion
}
