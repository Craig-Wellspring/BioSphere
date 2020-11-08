using UnityEngine;

[RequireComponent(typeof(EnergyData))]
public class Reproduction : ObjectSpawner
{
    [Header("Seed Settings")]
    [SerializeField] GameObject offspringSeed = null;
    [Space(10)]
    [Tooltip("Maximum energy passed on to Seed offspring. Energy beyond threshold will be returned to Source. If 0, SeedNV will be used.")]
    [SerializeField] float maxSeedEnergy = 50f;
    [SerializeField, Range(0, 50)] int seedingRadius = 2;
    [Tooltip("Must spawn seed above sea level.")]
    [SerializeField] bool aboveWaterOnly = true;
    [SerializeField] bool randomYRotation = true;
    [SerializeField] bool spawnScale0 = true;


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
    [SerializeField] bool logSeedLaying = false;
    [SerializeField] bool logEggLaying = false;


    // Cache
    EnergyData eData;
    void Start()
    {
        eData = GetComponentInParent<EnergyData>();
    }


    #region Seeds
    //// Spawn Seed \\\\
    public void SpawnSeed(float _energyEndowed, GameObject _offspringSeed = null)
    {
        // Choose Cast-off Seed
        if (_offspringSeed == null)
            _offspringSeed = offspringSeed;

        // If maxSeedEnergy is 0, use SeedNV as max
        if (maxSeedEnergy == 0)
            maxSeedEnergy = _offspringSeed.GetComponentInChildren<FoodData>().nutritionalValue.y;

        // Return excess energy to source if more than maximum
        if (_energyEndowed > maxSeedEnergy)
        {
            eData.ReturnEnergyToReserve(_energyEndowed - maxSeedEnergy);
            _energyEndowed = maxSeedEnergy;
        }

        // Expend Energy and plant Seed with the Energy spent
        GameObject spawnedFruit = SpawnObject(_offspringSeed, eData, _energyEndowed, null, randomYRotation, seedingRadius, aboveWaterOnly);

        // Adjust scale
        if (spawnScale0)
            spawnedFruit.GetComponentInChildren<Animator>(true).transform.localScale = Vector3.zero;

        // Debug
        if (logSeedLaying)
            Debug.Log(transform.root.name + " planted a Seed " + "[" + _offspringSeed.name + "]");
    }

    public void PlantSeedButton()
    {
        if (eData.energyReserve < maxSeedEnergy)
        {
            if (Servius.Server.GetComponent<GlobalLifeSource>().energyReserve > maxSeedEnergy)
            {
                float energyToLeech = maxSeedEnergy - eData.energyReserve;
                eData.energyReserve += energyToLeech;
                Servius.Server.GetComponent<GlobalLifeSource>().energyReserve -= energyToLeech;
            }
            else Debug.LogWarning("Not enough Energy remaining in Global Pool to Spawn Seed");
        }

        if (eData.energyReserve >= maxSeedEnergy)
            SpawnEgg(maxSeedEnergy);
    }
    #endregion



    #region Eggs
    //// Spawn Egg \\\\
    public void SpawnEgg(float _energyEndowed, GameObject _customOffspring = null)
    {
        // Return excess energy to source if more than maximum
        if (_energyEndowed > maxEggEnergy)
        {
            eData.ReturnEnergyToReserve(_energyEndowed - maxEggEnergy);
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
    }

    public void LayEggButton()
    {
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
