using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalLifeSource : AdvancedMonoBehaviour
{
    public float lifeEnergyPool = 0;
    [Space(10)]
    [Header("Manual")]
    public GameObject meteor;
    public Transform spawnPoint;
    [Space(10)]
    public bool spawnMeteor = false;


    private void Start()
    {
        if (lifeEnergyPool > 0)
        {
            //Initialize ServiusCam by attaching it to the first meteor spawned
            SpawnMeteor();
            FindObjectOfType<Meteor>().transform.localPosition = new Vector3(100, 100, 300);

            ServiusCam.Cam.transform.SetParent(FindObjectOfType<Meteor>().transform.Find("CameraDock"), false);
            ResetTransform(ServiusCam.Cam.transform);
        }
    }

    //// Spawn new Meteor from Spawnpoint \\\\
    public void SpawnMeteor()
    {
        //Create Meteor
        GameObject newMeteor = Instantiate(meteor, spawnPoint.position, transform.rotation);
        newMeteor.name = meteor.name;
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
