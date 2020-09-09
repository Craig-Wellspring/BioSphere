using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : AdvancedMonoBehaviour
{
    // Singleton
    public static CloudSpawner Spawner { get; private set; }
    void Awake()
    {
        if (Spawner == null)
            Spawner = this;
        else
            Destroy(gameObject);
    }


    // Settings
    public int numberOfClouds = 100;
    [Header("Settings")]
    [SerializeField] private Vector2 cloudSizeMinXMaxY;
    [SerializeField] private List<GameObject> cloudTypes;




    // Spawn initial clouds
    void Start()
    {
        for(int i = 0; i < numberOfClouds; i++)
            SpawnCloud(false);   
    }


    public void SpawnCloud(bool _increaseCount)
    {
        Vector3 spawnPos = Random.onUnitSphere * Random.Range(145, 160);

        GameObject newCloud = Instantiate(cloudTypes[Random.Range(0,cloudTypes.Count)], spawnPos, Quaternion.identity, transform);
        newCloud.name = "Cloud";
        newCloud.transform.rotation = Quaternion.FromToRotation(newCloud.transform.up, -GravityVector(newCloud.transform.position)) * newCloud.transform.rotation;
        newCloud.transform.localScale = new Vector3(Random.Range(cloudSizeMinXMaxY.x, cloudSizeMinXMaxY.y), Random.Range(cloudSizeMinXMaxY.x, cloudSizeMinXMaxY.y), Random.Range(cloudSizeMinXMaxY.x, cloudSizeMinXMaxY.y));

        if (_increaseCount)
            numberOfClouds += 1;
    }
}
