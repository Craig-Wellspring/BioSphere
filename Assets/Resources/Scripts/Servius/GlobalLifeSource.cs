using UnityEngine;

public class GlobalLifeSource : AdvancedMonoBehaviour
{
    [SerializeField] private bool initialize = true;
    [Space(15)]
    public float energyReserve = 0;
    public float minimumEnergyReserve;
    [Space(10)]
    public GameObject meteor;
    [SerializeField] private Transform spawnPoint;


    void Start()
    {
        if (initialize)
        {
            SpawnMeteor();
            FindObjectOfType<Meteor>().transform.localPosition = new Vector3(100, 100, 200);
        }
    }

    //// Spawn new Meteor from Spawnpoint \\\\
    public void SpawnMeteor()
    {
        //Create Meteor
        GameObject _newMeteor = Instantiate(meteor, spawnPoint.position, transform.rotation);
        _newMeteor.name = meteor.name;

        _newMeteor.GetComponent<Meteor>().guardian.GetComponentInChildren<EnergyData>().energyReserve += minimumEnergyReserve;
        energyReserve -= minimumEnergyReserve;
    }
}
