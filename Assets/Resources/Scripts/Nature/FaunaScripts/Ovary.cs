using UnityEngine;

[RequireComponent(typeof(EnergyData))]
public class Ovary : ObjectSpawner
{
    [Header("Settings")]
    [Tooltip("Return a percentage of the cost required to Lay Eggs and Seeds to the Source when spawned."), SerializeField]
    [Range(0, 1)] float returnEnergyToSource = 0.5f;

    [Header("Egg Settings")]
    [SerializeField] EggData eggData;
    [Space(10)]
    [SerializeField] GameObject eggToSpawn;
    [SerializeField, Range(0,1)] float energyAsNV = 0.2f;
    [SerializeField, Range(0, 3)] float incubationSpeed = 0.5f;
    [SerializeField] bool canHatch = true;

    [Header("Seed Settings")]
    [SerializeField] GameObject offspringSeed;
    [Space(10)]
    [SerializeField, Range(0, 50)] float seedingRadius = 2f;
    [SerializeField] bool randomYRotation = true;
    [SerializeField] bool spawnScale0 = true;


    [Header("Debug")]
    [SerializeField] bool logEggLaying = false;
    [SerializeField] bool logSeedLaying = false;


    // Cache
    EnergyData eData;
    void Start()
    {
        eData = GetComponent<EnergyData>();
    }


    #region Eggs
    //// Spawn Egg \\\\
    public void SpawnEgg(float _energyEndowed, GameObject _customOffspring = null)
    {
        // Spawn egg
        GameObject newEgg = SpawnObject(eggToSpawn, eData, _energyEndowed, returnEnergyToSource);

        // Assign name and pass on genetics
        newEgg.name = eggData.eggName;
        GetComponent<Genetics>().PassDownGenes(newEgg.GetComponent<Genetics>());
        GetComponent<Genetics>().numberOfChildren++;

        // Move nutritional value to egg yolk
        float eggNV = returnEnergyToSource > 0 ? (_energyEndowed * energyAsNV * returnEnergyToSource) : _energyEndowed * energyAsNV;
        newEgg.GetComponentInChildren<EnergyData>().RemoveEnergy(eggNV);
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
    }

    public void LayEggButton()
    {
        if (eData.energyReserve < eData.surplusThreshold)
        {
            if (Servius.Server.GetComponent<GlobalLifeSource>().energyReserve > eData.surplusThreshold)
            {
                float energyToLeech = eData.surplusThreshold - eData.energyReserve;
                eData.energyReserve += energyToLeech;
                Servius.Server.GetComponent<GlobalLifeSource>().energyReserve -= energyToLeech;

                SpawnEgg(eData.surplusThreshold);
            }
            else Debug.LogWarning("Not enough Energy remaining in Global Pool to Spawn Egg");
        }
    }
    #endregion

    #region Seeds
    //// Spawn Seed \\\\
    public void SpawnSeed(float _energyEndowed, GameObject _offspringSeed = null)
    {
        // Choose Cast-off Seed
        if (_offspringSeed == null)
            _offspringSeed = offspringSeed;

        // Expend Energy and plant Seed with the Energy spent
        GameObject spawnedFruit = SpawnObject(_offspringSeed, eData, _energyEndowed, returnEnergyToSource, null, randomYRotation, seedingRadius);

        // Adjust scale
        if (spawnScale0)
            spawnedFruit.GetComponentInChildren<Animator>(true).transform.localScale = Vector3.zero;

        // Debug
        if (logSeedLaying)
            Debug.Log(transform.root.name + " planted a Seed " + "[" + _offspringSeed.name + "]");
    }

    public void PlantSeedButton()
    {
        if (eData.energyReserve < eData.surplusThreshold)
        {
            if (Servius.Server.GetComponent<GlobalLifeSource>().energyReserve > eData.surplusThreshold)
            {
                float energyToLeech = eData.surplusThreshold - eData.energyReserve;
                eData.energyReserve += energyToLeech;
                Servius.Server.GetComponent<GlobalLifeSource>().energyReserve -= energyToLeech;

                SpawnSeed(eData.surplusThreshold);
            }
            else Debug.LogWarning("Not enough Energy remaining in Global Pool to Spawn Seed");
        }
    }
    #endregion
}
