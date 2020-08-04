using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : AdvancedMonoBehaviour
{
    public List<GameObject> clouds;
    public int numberOfClouds = 100;

    void Start()
    {
        for(int i = 0; i < numberOfClouds; i++)
        {
            Vector3 spawnPos = Random.onUnitSphere * Random.Range(145, 160);
            SpawnCloud(spawnPos);            
        }
    }

    private void SpawnCloud(Vector3 _spawnPos)
    {
        //Quaternion newRot = Quaternion.FromToRotation(transform.up, GravityUp().eulerAngles) * transform.rotation;
        GameObject newCloud = Instantiate(clouds[Random.Range(0,clouds.Count)], _spawnPos, transform.rotation, transform);
        newCloud.name = "Cloud";
        PlanetCore.Core.AlignWithGravity(newCloud.transform);
        //newCloud.transform.LookAt(PlanetCore.Core.transform, GravityUp().eulerAngles);
        //newCloud.transform.Rotate(-90,90,0);
        newCloud.transform.localScale = new Vector3(Random.Range(0.9f, 2.5f), Random.Range(0.9f, 2.5f), Random.Range(0.9f, 2.5f));
    }
}
