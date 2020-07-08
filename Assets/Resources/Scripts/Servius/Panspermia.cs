using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panspermia : MonoBehaviour
{
    public GameObject meteor;
    public Transform spawnPoint;
    [Space(10)]
    public float energyDeficit = 0;
    public float launchThreshold = 1000;
    [Space(10)]
    public bool spawnMeteor = false;


    private void Start()
    {
        if (energyDeficit > 0)
        {
            //Initialize ServiusCam by attaching it to the first meteor spawned
            SpawnMeteor();

            ServiusCam.Cam.transform.SetParent(FindObjectOfType<Meteor>().transform.Find("CameraDock"), false);
            ServiusCam.Cam.ResetTransform();
        }
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

    //// Launch Meteor if Energy Deficit is above Threshold \\\\
    public void CheckForLaunch()
    {
        if (energyDeficit >= launchThreshold)
            SpawnMeteor();
    }


    //// Spawn new Meteor from Spawnpoint with current Energy Deficit \\\\
    private void SpawnMeteor()
    {
        //Create Meteor
        GameObject newMeteor = Instantiate(meteor, spawnPoint.position, transform.rotation);
        newMeteor.name = meteor.name;

        //Allocate Energy
        newMeteor.GetComponent<Meteor>().energyStored = energyDeficit;
        energyDeficit = 0;
    }
}
