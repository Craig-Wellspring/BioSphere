using UnityEngine;

public class GlobalLifeSource : MonoBehaviour
{
    [SerializeField] private bool initialize = true;

    [Header("Energy Settings")]
    public float energyReserve = 0;
    public float minimumEnergyReserve;
    public bool logEnergyReturn = false;

    [Header("Meteor Settings")]
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
