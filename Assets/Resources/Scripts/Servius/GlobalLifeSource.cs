using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalLifeSource : AdvancedMonoBehaviour
{
    public float lifeEnergyPool = 0;
    [Space(10)]
    public GameObject meteor;
    public Transform spawnPoint;
    [Space(10)]
    [Header("Manual")]
    public bool spawnMeteor = false;


    private void Start()
    {
        SpawnMeteor();
        FindObjectOfType<Meteor>().transform.localPosition = new Vector3(100, 100, 300);
    }

    //// Spawn new Meteor from Spawnpoint \\\\
    public void SpawnMeteor()
    {
        //Create Meteor
        GameObject _newMeteor = Instantiate(meteor, spawnPoint.position, transform.rotation);
        _newMeteor.name = meteor.name;
    }

    //// Manual Launch Button \\\\
    private void OnValidate()
    {
        if (spawnMeteor)
        {
            SpawnMeteor();
            spawnMeteor = false;
        }
    }
}
