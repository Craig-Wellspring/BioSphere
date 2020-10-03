using UnityEngine;

[RequireComponent(typeof(EnergyData))]
public class Ovary : ObjectSpawner
{
    [Tooltip("Return a percentage of the cost required to Lay Eggs and Seeds to the Source when spawned."), SerializeField]
    [Range(0, 100)] float returnPercentToSource = 50;

    [Header("Egg Settings")]
    [SerializeField] GameObject offspringCreature;
    [Space(10)]
    [SerializeField] GameObject eggToSpawn;
    [SerializeField] string newEggName = "Egg";
    [SerializeField] Color newEggColor;
    [SerializeField, Range(0, 3)] float newEggSize = 1f;
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

    float percentEnergyToNV = 0.2f;


    // Cache
    EnergyData eData;
    void Start()
    {
        eData = GetComponent<EnergyData>();
    }

    #region Eggs
    //// Spawn Egg \\\\
    public void SpawnEgg(float _energyEndowed, GameObject _offspringCreature = null)
    {
        // Spawn egg
        GameObject newEgg = SpawnObject(eggToSpawn, eData, _energyEndowed, returnPercentToSource);
        if (newEgg != null)
        {
            // Assign name and pass on genetics
            newEgg.name = newEggName;
            GetComponent<Genetics>().PassDownGenes(newEgg.GetComponent<Genetics>());

            // Move nutritional value to egg yolk
            float eggNV = returnPercentToSource > 0 ? (_energyEndowed * percentEnergyToNV * (returnPercentToSource / 100)) : _energyEndowed * percentEnergyToNV;
            newEgg.GetComponentInChildren<EnergyData>().RemoveEnergy(eggNV);
            newEgg.GetComponentInChildren<FoodData>().AddNV(eggNV);

            // Set offspring
            if (_offspringCreature == null)
                _offspringCreature = offspringCreature;
            newEgg.GetComponentInChildren<HatchCreature>().creatureToHatch = _offspringCreature;

            // Set color
            newEgg.GetComponentInChildren<MeshRenderer>().material.color = newEggColor;

            // Set size
            newEgg.transform.GetChild(0).transform.localScale = new Vector3(newEggSize, newEggSize, newEggSize);
            newEgg.GetComponentInChildren<Rigidbody>().mass = newEggSize;

            // Set incubation speed
            newEgg.GetComponentInChildren<Animator>().SetFloat("IncubationSpeed", incubationSpeed);

            // Set hatchability
            newEgg.GetComponentInChildren<Animator>().SetBool("CanHatch", canHatch);
        }
        else
            Debug.LogError("Spawn Egg returned null", this);


        // Debug
        if (logEggLaying)
            Debug.Log(transform.root.name + " laid an Egg " + "[" + _offspringCreature.name + "]");
    }

    public void LayEggButton()
    {
        CreatureData cData = GetComponent<CreatureData>();
        if (eData.energyReserve < cData.levelUpCost)
        {
            if (Servius.Server.GetComponent<GlobalLifeSource>().energyReserve > cData.levelUpCost)
            {
                float energyToLeech = cData.levelUpCost - eData.energyReserve;
                eData.energyReserve += energyToLeech;
                Servius.Server.GetComponent<GlobalLifeSource>().energyReserve -= energyToLeech;

                SpawnEgg(cData.levelUpCost);
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

        // Expend Energy and plant Seed with the Energy spent to Evolve
        GameObject spawnedFruit = SpawnObject(_offspringSeed, eData, _energyEndowed, returnPercentToSource, null, randomYRotation, seedingRadius);

        // Adjust scale
        if (spawnScale0)
            spawnedFruit.GetComponentInChildren<Animator>(true).transform.localScale = Vector3.zero;


        // Debug
        if (logSeedLaying)
            Debug.Log(transform.root.name + " planted a Seed " + "[" + offspringSeed.name + "]");
    }

    public void PlantSeedButton()
    {
        CreatureData cData = GetComponent<CreatureData>();
        if (eData.energyReserve < cData.levelUpCost)
        {
            if (Servius.Server.GetComponent<GlobalLifeSource>().energyReserve > cData.levelUpCost)
            {
                float energyToLeech = cData.levelUpCost - eData.energyReserve;
                eData.energyReserve += energyToLeech;
                Servius.Server.GetComponent<GlobalLifeSource>().energyReserve -= energyToLeech;

                SpawnSeed(cData.levelUpCost);
            }
            else Debug.LogWarning("Not enough Energy remaining in Global Pool to Spawn Seed");
        }
    }
    #endregion
}
