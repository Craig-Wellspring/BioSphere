using UnityEngine;

public class GlobalLifeSource : AdvancedMonoBehaviour
{
    [SerializeField] private bool initialize = true;
    [Space(15)]
    public float energyReserve = 0;
    public float minimumEnergyReserve;
    [Space(10)]
    [SerializeField] GameObject meteor;
    [SerializeField] float spawnRadius = 100;


    void Start()
    {
        if (initialize)
            SpawnMeteor();
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
