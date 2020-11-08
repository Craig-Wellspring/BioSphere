using UnityEngine;

public class GlobalLifeSource : MonoBehaviour
{
    [SerializeField] bool spawnMeteor = true;
    [SerializeField] bool quickStart = false;
    public GameObject seedToPlant;


    [Header("Energy Settings")]
    public float energyReserve = 0;

    [Space(10)]
    public float minimumEnergyReserve = 200;
    public float maxEnergyPlanted = 1500;
    public bool logEnergyReturn = false;

    [Header("Meteor Settings")]
    [SerializeField] GameObject meteor;
    [SerializeField] float spawnRadius = 100;


    void Start()
    {
        if (spawnMeteor)
            SpawnMeteor();

        if (quickStart)
        {
            while (energyReserve > minimumEnergyReserve)
            {
                Vector3 seedPos = UtilityFunctions.GroundBelowPosition(Random.onUnitSphere * spawnRadius).position;

                while (!UtilityFunctions.AboveSeaLevel(seedPos))
                    seedPos = UtilityFunctions.GroundBelowPosition(Random.onUnitSphere * spawnRadius).position;

                PlantSeedFromSource(seedPos);
            }
        }
    }

    //Plant Seedgrass
    public void PlantSeedFromSource(Vector3 _spawnPos)
    {
        float _energyToPlant = (energyReserve > maxEnergyPlanted + minimumEnergyReserve) ? maxEnergyPlanted : energyReserve - minimumEnergyReserve;
        if (_energyToPlant > 0)
        {
            GameObject newSeed = Instantiate(seedToPlant, _spawnPos, Quaternion.identity, null);

            newSeed.transform.rotation = UtilityFunctions.GravityOrientedRotation(newSeed.transform);
            newSeed.name = seedToPlant.name;
            newSeed.GetComponent<EnergyData>().AddEnergy(_energyToPlant);
            energyReserve -= _energyToPlant;

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
