using UnityEngine;

public class GlobalLifeSource : MonoBehaviour
{
    [Header("State")]
    public float energyReserve = 0;


    [Header("Energy Settings")]
    public float minimumEnergyReserve = 200;
    public bool logEnergyTaken = false;
    public bool logEnergyReturn = false;


    [Header("Seeding Settings")]
    public GameObject seedToPlant;
    [Space(10)]

    [SerializeField, Range(0,100)] int quickStartSeeds = 20;


    [Header("Meteor Settings")]
    [SerializeField] GameObject meteor;
    [Space(10)]

    [SerializeField] bool spawnMeteor = true;
    [SerializeField] float spawnRadius = 100;


    void Start()
    {
        if (spawnMeteor)
            SpawnMeteor();

        if (quickStartSeeds > 0)
        {
            for (int i = 0; i < quickStartSeeds; i++)
                if (energyReserve > minimumEnergyReserve)
                {
                    Vector3 seedPos = UtilityFunctions.GroundBelowPosition(Random.onUnitSphere * spawnRadius).position;

                    while (!UtilityFunctions.IsAboveSeaLevel(seedPos))
                        seedPos = UtilityFunctions.GroundBelowPosition(Random.onUnitSphere * spawnRadius).position;

                    PlantSeedFromSource(seedPos);
                }
        }
    }

    public float EnergyAvailable(float _maxEnergy)
    {
        return (energyReserve > _maxEnergy + minimumEnergyReserve) ? _maxEnergy : energyReserve - minimumEnergyReserve;
    }

    //Plant Seedgrass
    public void PlantSeedFromSource(Vector3 _spawnPos)
    {
        float _energyToPlant = EnergyAvailable(seedToPlant.GetComponentInChildren<FoodData>().nutritionalValue.y);
        if (_energyToPlant > 0)
        {
            GameObject newSeed = Instantiate(seedToPlant, _spawnPos, Quaternion.identity, null);

            newSeed.transform.rotation = UtilityFunctions.GravityOrientedRotation(newSeed.transform);
            newSeed.name = seedToPlant.name;
            newSeed.GetComponent<EnergyData>().TakeEnergyFromSource(_energyToPlant);

            foreach (FoodData fData in newSeed.GetComponentsInChildren<FoodData>())
                fData.enabled = true;
        }
    }

    //// Spawn new Meteor from Spawnpoint \\\\
    public void SpawnMeteor()
    {
        Vector3 spawnPoint = Random.onUnitSphere * spawnRadius;

        //Create Meteor
        GameObject _newMeteor = Instantiate(meteor, spawnPoint, transform.rotation);
        _newMeteor.name = meteor.name;

        _newMeteor.GetComponent<Meteor>().guardian.GetComponentInChildren<EnergyData>().energyReserve += minimumEnergyReserve;
        energyReserve -= minimumEnergyReserve;
    }
}
